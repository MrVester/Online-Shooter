using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(CircleCollider2D),typeof(Rigidbody2D),typeof(SpriteRenderer))]
public class Weapon : MonoBehaviour,IWeapon
{   
    //Add bullet constructor for weapon
    //Add bullet constructor for weapon
    //Add bullet constructor for weapon
    public int currentAmmo { get; private set; }
    public bool isDropped { get; private set; } = true;
   
    [field:SerializeField] public WeaponDataSO Data { get; private set; }
    private CircleCollider2D weaponCollider;
     private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PhotonView PV;
    public event Action WeaponEquipped;
    private float cooldownTimestamp = 0;
    public LayerMask raycastToShoot;

    private void Awake()
    {
        raycastToShoot = LayerMask.NameToLayer("Player");
        currentAmmo = Data.ammo;
         PV=GetComponent<PhotonView>();
        _spriteRenderer=GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Data.sprite;
        _spriteRenderer.sortingOrder = 1;
        weaponCollider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
       
    }
    bool TryShoot(IWeapon weapon)
    {
        if (Time.time < cooldownTimestamp) return false;
        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            return false;
        }
        cooldownTimestamp = Time.time + (weapon.Data.shootingSpeed / 10);
        currentAmmo -= 1;
        return true;
    }

    public void Shoot(IWeapon weapon)
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(GetComponentInParent<PlayerController>().Look);
        var direction = (mouseScreenPosition - (Vector2)transform.position).normalized;
        Ray2D ray = new Ray2D(transform.position, direction);
        RaycastHit2D hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        if (TryShoot(weapon) && (hit = Physics2D.Raycast(ray.origin, ray.direction, int.MaxValue, ~raycastToShoot)))
        {
            
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(weapon.Data.damage);
            print("We hit " + hit.collider.gameObject.name);
            print("Shooting " + weapon.Data.name);
        }

    }

    public void Dropped()
    {
        isDropped = true;
        weaponCollider.enabled = true;
        _rb.simulated = true;
    }
    public void Equiped()
    {
        isDropped = false;
        weaponCollider.enabled = false;
         _rb.simulated = false;
        WeaponEquipped?.Invoke();
    }


    public void Disolve()
    {
        if(currentAmmo<=0)
        {
            currentAmmo = 0;
            //Disolve time = 3-5s
        }
        else
        {
            if (isDropped)
                print("Do smt");
            //Disolve time = 10s
        }
    }
    public void DestroyWeapon()
    {
        PhotonNetwork.Destroy(PV);
    }

}

public interface IWeapon
{   public WeaponDataSO Data{ get; }
   
    public bool isDropped { get; }
    public void Dropped();
    public void Equiped();
    public void Disolve();
    public event Action WeaponEquipped;
    public int currentAmmo { get; }
    public void Shoot(IWeapon weapon);

}
