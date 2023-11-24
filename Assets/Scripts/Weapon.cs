using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D),typeof(Rigidbody2D))]
public class Weapon : MonoBehaviour,IWeapon
{
   
    [field:SerializeField] public WeaponDataSO Data { get; private set; }
    //public GameObject weaponGO;
    private CircleCollider2D weaponCollider;
    private Rigidbody2D _rb;
    public int currentAmmo { get; private set; }
    public bool isDropped { get; private set; } = true;

    private void Awake()
    {
        weaponCollider = GetComponent<CircleCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }


    public void Droped()
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

  
}

public interface IWeapon
{
    public void Droped();
    public void Equiped();
    public void Disolve();
    public bool isDropped { get; }
}
