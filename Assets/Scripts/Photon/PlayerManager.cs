using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject controller;
    SpawnPointManager sPManager;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            sPManager = FindObjectOfType<SpawnPointManager>();
            CreateController();
        }
    }
    private void CreateController()
    {
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),sPManager.GetRandomPos(),Quaternion.identity,0, new object[] { PV.ViewID});
    }
    public void Die()
    {
        controller.GetComponent<WeaponController>().equippedWeapon?.DestroyWeapon();
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
