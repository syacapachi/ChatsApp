using System;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;
using Firebase.Firestore;
using System.Collections.Generic;


//���S�ȃA�N�Z�X�C���^�[�t�F�[�X
//���[�U�[�̑����ɉ����ăA�N�Z�X�\�ȃw�b�_�[���Ⴄ(��ʃ��[�U�[���Ǘ��Ҍ����ɃA�N�Z�X�ł��Ȃ��悤�ɂ�����)
public interface IAuthService 
{
    string CurrentUserId { get; }
    string UserName { get; }
    void SignUp(string email, string password,string username, System.Action<bool, string> callback);
    void SignIn(string email, string password, System.Action<bool, string> callback);
    void SignOut();
}

public interface ISettingsService : IAuthService
{
    FirebaseAuth settingAuth { get; }
    void DeleteUserData(System.Action<bool, string> callback);
}

public class FirebaseAuthService : IAuthService,ISettingsService
{
    private FirebaseAuth auth;
    private FirebaseUser currentUser = null;
    
    public FirebaseAuthService()
    {
        auth = FirebaseAuth.DefaultInstance;
        //currentUser = Firebase.Auth.FirebaseUser
    }
    public string CurrentUserId => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser?.UserId;

    public string UserName => Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;

    public FirebaseAuth settingAuth => auth;


    public async void SignUp(string email, string password, string userName, System.Action<bool, string> callback)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var currentUser = result.User;

            if (string.IsNullOrEmpty(userName))
            {
                userName = currentUser.UserId;
            }

            // ���[�U�[�v���t�B�[���X�V
            await currentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
            {
                DisplayName = userName
            });

            // Firestore�ɕۑ�
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
            var userDoc = new Dictionary<string, object>()
            {
                { "username", userName },
                { "email", email },
                { "iconUrl", "" },               // ���Firebase Storage�ɃA�b�v���[�h���Đݒ�
                { "statusMessage", "" },         // �v���t�B�[���R�����g
                { "createdAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "updatedAt", Timestamp.GetCurrentTimestamp().ToString() },
                { "frends",new List<string>() },
                { "chatRooms",new List<string>()}
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

    public async void SignIn(string email, string password, System.Action<bool, string> callback)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var currentUser = result.User;

            callback(true, "���O�C������");


        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
    }

    public void SignOut()
    {
        auth.SignOut();
        currentUser = null;
        //return Task.CompletedTask;
    }
    private void CurrentUser()
    {
        currentUser = auth.CurrentUser;
        Debug.Log("Get CurrentUser!");
    } 
    public async void DeleteUserData(System.Action<bool, string> callback)
    {
        CurrentUser();
        try
        {
            await currentUser.DeleteAsync();
            callback(true, "���[�U�[�f�[�^�폜����");
        }
        catch (Exception e)
        {
            callback(false, e.ToString());
        }
        
    }
}

