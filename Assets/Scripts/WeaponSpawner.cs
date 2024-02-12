using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    private WeaponList _weaponList;
    private PhotonView PV;
    private Weapon currentWeapon;
    public Vector2 spawnOffset;
    public bool TestToggle=false;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _weaponList = GetComponent<WeaponList>();
    }
    void Start()
    {       
      currentWeapon = SpawnRandomWeapon<Weapon>(new Vector3(spawnOffset.x, spawnOffset.y, 0) + transform.position);
    }

    private void Update()
    {
        if (TestToggle)
        {
            TestToggle = false;
            currentWeapon = SpawnRandomWeapon<Weapon>(new Vector3(spawnOffset.x, spawnOffset.y, 0) + transform.position);
        }
    }

    public T SpawnRandomWeapon<T>(Vector3 coords)
    {
        return SpawnWeapon<T>(_weaponList.RandomWeaponName(),coords);
    }

    public T SpawnWeapon<T>(string weapon, Vector3 coords)
    {
        T genericObj;
        GameObject weaponObj = new GameObject();

        weaponObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", weapon), coords, Quaternion.identity, 0, new object[] { PV.ViewID });

        weaponObj.TryGetComponent<T>(out genericObj);
        return genericObj;
    }
}
