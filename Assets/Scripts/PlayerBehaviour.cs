using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerBehaviour : MonoBehaviourPunCallbacks
{
    private Vector3 playerVectorColor;
    private PhotonView playerPV;
    private SpriteRenderer playerRenderer;

    private void Awake()
    {
        playerPV = GetComponent<PhotonView>();
        playerRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {

        playerVectorColor = new Vector3(Random.Range(100f, 255f) / 255f, Random.Range(100f, 255f) / 255f, Random.Range(100f, 255f) / 255f);
        if(playerPV.IsMine)
        SetPlayerColor(playerVectorColor);


    }

    public void SetPlayerColor(Vector3 playerColorToSet)
    {

        playerRenderer.color = new Color(playerColorToSet.x, playerColorToSet.y, playerColorToSet.z, 1);
        if (playerPV.IsMine)
        {
            
            Hashtable hash = new Hashtable();
            hash.Add("playerColor", playerColorToSet);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!playerPV.IsMine && targetPlayer == playerPV.Owner)
        {
            SetPlayerColor((Vector3)changedProps["playerColor"]);
        }

    }

    void Update()
    {
       
    }
   
}
