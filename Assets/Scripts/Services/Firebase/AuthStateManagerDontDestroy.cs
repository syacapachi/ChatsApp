using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth; // Firebase Authentication�̃C���|�[�g
using System;
using System.Threading.Tasks; // EventHandler�̂��߂ɕK�v

public class AuthStateManagerDontDestroy : MonoBehaviour
{
    // FirebaseAuth�̃C���X�^���X��ێ�����ϐ�
    private FirebaseAuth auth;

    void Awake()
    {
        //�j������Ȃ��悤�ɂ���
        DontDestroyOnLoad(gameObject);
        // FirebaseAuth�̃f�t�H���g�C���X�^���X���擾
        auth = FirebaseAuth.DefaultInstance;

        // StateChanged�C�x���g�Ƀ��X�i�[��o�^
        // ���̃��X�i�[�́A�F�؏�Ԃ��ς�邽�т�OnAuthStateChanged���\�b�h���Ăяo��
        auth.StateChanged += OnAuthStateChanged;
        Debug.Log("FirebaseAuth.StateChanged ���X�i�[��o�^���܂����B");
    }

    // �F�؏�Ԃ��ύX���ꂽ�Ƃ��ɌĂяo����郁�\�b�h
    void OnAuthStateChanged(object sender, EventArgs eventArgs)
    {
        // ���݂̃��[�U�[�����擾

        if (AuthManagerBase.Instance.CurrentUserId != null)
        {
            Task.Run(async ()=> await AuthManagerBase.Instance.OnLogin());
            Task.Run(async () => await AdministerManagerBase.Instance.OnLogin());
            // ���[�U�[�����O�C�����Ă���ꍇ
            Debug.Log($"���[�U�[�����O�C�����܂���: {AuthManagerBase.Instance.CurrentUserName ?? "�s���ȃ��[�U�[��"} ({AuthManagerBase.Instance.UserEmail})");
            // ��: ���O�C����̉�ʂɑJ�ڂ���A���O�C��UI���\���ɂ���
            if (SceneManager.GetActiveScene().name != "TestUserHomeScene")
            {
                SceneManager.LoadScene("TestUserHomeScene");
            }

        }
        else
        {
            // ���[�U�[�����O�A�E�g���Ă���A�܂��̓��O�C�����Ă��Ȃ��ꍇ
            Debug.Log("���[�U�[�����O�A�E�g���܂����A�܂��̓��O�C�����Ă��܂���B");
            // ��: ���O�C����ʂ�\������A�Q�[���̃��C�����j���[�ɖ߂�
            if (SceneManager.GetActiveScene().name != "TestLoginScene")
            {
                SceneManager.LoadScene("TestLoginScene");
            }

        }
    }

    void OnDestroy()
    {
        // �I�u�W�F�N�g���j�������ۂɁA���������[�N��h�����߃��X�i�[�̓o�^����������
        if (auth != null)
        {
            auth.StateChanged -= OnAuthStateChanged;
            Debug.Log("FirebaseAuth.StateChanged ���X�i�[���������܂����B");
        }
    }

    
}
