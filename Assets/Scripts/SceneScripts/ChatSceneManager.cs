using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform messageContainer; // ScrollViewのContent
    [SerializeField] private GameObject myMessagePrefab;   // メッセージ用のPrefab
    [SerializeField] private GameObject otherMessagePrefab;
    [SerializeField] private TextMeshProUGUI inputField;      // 送信用テキストボックス



    void Start()
    {
        ChatManagerBase.Instance.OnMessageReceived += AddMessageToUI;
        // 履歴をリアルタイムで受信
        //ルーム選択画面で直接実行できるため、そっちのほうが良い
        //ChatManagerBase.Instance.StartListenMessages();
    }
    

    void OnDestroy()
    {
        ChatManagerBase.Instance.OnMessageReceived -= AddMessageToUI;
        ChatManagerBase.Instance.StopLister();
    }

    private void AddMessageToUI(string message,string username,string timestamp)
    {
        GameObject prefab = (username == AuthManagerBase.Instance.CurrentUserName) ? myMessagePrefab : otherMessagePrefab;
        GameObject msgObj = Instantiate(prefab, messageContainer);
        var textComp = msgObj.GetComponentInChildren<TextMeshProUGUI>();
        textComp.text = $"[{username}]:{message}\n{timestamp}";
        Debug.Log($"message:{message}\nuser:{username}\n{timestamp}");
    }
    public void OnSendButtonClick()
    {
        ChatManagerBase.Instance.SendMessage(inputField.text);
        inputField.text = "";
    }

}
