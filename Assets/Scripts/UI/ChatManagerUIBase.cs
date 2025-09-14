using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
/// 
public class ChatManagerUIBase : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button sendButton;
    [SerializeField] Transform content; // ScrollView �� Content
    [SerializeField] GameObject messagePrefab;

    private FirebaseFirestore db;
    private FirebaseUser user;
    private ListenerRegistration listener;

    [Header("Room Settings")]
    public string roomId; // �I�����ꂽ�`���b�g���[����ID

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        user = FirebaseAuth.DefaultInstance.CurrentUser;

        sendButton.onClick.AddListener(SendMessage);

        // ���������A���^�C���Ŏ�M
        ListenMessages();
    }

    void OnDestroy()
    {
        listener?.Stop();
    }

    void SendMessage()
    {
        if (string.IsNullOrEmpty(inputField.text)) return;

        var msg = new Dictionary<string, object>
        {
            { "senderId", user.UserId },
            { "message", inputField.text },
            { "timestamp", Timestamp.GetCurrentTimestamp().ToString() }
        };

        db.Collection("chatRooms").Document(roomId)
          .Collection("messages").AddAsync(msg);

        inputField.text = ""; // ���͗����N���A
    }

    void ListenMessages()
    {
        listener = db.Collection("chatRooms").Document(roomId)
            .Collection("messages")
            .OrderBy("timestamp")
            .Listen(snapshot =>
            {
                // �Â����b�Z�[�W����x�N���A�i�ȈՎ����j
                foreach (Transform child in content)
                {
                    Destroy(child.gameObject);
                }

                // �ŐV�̃��b�Z�[�W��UI�ɔ��f
                foreach (var doc in snapshot.Documents)
                {
                    string senderId = doc.GetValue<string>("senderId");
                    string text = doc.GetValue<string>("message");

                    GameObject msgObj = Instantiate(messagePrefab, content);
                    var msgText = msgObj.GetComponentInChildren<TMP_Text>();
                    msgText.text = text;

                    // �����̃��b�Z�[�W�͐F��ʒu��ς���
                    if (senderId == user.UserId)
                    {
                        msgText.color = Color.green;
                        msgObj.GetComponent<RectTransform>().anchoredPosition += new Vector2(200, 0);
                    }
                    else
                    {
                        msgText.color = Color.white;
                    }
                }
            });
    }
}
