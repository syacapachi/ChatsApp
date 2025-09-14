using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatSceneManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform messageContainer; // ScrollView��Content
    [SerializeField] private GameObject myMessagePrefab;   // ���b�Z�[�W�p��Prefab
    [SerializeField] private GameObject otherMessagePrefab;
    [SerializeField] private TextMeshProUGUI inputField;      // ���M�p�e�L�X�g�{�b�N�X



    void Start()
    {
        ChatManagerBase.Instance.OnMessageReceived += AddMessageToUI;
        // ���������A���^�C���Ŏ�M
        //���[���I����ʂŒ��ڎ��s�ł��邽�߁A�������̂ق����ǂ�
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
