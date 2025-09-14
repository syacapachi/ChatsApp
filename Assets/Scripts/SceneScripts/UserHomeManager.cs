using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserHomeManager : MonoBehaviour
{
    [SerializeField] private Transform roomListParent;   // GridLayoutGroupやVerticalLayoutGroupをアタッチした親
    [SerializeField] private GameObject roomButtonPrefab;
    [SerializeField] TextMeshProUGUI userNameGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        userNameGUI.text = AuthManagerBase.Instance.CurrentUserName + AuthManagerBase.Instance.CrrentUserUintId.ToString();
        ChatRoomsManagerBase.Instance.OnChatRoomsReceived += OnRoomLoad;
        ChatRoomsManagerBase.Instance.LoadRoomsAsync();
    }

    // Update is called once per frame
    void OnDestroy()
    {
        ChatRoomsManagerBase.Instance.OnChatRoomsReceived -= OnRoomLoad;
    }
    public void OnRoomLoad(string roomName, string roomId)
    {
        GameObject buttonObj = Instantiate(roomButtonPrefab, roomListParent);
        RoomButton button = buttonObj.GetComponent<RoomButton>();
        button.Setup(roomId, roomName);
    }
    public void OnSignOutButtonClick()
    {
        AuthManagerBase.Instance.SignOut();
        Debug.Log("SiginOut Success!");
    }
    public void OnCreateRoomButtonClick()
    {
        string userid = AuthManagerBase.Instance.CurrentUserId;
        Debug.Log($"CreateButton OnClick User:{userid}");
        ChatRoomsManagerBase.Instance.MakeRoomAsync(userid);
        ChatRoomsManagerBase.Instance.LoadRoomsAsync();
    }
}
