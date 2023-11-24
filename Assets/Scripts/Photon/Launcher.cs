using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    public int gameSceneIndex = 1;
    public static Launcher Instance { get; private set; }

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListPrefab;
    [SerializeField] GameObject playerListPrefab;
    [SerializeField] GameObject startGameButton;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Debug.Log("Connecting to Master");
        MenuManager.Instance.OpenMenu("Loading");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void StartGame()
    {

        PhotonNetwork.LoadLevel(gameSceneIndex);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene=true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }
   
  public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        roomNameInputField.text = null;
        MenuManager.Instance.OpenMenu("Loading");
       
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(player);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        errorText.text = "Joining room failed: " + message;
        MenuManager.Instance.OpenMenu("Error");
        Debug.Log(message);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: "+message;
        MenuManager.Instance.OpenMenu("Error");
        Debug.Log(message);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
       
    }
    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
       foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
                continue;
            Instantiate(roomListPrefab,roomListContent).GetComponent<RoomListItem>().Setup(room);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }
}
