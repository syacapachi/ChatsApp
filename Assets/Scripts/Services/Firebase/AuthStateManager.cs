using Firebase.Auth;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using static UnityEngine.CullingGroup;

public partial class AuthStateManager : AuthStateMangerBase
{
    private FirebaseAuth auth;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new AuthStateManager();
    }
    private AuthStateManager()
    {
        auth = FirebaseAuth.DefaultInstance;
        
    }
    public override event Action<string> OnUserStateChanged;
    public override event Action<string> OnIdChanged;

    public override void StartListenState()
    {
        auth.StateChanged += OnStateChange;
    }
    public override void StopListenState()
    {
        auth.StateChanged -= OnStateChange;
    }

    private void OnStateChange(object sender,EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            OnUserStateChanged?.Invoke("Login");
        }
        else
        {
            OnUserStateChanged?.Invoke("LogOut");
        }
        
    }
    public override void StartLintenId()
    {
        auth.IdTokenChanged += OnIdChange;
    }
    public override void StopLintenId()
    {
        auth.IdTokenChanged -= OnIdChange;
    }
    private void OnIdChange(object sender,EventArgs eventArgs)
    {
        if(auth.CurrentUser != null)
        {
            OnIdChanged?.Invoke("SessionRunning");
        }
        else
        {
            OnIdChanged?.Invoke("SessionTimeOut");
        }
        
    }

    // ��Ƃ��āA���O�C���ƃ��O�A�E�g�̃_�~�[���\�b�h
    // �����͖{���A���O�C���{�^���⃍�O�A�E�g�{�^���̃N���b�N�C�x���g�ȂǂŌĂяo����܂�
    public override void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("�������O�C�����L�����Z������܂����B");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"�������O�C���Ɏ��s���܂���: {task.Exception}");
                return;
            }
            FirebaseUser newUser = task.Result.User;
            Debug.Log($"�������[�U�[�Ƃ��ă��O�C�����܂���: {newUser.UserId}");
        });
    }

    public override void SignOutAnonymously()
    {
        auth.SignOut();
        Debug.Log("���[�U�[�����O�A�E�g���܂����B");
    }
}

