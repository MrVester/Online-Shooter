using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerBehaviour : MonoBehaviourPunCallbacks
{
    private Vector3 playerVectorColor;
    private PhotonView PV;
    private SpriteRenderer playerRenderer;
    private HashController _hashController;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerRenderer = GetComponent<SpriteRenderer>();
        _hashController = GetComponent<HashController>();


    }

    private void Start()
    {

        playerVectorColor = new Vector3(Random.Range(100f, 255f) / 255f, Random.Range(100f, 255f) / 255f, Random.Range(100f, 255f) / 255f);
        if (PV.IsMine)
        {
            SetPlayerColor(playerVectorColor);
        }
        


    }

    public void SetPlayerColor(Vector3 playerColorToSet)
    {

        playerRenderer.color = new Color(playerColorToSet.x, playerColorToSet.y, playerColorToSet.z, 1);
        if (PV.IsMine)
        {


            _hashController.Add("playerColor", playerColorToSet);
            PhotonNetwork.LocalPlayer.SetCustomProperties(_hashController.GetHash());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("playerColor"))
            {
                SetPlayerColor((Vector3)changedProps["playerColor"]);
                _hashController.Remove("playerColor");
            }
        }

    }
   
}
