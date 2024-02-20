using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class WeaponSpawner : MonoBehaviour
{
    private WeaponList _weaponList;
    private PhotonView PV;
    private Weapon currentWeapon=null;
    public SpriteRenderer buttonSprite;
    public Vector2 spawnOffset;
    private new bool enabled = true;
    public float spawnCoolDown = 10f;
    private float currentCoolDown=0f;


    private Vector3 pressedPos = Vector3.zero;
    private Vector3 unpressedPos = new Vector3(0,0.15f,0);
    private Coroutine timerCoroutine;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _weaponList = GetComponent<WeaponList>();
        
    }
    void Start()
    {
        EnableButton();
        // currentWeapon = SpawnRandomWeapon<Weapon>(new Vector3(spawnOffset.x, spawnOffset.y, 0) + transform.position);
    }

    private void Update()
    {

    }
    public bool isWeaponNull()
    {
        return currentWeapon==null;
    }
   
    public T SpawnRandomWeapon<T>(Vector3 coords)
    {
        return SpawnWeapon<T>(_weaponList.RandomWeaponName(), coords);
    }

    public T SpawnWeapon<T>(string weapon, Vector3 coords)
    {
        T genericObj;
        GameObject weaponObj = new GameObject();

        weaponObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", weapon), coords, Quaternion.identity, 0, new object[] { PV.ViewID });

        weaponObj.TryGetComponent<T>(out genericObj);
        return genericObj;
    }
    public void StartTimer()
    {
        if (currentWeapon != null)
        {
            currentWeapon.WeaponEquipped -= StartTimer;
            currentWeapon = null;
        }
            currentCoolDown = spawnCoolDown;
            timerCoroutine = StartCoroutine(Timer()) ;
        
        
       
    }
   public void StopTimer()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        currentCoolDown = 0f;
    }
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);
        currentCoolDown -=1f;
        if (currentCoolDown <= 0)
        {
            timerCoroutine = null;
            //After timer
            EnableButton();
        }
        else
        {
            //Not uses now
            //RefreshTimer_S();
            timerCoroutine=StartCoroutine(Timer());
        }
    }
    public enum EventCodes : byte
    {
        NewPlayer,
        UpdatePlayers,
        ChangeStat,
        NewMatch,
        RefreshTimer
    }
    public void RefreshTimer_S()
    {
        object[] package = new object[] { currentCoolDown };
        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.RefreshTimer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability=true});
    }
    public void RefreshTimer_R(object[] data)
    {
        currentCoolDown = (int)data[0];
        //RefreshTimerUI();
    }

    public void EnableButton()
    {
        PV.RPC("RPC_EnableButton", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_EnableButton()
    {
        buttonSprite.transform.localPosition=unpressedPos;
        enabled = true;
    }

    public void DisableButton()
    {
        PV.RPC("RPC_DisableButton", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_DisableButton()
    {
        buttonSprite.transform.localPosition = pressedPos;
        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        print("Collider name entered spawner: "+collider.name);
        PlayerController tmpPlayer;
        var isPlayer = collider.gameObject.TryGetComponent(out tmpPlayer);
        if (isPlayer && currentWeapon==null && enabled)
        {
            DisableButton();
            currentWeapon = SpawnRandomWeapon<Weapon>(new Vector3(spawnOffset.x, spawnOffset.y, 0) + transform.position);
            //To spawn weapon on equip and delete StartTimer call function line
            //currentWeapon.WeaponEquipped+= StartTimer;

            StartTimer();
            PV.RPC("RPC_DisableButton", RpcTarget.All);

        }
    }
    
}
