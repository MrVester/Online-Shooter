using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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

    private void Awake()
    {
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
    [PunRPC]
    public void RPC_Destroy()
    {
        Destroy(gameObject);
    }
}

public interface IWeapon
{   public WeaponDataSO Data{ get; }
   
    public bool isDropped { get; }
    public void Dropped();
    public void Equiped();
    public void Disolve();

}
