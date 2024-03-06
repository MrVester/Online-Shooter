using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : MonoBehaviourPunCallbacks
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
        SpawnDeathParticles();
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
    private void SpawnDeathParticles()
    {
      PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "DeathParticles"), controller.transform.position, Quaternion.identity, 0, new object[] { PV.ViewID });
    }
    /*public override void OnJoinedLobby()
    {
        sPManager = FindObjectOfType<SpawnPointManager>();
        CreateController();
    }*/
}
