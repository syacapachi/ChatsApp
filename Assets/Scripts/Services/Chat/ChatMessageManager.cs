using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.UI;

public partial class ChatManager : ChatManagerBase
{
    private FirebaseFirestore db;
    private FirebaseUser user;
    //„ÉÅEÅE„ÇøÊõ¥Êñ∞„ÇíË¶ã„Çã„ÇÅEÅ§
    private ListenerRegistration listener;
    private string roomId;
    public override event Action<string,string,string> OnMessageReceived;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Instance = new ChatManager();
        Debug.Log("ChatManager(Firebase)èâä˙âªäÆóπ");
    }
    private ChatManager()
    {
        db = FirebaseFirestore.DefaultInstance;
        user = FirebaseAuth.DefaultInstance.CurrentUser;
    }
   
    public async override void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return;
        try
        {
            var msg = new Dictionary<string, object>
            {
                { "senderId", user.UserId },
                { "senderName",user.DisplayName},
                { "message", message },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };

            await db.Collection("chatRooms").Document(roomId)
              .Collection("messages").AddAsync(msg);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        

    }
    public override void SendPicture(string pictureURL)
    {
        throw new NotImplementedException();
    }
    public override void StartListenMessages(string _roomId)
    {
        roomId = _roomId;
        listener = db.Collection("chatRooms").Document(roomId)
            .Collection("messages")
            .OrderBy("timestamp")
            .Listen(async snapshot =>
            {
                foreach (var docChange in snapshot.GetChanges())
                {
                    if (docChange.ChangeType == DocumentChange.Type.Added)
                    {
                        string userID = docChange.Document.GetValue<string>("senderId");
                        string username = await GetUserName(userID);
                        string msg = docChange.Document.GetValue<string>("message");
                        string timestamp = docChange.Document.GetValue<string>("timestamp");
                        Debug.Log($"[Firestore] Âèó‰ø°: {msg}");
                        OnMessageReceived?.Invoke(msg,username,timestamp); // UIÂÅ¥„Å∏ÈÄöÁü•
                    }
                }
            });
    }

    public override void StopLister()
    {
        listener?.Dispose();
    }
    private async Task<string> GetUserName(string userId)
    {
        DocumentSnapshot snapshot = await db.Collection("users").Document(userId).GetSnapshotAsync();
        if (snapshot.Exists && snapshot.ContainsField("username"))
        {
            return snapshot.GetValue<string>("username");
        }
        return "Unknown";
    }
}
