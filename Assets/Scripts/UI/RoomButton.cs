// RoomButton.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.EditorTools;
using UnityEngine.SceneManagement;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    private string roomId;

    public void Setup(string id, string name)
    {
        roomId = id;
        roomNameText.text = name;
    }

    public void OnClick()
    {
        // ルームIDを保存してシーン遷移
        //Listenlerを起動
        SceneManager.LoadScene("TestChatScene");
        ChatManagerBase.Instance.StartListenMessages(roomId);
    }
}
