using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class WeaponController : MonoBehaviour
{

    public GameObject weaponHolder;
    public GameObject hand;
    [SerializeField]private Weapon currentWeapon;
    public Vector2 dropWeaponOffset;
    private IPlayerController _player;
    private PlayerInput _input;
    private int flipVector;
    private string deviceName;

    private bool isAttacked;


    private PhotonView PV;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        PV = GetComponent<PhotonView>();
        _player = GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Equiped();
        }
        else
        {
            currentWeapon.Droped();
        }
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
            ((Mathf.Abs(rotZ) < 90 && sc.y > 0) || (Mathf.Abs(rotZ) > 90 && sc.y < 0)) && (_player.FlipVector == 1|| _player.FlipVector == -1)
            )
            hand.transform.localScale = new Vector3(sc.x , sc.y * -1, sc.z);
    }

    private void FlipHand()
    {
        var pos = hand.transform.localPosition;
        if ((_player.FlipVector == 1 && pos.x < 0) || (_player.FlipVector == -1 && pos.x > 0))
            hand.transform.localPosition = new Vector3(pos.x * -1, pos.y, pos.z);
    }

  

    public void EquipWeapon(Weapon weapon)
    {
        if (currentWeapon!=null)
        {
            DropWeapon(currentWeapon);
        }
        weapon.Equiped();
        currentWeapon = weapon;
        weapon.gameObject.transform.parent = weaponHolder.transform;
        weapon.gameObject.transform.position = weaponHolder.transform.position;
        weapon.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        weapon.gameObject.transform.localScale = Vector3.one;
    }

    
     public void DropWeapon(Weapon weapon)
     {
        Weapon instantiatedWeapon = Instantiate(weapon, transform.position + new Vector3(dropWeaponOffset.x*flipVector, dropWeaponOffset.y,0), Quaternion.Euler(Vector3.zero));
        instantiatedWeapon.Droped();
        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
     }

     public void OnCollisionEnter2D(Collision2D weapon)
     {
        
         Weapon tmpWeapon;
         var isWeapon=weapon.gameObject.TryGetComponent(out tmpWeapon);
        print("Entered collider, isWeapon: "+ isWeapon);
        if (isWeapon)
         {
            print("Entered collider weapon");
            EquipWeapon(tmpWeapon);
         }
     }
}
