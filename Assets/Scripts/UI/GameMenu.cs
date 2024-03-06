using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject Menu;
    private PlayerInput _input;
    public int lobbyIndex = 0;
    // Start is called before the first frame update
    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }
    void Start()
    {
        if(Menu.activeSelf)
        DisactivateMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (_input.FrameInput.Menu)
        {
            print("ESC");
            HandleMenu();
        }
    }
    private void HandleMenu()
    {
        if (!Menu.activeSelf)
        {
            ActivateMenu();
        }
        else
        {
            DisactivateMenu();
        }
    }
    private void ActivateMenu()
    {
        Menu.SetActive(true);
    }
    private void DisactivateMenu()
    {
        Menu.SetActive(false);
    }
    public void LeaveRoom()
    {
        DisactivateMenu();
       // PhotonNetwork.RemoveBufferedRPCs();
        PhotonNetwork.LeaveRoom(true);
    }
    public override void OnLeftRoom()
    {
        StartCoroutine(WaitToLeave());
    }

    IEnumerator WaitToLeave()
    {
        while (PhotonNetwork.InRoom)
            yield return null;
        SceneManager.LoadScene(lobbyIndex);
    }
}
