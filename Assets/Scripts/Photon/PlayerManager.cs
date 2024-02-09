using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }
    private void CreateController()
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),Vector3.zero,Quaternion.identity,0, new object[] { PV.ViewID});
    }
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
