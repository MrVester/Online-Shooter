using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class WeaponController : MonoBehaviourPunCallbacks
{

    public GameObject weaponHolder;
    public GameObject hand;
    [SerializeField] private Weapon currentWeapon;
    public Vector2 dropWeaponOffset;
    private IPlayerController _player;
    private PlayerInput _input;
    private WeaponList _weaponList;
    private HashController _hashController;

    private int flipVector;
    private string deviceName;

    private bool isAttacked;


    private PhotonView PV;

    private void Awake()
    {
        _weaponList = GetComponent<WeaponList>();
        _input = GetComponent<PlayerInput>();
        _player = GetComponent<PlayerController>();
        _hashController = GetComponent<HashController>();
        PV = GetComponent<PhotonView>();

    }

    private void Start()
    {
        if (PV.IsMine)
        {
            int count = _weaponList.WeaponCount();
           // int randWeapon = Random.Range(0, count - 1);
            int randWeapon = 2;

            if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Weapon1") /*+ (randWeapon + 1))*/, new Vector3(2, 0, 0), Quaternion.identity);
            //var instWeapon = Instantiate(_weaponList.GetWeaponByID(randWeapon));
            EquipWeapon(randWeapon);
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



    public void EquipWeapon(int weaponID) //FIX NOT EQUIPPING WEAPON ON  !PV.IsMine CLIENT
    {
        if (currentWeapon != null && PV.IsMine)
        {
            DropWeapon(currentWeapon.ID);
        }

        Weapon tmpWeapon = Instantiate(_weaponList.GetWeaponByID(weaponID));
        
        tmpWeapon.Equiped();
        currentWeapon = tmpWeapon;
        tmpWeapon.gameObject.transform.parent = weaponHolder.transform;
        tmpWeapon.gameObject.transform.position = weaponHolder.transform.position;
        tmpWeapon.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        tmpWeapon.gameObject.transform.localScale = Vector3.one;
        if (PV.IsMine)
        {

            _hashController.Add("equipWeaponID", weaponID);
            PhotonNetwork.LocalPlayer.SetCustomProperties(_hashController.GetHash());
        }
    }

    [PunRPC]
    public void EquipWeaponRpc()
    {

    }
    public void DropWeapon(int weaponID)
    {
        if (PV.IsMine)
        {
            GameObject instantiatedWeapon = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Weapon" + (weaponID + 1)), transform.position + new Vector3(dropWeaponOffset.x * flipVector, dropWeaponOffset.y, 0), Quaternion.Euler(Vector3.zero));
            instantiatedWeapon.GetComponent<Weapon>().Dropped();
        }
           
        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
        if (PV.IsMine)
        {
            _hashController.Add("dropWeaponID", weaponID);
            PhotonNetwork.LocalPlayer.SetCustomProperties(_hashController.GetHash());
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            print("PlayerPropertiesUpdated");
            if (changedProps.ContainsKey("equipWeaponID"))
            {
                print("ContainsKey(\"equipWeaponID\")");
                EquipWeapon((int)changedProps["equipWeaponID"]);
                _hashController.Remove("equipWeaponID");
            }

            if (changedProps.ContainsKey("dropWeaponID"))
            {
                print("ContainsKey(\"dropWeaponID\")");
                DropWeapon((int)changedProps["dropWeaponID"]);
                _hashController.Remove("dropWeaponID");
            }

        }

    }

    public void OnCollisionEnter2D(Collision2D weapon)
    {

        Weapon tmpWeapon;
        var isWeapon = weapon.gameObject.TryGetComponent(out tmpWeapon);
        //print("Entered collider, isWeapon: "+ isWeapon);
        if (isWeapon && PV.IsMine)
        {
            // print("Entered collider weapon");

            EquipWeapon(tmpWeapon.ID);
            weapon.gameObject.GetComponent<PhotonView>().RPC("DestroyRpc", RpcTarget.All);
            // PhotonNetwork.Destroy(weapon.gameObject);
        }
    }
}