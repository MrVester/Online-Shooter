using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class WeaponController : MonoBehaviourPunCallbacks, IWeaponController
{

    [SerializeField]private GameObject weaponHolder;
    [SerializeField] private GameObject hand;
    [SerializeField] public Weapon equippedWeapon { get; set; }
    public Vector2 dropWeaponOffset;
    private IPlayerController _player;
    private PlayerInput _input;
    private int flipVector;
    private string deviceName;


    private bool isAttacked;

    private float cooldownTimestamp=0;

    private PhotonView PV;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _player = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>();

    }
    private void Start()
    {

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
    

    bool TryShoot(IWeapon weapon)
    {
        if (Time.time < cooldownTimestamp) return false;
        cooldownTimestamp = Time.time + (weapon.Data.shootingSpeed / 10);
        return true;
    }

    public void Shoot(IWeapon weapon)
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(_player.Look);
        var direction = (mouseScreenPosition - (Vector2)hand.transform.position).normalized;
        Ray2D ray = new Ray2D(hand.transform.position , direction);
        RaycastHit2D hit;
       
        Debug.DrawRay(ray.origin, ray.direction,Color.red);
        if (TryShoot(weapon)&&(hit=Physics2D.Raycast(ray.origin , ray.direction)))
        {   
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(weapon.Data.damage);
            print("We hit "+ hit.collider.gameObject.name);
            print("Shooting " + weapon.Data.name);
        }
       
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


    public void EquipWeapon(Weapon weapon)
    {
        int id = weapon.GetComponent<PhotonView>().ViewID;
        PV.RPC("RPC_EquipWeapon", RpcTarget.All, id);
    }
        [PunRPC]
    public void RPC_EquipWeapon(int weaponid)
    {
        if (equippedWeapon != null && PV.IsMine)
        {
            DropWeapon(equippedWeapon, dropWeaponOffset);
        }
        Weapon weapon = PhotonView.Find(weaponid).GetComponent<Weapon>();
        
        weapon.Equiped();
        equippedWeapon = weapon;
        weapon.transform.parent = weaponHolder.transform;
        weapon.transform.position = weaponHolder.transform.position;
        weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        weapon.transform.localScale = Vector3.one;
        
    }

    public void DropWeapon(Weapon weapon,Vector2 dropWeaponOffset)
    {
        int id = weapon.GetComponent<PhotonView>().ViewID;
        PV.RPC("RPC_DropWeapon", RpcTarget.All, id, dropWeaponOffset);
    }
    [PunRPC]
    public void RPC_DropWeapon(int weaponid, Vector2 dropWeaponOffset)
    {
        Weapon weapon;
        Vector3 offsetCoords = transform.position + new Vector3(dropWeaponOffset.x * flipVector, dropWeaponOffset.y, 0);

        if (equippedWeapon == null)
            return;

        PhotonView.Find(weaponid).TryGetComponent(out weapon);

        weapon.transform.parent = null;
        weapon.transform.position = offsetCoords;
        weapon.transform.localRotation = Quaternion.Euler(Vector3.zero);
        weapon.transform.localScale = Vector3.one;
        weapon.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        weapon.Dropped();
        equippedWeapon = null;
        
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
        if (isWeapon)
        {

            EquipWeapon(tmpWeapon);

        }
    }

}
public interface IWeaponController
{   public Weapon equippedWeapon { get; set; }
    public void Shoot(IWeapon weapon);

}