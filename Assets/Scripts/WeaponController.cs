using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class WeaponController : MonoBehaviourPunCallbacks
{

    [SerializeField]private GameObject weaponHolder;
    [SerializeField] private GameObject hand;
    [SerializeField] private Weapon currentWeapon;
    public Vector2 dropWeaponOffset;
    private IPlayerController _player;
    private PlayerInput _input;
    private WeaponList _weaponList;

    private int flipVector;
    private string deviceName;

    private bool isAttacked;


    private PhotonView PV;

    private void Awake()
    {
        _weaponList = GetComponent<WeaponList>();
        _input = GetComponent<PlayerInput>();
        _player = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();

    }

    private void Start()
    {
        //Now i am instantiating objects by Data.name from photon resources and to get weapons with
        //changed properties (in game etc. bullets amount) is it necessary to just sync that properties?
        //The problem is i have to name resources the same name in WeaponSO
        if (PV.IsMine)
        {
            int weaponCount = _weaponList.WeaponCount();
            int randWeapon = Random.Range(0, weaponCount - 1);

            //Spawn 1 rand weapon for test
            if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", _weaponList.GetList()[randWeapon].Data.name), new Vector3(5, 0, 0), Quaternion.identity);
            
            //Equip 1 rand weapon per player for test
            EquipWeapon(_weaponList.GetList()[randWeapon].Data.name);
        }
        /*  if (currentWeapon != null)
          {
              currentWeapon.Equiped();
          }*/

    }
    private void Update()
    {

        if (!PV.IsMine)
            return;
        isAttacked = _input.FrameInput.AttackDown;
        //deviceName = _input.FrameInput.CurrentDevice;
        flipVector = _player.FlipVector; //print(deviceName);
        HandRotation();
        FlipHand();
    }

    private void HandRotation()
    {
        //follow coursor and gamepadstick axis
        float AngleRad;


        //For Mouse
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(_player.Look);
        var direction = (mouseScreenPosition - (Vector2)hand.transform.position).normalized;
        AngleRad = Mathf.Atan2(direction.y, direction.x);

        //For gamepad
        // AngleRad = Mathf.Atan2(_player.Look.y, _player.Look.x);

        float angle = (180 / Mathf.PI) * AngleRad;
        hand.transform.rotation = Quaternion.Euler(0, 0, angle);


        //if >180 Z rotation then flip weapon
        var rotZ = hand.transform.eulerAngles.z - 180;
        var sc = hand.transform.localScale;

        if (
            ((Mathf.Abs(rotZ) < 90 && sc.y > 0) || (Mathf.Abs(rotZ) > 90 && sc.y < 0)) && (_player.FlipVector == 1 || _player.FlipVector == -1)
            )
            hand.transform.localScale = new Vector3(sc.x, sc.y * -1, sc.z);
    }

    private void FlipHand()
    {
        var pos = hand.transform.localPosition;
        if ((_player.FlipVector == 1 && pos.x < 0) || (_player.FlipVector == -1 && pos.x > 0))
            hand.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
    }



    public void EquipWeapon(string weaponName) 
    {
        if (currentWeapon != null && PV.IsMine)
        {
            DropWeapon(currentWeapon.Data.name,dropWeaponOffset);
        }

        Weapon tmpWeapon = Instantiate(_weaponList.GetWeaponByName(weaponName));
        
        tmpWeapon.Equiped();
        currentWeapon = tmpWeapon;
        tmpWeapon.gameObject.transform.parent = weaponHolder.transform;
        tmpWeapon.gameObject.transform.position = weaponHolder.transform.position;
        tmpWeapon.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        tmpWeapon.gameObject.transform.localScale = Vector3.one;
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("equipWeaponNAME", weaponName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    [PunRPC]
    public void EquipWeaponRpc()
    {

    }
    public void DropWeapon(string weaponName, Vector2 dropWeaponOffset)
    {
        if (PV.IsMine)
        {
            int weaponIndexbbyName = _weaponList.GetList().FindIndex(go => go.Data.name == weaponName);
            //Add bullet constructor for weapon
           

            GameObject instantiatedWeapon = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", weaponName), transform.position + new Vector3(dropWeaponOffset.x * flipVector, dropWeaponOffset.y, 0), Quaternion.Euler(Vector3.zero));
            instantiatedWeapon.GetComponent<Weapon>().Dropped();
        }
           
        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("dropWeaponNAME", weaponName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
           // print("PlayerPropertiesUpdated");
            if (changedProps.ContainsKey("equipWeaponNAME"))
            {
                //print("EQUIPED WEAPON WITH ID: "+ (int)changedProps["equipWeaponID"]);
                EquipWeapon((string)changedProps["equipWeaponNAME"]);
            }

            if (changedProps.ContainsKey("dropWeaponNAME"))
            {
                //print("DROPPED WEAPON WITH ID: " + (int)changedProps["dropWeaponID"]);
                DropWeapon((string)changedProps["dropWeaponNAME"],dropWeaponOffset);
            }

        }

    }


    /*public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarning($"Player disconnected: {cause}");
    }*/

    public void OnCollisionEnter2D(Collision2D weapon)
    {

        Weapon tmpWeapon;
        var isWeapon = weapon.gameObject.TryGetComponent(out tmpWeapon);
        //print("Entered collider, isWeapon: "+ isWeapon);
        if (isWeapon && PV.IsMine)
        {
            // print("Entered collider weapon");

            EquipWeapon(tmpWeapon.Data.name);
            weapon.gameObject.GetComponent<PhotonView>().RPC("DestroyRpc", RpcTarget.All);
            // PhotonNetwork.Destroy(weapon.gameObject);
        }
    }
}