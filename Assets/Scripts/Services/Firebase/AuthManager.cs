using System;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class AuthManager : AuthManagerBase
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private FirebaseFirestore db;
    static uint NullId = 0;
    private uint uintId = NullId;
    // ���̂���������(���̃R�[�h��Unity���V�[�������[�h����O�ɌĂ�)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new AuthManager();
        Debug.Log("AuthManager(Firebase) ����������");
    }

    private AuthManager()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
        Task.Run(async () => await OnLogin());
    }
    public override async Task OnLogin()
    {
        Debug.Log("AuthManagerBase.Instance.OnLogin()");
        DocumentSnapshot userdocsnapshot = await db.Collection("users").Document(AuthManagerBase.Instance.CurrentUserId).GetSnapshotAsync();
        if (userdocsnapshot.Exists)
        {
            Debug.Log("uintId Update");
            uintId = userdocsnapshot.GetValue<uint>("uintId");
        }
        else
        {
            Debug.Log("uintId is not Exist");
            uintId = NullId;
        }
    }
    public override string CurrentUserId => auth.CurrentUser?.UserId;
    public override string CurrentUserName => auth.CurrentUser?.DisplayName;
    public override string UserEmail=> currentUser?.Email;

    public override uint CrrentUserUintId
    {
        get 
        {
            Task.Run(async () => await OnLogin());
            return uintId; 
        }
    }

    public async override void SignUp(string email, string password, string username, Action<bool, string> callback)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var currentUser = result.User;
            System.Random rand = new System.Random();
            uint candicate = NullId;
            candicate = (uint)rand.Next(int.MinValue,int.MaxValue);
            //�ł���Ώd�������Ƃ��̏���.
            uintId = candicate;

            if (string.IsNullOrEmpty(username))
            {
                username = currentUser.UserId;
            }

            // ���[�U�[�v���t�B�[���X�V
            await currentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
            {
                DisplayName = username
            });

            // Firestore�ɕۑ�
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            var userDoc = new Dictionary<string, object>()
            {
                { "username", username },
                { "email", email },
                { "iconUrl", "" },               // ���Firebase Storage�ɃA�b�v���[�h���Đݒ�
                { "statusMessage", "" },         // �v���t�B�[���R�����g
                { "createdAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "updatedAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "frends",new List<string>() },
                { "chatRooms",new List<string>()},
                { "uintId", candicate}
            };

            await db.Collection("users").Document(currentUser.UserId).SetAsync(userDoc);

            Debug.Log("���[�U�[���o�^����");
            callback(true, "�T�C���A�b�v����");
        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public async override void SignIn(string email, string password, Action<bool, string> callback)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            await OnLogin();
            var currentUser = result.User;
            

            callback(true, "���O�C������");


        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public override void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        uintId = 0;
    }
}
