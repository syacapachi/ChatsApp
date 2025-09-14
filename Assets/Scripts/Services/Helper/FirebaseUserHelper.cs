using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseUserHelper : MonoBehaviour
{
    FirebaseAuth auth;
    FirebaseFirestore db;

    async void Start()
    {
        await InitFirebase();
        await SignInAnonymouslyIfNeededAndSave();
        // �f�o�b�O�p�F�v���W�F�N�gID�ƌ��݃��[�U�[�����O
        Debug.Log("ProjectId: " + Firebase.FirebaseApp.DefaultInstance.Options.ProjectId);
        Debug.Log("CurrentUser: " + (auth.CurrentUser != null ? auth.CurrentUser.UserId : "null"));
    }

    async Task InitFirebase()
    {
        var dep = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dep != DependencyStatus.Available)
        {
            Debug.LogError("Firebase dependencies not available: " + dep);
            return;
        }

        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }

    // �i�e�X�g�p�j�������O�C�����Ă���ۑ�����ꍇ
    public async Task SignInAnonymouslyIfNeededAndSave()
    {
        if (auth.CurrentUser == null)
        {
            var signResult = await auth.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously: " + signResult.User.UserId);
        }

        await SaveUserToFirestore(); // �ۑ����s��
    }

    // ���ۂ� firestore �ɕۑ����郁�\�b�h
    public async Task SaveUserToFirestore()
    {
        if (auth == null || db == null)
        {
            Debug.LogError("Firebase not initialized");
            return;
        }

        var user = auth.CurrentUser;
        if (user == null)
        {
            Debug.LogError("User not signed in");
            return;
        }

        string uid = user.UserId; // SDK�ɂ�� property���� UserId / Uid �̏ꍇ������܂��B���Ȃ���SDK�ɍ��킹�Ă��������B
        var userDoc = new Dictionary<string, object>()
        {
            { "username", user.DisplayName ?? "NoName" },
            { "email", user.Email ?? "" },
            { "updatedAt", Timestamp.GetCurrentTimestamp().ToString() }
        };

        try
        {
            await db.Collection("users").Document(uid).SetAsync(userDoc);
            Debug.Log("Saved user document for uid: " + uid);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to write user doc: " + e);
        }
    }

    // �ۑ������h�L�������g��ǂݎ���Ċm�F���郁�\�b�h
    public async Task ReadUserFromFirestore()
    {
        var user = auth.CurrentUser;
        if (user == null) { Debug.LogError("Not signed in"); return; }

        try
        {
            var snap = await db.Collection("users").Document(user.UserId).GetSnapshotAsync();
            if (snap.Exists)
            {
                Debug.Log("Firestore doc exists:");
                foreach (var field in snap.ToDictionary())
                {
                    Debug.Log(field.Key + " = " + field.Value);
                }
            }
            else
            {
                Debug.Log("No user document found in Firestore for uid: " + user.UserId);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Read failed: " + e);
        }
    }
}
