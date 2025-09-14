using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public partial class ChatRoomsManger : ChatRoomsManagerBase
{
    FirebaseFirestore db;
    public override event Action<string,string> OnChatRoomsReceived;
    public override event Action<List<(string groupId, string groupName)>> OnFoundRoomsReceived;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        Instance = new ChatRoomsManger();
        Debug.Log("ChatRoomsManager(Firebase) 初期化完了");
    }
    private ChatRoomsManger()
    {
        db = FirebaseFirestore.DefaultInstance;
    }
    public override async void  MakeRoomAsync(string roomname,string ownerId, List<string> membersId)
    {
        try
        {
            DocumentReference newChatRoomRef = db.Collection("groups").Document();
            // ← Document() を引数なしで呼ぶと自動でランダムIDが割り当てられる
            if (!membersId.Contains(ownerId)) 
            { 
                membersId.Add(ownerId);
            }
                
            //初期化
            db.Collection("chatRooms").Document(newChatRoomRef.Id);
            var docs = new Dictionary<string, object>()
            {
                {"roomname",roomname },
                {"owner",ownerId},
                {"members",membersId },
                {"createAT",Timestamp.GetCurrentTimestamp().ToString() },

            };
            await newChatRoomRef.SetAsync(docs);
            //Chatログ保存
            var welcomeMessage = new Dictionary<string, object>()
            {
                { "senderId", ownerId },
                { "senderName", "System" },
                { "message", $"グループ {roomname} が作成されました！" },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };
            await newChatRoomRef.Collection("message").AddAsync(welcomeMessage);

            //ユーザーの所属グループに登録
            await db.Collection("users").Document(ownerId)
              .UpdateAsync("chatRooms", FieldValue.ArrayUnion(newChatRoomRef.Id));

            foreach (var memberId in membersId) 
            {
                await db.Collection("user").Document(memberId)
                    .UpdateAsync("chatRooms", FieldValue.ArrayUnion(newChatRoomRef.Id));
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

    }
    public override async void MakeRoomAsync(string ownerId)
    {
        try
        {
            DocumentReference newChatRoomRef = db.Collection("chatRooms").Document();
            // ← Document() を引数なしで呼ぶと自動でランダムIDが割り当てられる
            db.Collection("chatRooms").Document(newChatRoomRef.Id);
            var docs = new Dictionary<string, object>()
            {
                {"roomname",newChatRoomRef.Id },
                {"owner",ownerId},
                {"members",ownerId },
                {"createAT",Timestamp.GetCurrentTimestamp().ToString() },

            };
            await newChatRoomRef.SetAsync(docs);
            //Chatログ保存
            var welcomeMessage = new Dictionary<string, object>()
            {
                { "senderId", ownerId },
                { "senderName", "System" },
                { "message", $"グループ {newChatRoomRef.Id} が作成されました！" },
                { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
            };
            await newChatRoomRef.Collection("messages").AddAsync(welcomeMessage);

            //ユーザーの所属グループに登録
            await db.Collection("users").Document(ownerId)
              .UpdateAsync("chatRooms", FieldValue.ArrayUnion(newChatRoomRef.Id));
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public override async void JoinRoomsAsync(string userId, string roomId)
    {
        try
        {
            // グループにユーザーを追加
            await db.Collection("chatRooms").Document(roomId)
                .UpdateAsync("members", FieldValue.ArrayUnion(userId));

            // ユーザーにグループを追加
            await db.Collection("users").Document(userId)
                .UpdateAsync("chatRooms", FieldValue.ArrayUnion(roomId));

            Debug.Log($"ユーザー {userId} がグループ {roomId} に参加しました。");
        }
        catch (Exception e)
        { 
            Debug.LogError(e.Message);
        }
        
    }
    public override async void LoadRoomsAsync()
    {
        try { 
            // ログイン中のユーザーID
            string userId = AuthManagerBase.Instance.CurrentUserId;

            if (string.IsNullOrEmpty(userId))
            {
                UnityEngine.Debug.LogError("ユーザーがログインしていません");
                return;
            }
            // users/{userId}/ ドキュメントを取得
            DocumentSnapshot userdoc = await db.Collection("users")
                                        .Document(userId)
                                        .GetSnapshotAsync();
            if (userdoc.Exists)
            {
                List<string> roomIds = userdoc.GetValue<List<string>>("chatRooms");
                foreach (string Id in roomIds)
                {
                    string roomId = Id;
                    DocumentSnapshot chatdoc = await db.Collection("chatRooms").Document(roomId).GetSnapshotAsync();
                    if (chatdoc.Exists)
                    {
                        string roomName = chatdoc.GetValue<string>("roomname");
                        //var dic = chatdoc.ToDictionary();
                        //foreach (var kv in dic)
                        //{
                        //    Debug.Log($"Key{kv.Key},value:{kv.Value}");
                        //}
                        Debug.Log($"GetRoomname{roomName}");
                        Debug.Log($"GetRoomID{roomId}");
                        OnChatRoomsReceived?.Invoke(roomId, roomName);
                    }
                    else
                    {
                        Debug.Log("Key Is not Exists");
                    }
                }
            }
            else
            {
                Debug.Log("Room is not Exist");
            }
            
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log($"LoadRoomsAsync でエラー: {e}");
        }
    }
    public override void SearchGroupsByNamePrefix(string keyword)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 文字列の前方一致検索 (例: "Uni" -> "Unity Lovers")
        db.Collection("groups")
          .OrderBy("name")
          .StartAt(keyword)
          .EndAt(keyword + "\uf8ff")   // Unicode 最大値で範囲指定
          .GetSnapshotAsync()
          .ContinueWith(task =>
          {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.LogError("グループ検索失敗: " + task.Exception);
                  return;
              }

              List<(string, string)> results = new List<(string, string)>();

              foreach (var doc in task.Result.Documents)
              {
                  string groupId = doc.Id;
                  string groupName = doc.ContainsField("name") ? doc.GetValue<string>("name") : "(No Name)";
                  results.Add((groupId, groupName));
              }

              OnFoundRoomsReceived?.Invoke(results);
          });
    }

}
