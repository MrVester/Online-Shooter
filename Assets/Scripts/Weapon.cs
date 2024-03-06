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
    public CircleCollider2D triggerCollider;
    public CircleCollider2D physicalCollider;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private PhotonView PV;
    public event Action WeaponEquipped;
    private float cooldownTimestamp = 0;
    [SerializeField]
    private LayerMask raycastToShoot;
    private LayerMask raycastToCheck;
    public event Action BulletShot;
    public Transform shootParticlePos;
    private ParticleSystem shotParticles;

    private void Awake()
    {
        //Default+Player physical Layers = 65
        raycastToShoot.value = 65;
        raycastToCheck.value = 1;
        //raycastToShoot = LayerMask.NameToLayer("Player");
        
        currentAmmo = Data.ammo;
         PV=GetComponent<PhotonView>();
        
        _spriteRenderer=GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Data.sprite;
        _spriteRenderer.sortingOrder = 1;
        //weaponColliders = GetComponents<CircleCollider2D>();

        _rb = GetComponent<Rigidbody2D>();
        shotParticles=GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
       
    }
    //ADD BULLET SYNC OVER CLIENTS
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
        BulletShot?.Invoke();
        ShotParticles();
        return true;
    }
    public void ShotParticles()
    {
        PV.RPC("RPC_ShotParticles", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_ShotParticles()
    {
        shotParticles.Play();
    }
    
   
    public void Shoot(IWeapon weapon)
    {
        Vector2 mouseScreenPosition = Camera.main.ScreenToWorldPoint(GetComponentInParent<PlayerController>().Look);
        var direction = (mouseScreenPosition - (Vector2)shootParticlePos.position).normalized;
       // var directionToCheck = ((Vector2)shootParticlePos.position-(Vector2)transform.position ).normalized;
        Ray2D rayToHit = new Ray2D(shootParticlePos.position, direction);
       // Ray2D rayToCheck = new Ray2D(transform.position, directionToCheck);
        RaycastHit2D hit;
       // RaycastHit2D hitToCheck;
        
        Debug.DrawRay(rayToHit.origin, rayToHit.direction, Color.red);
        //Debug.DrawRay(transform.position, rayToCheck.direction, Color.black);
        if (TryShoot(weapon)
            && (hit = Physics2D.Raycast(rayToHit.origin, rayToHit.direction, int.MaxValue, raycastToShoot))
            /*&& (hitToCheck = Physics2D.Raycast(rayToCheck.origin, rayToCheck.direction, int.MaxValue, raycastToCheck))*/
            )
        {
            PhotonView hittedPV;
            
            if(hit.collider.gameObject.TryGetComponent(out hittedPV)/*&&!hitToCheck*/)
            if (!hittedPV.IsMine)
            {
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(weapon.Data.damage);
                print("We hit " + hit.collider.gameObject.name);
                print("Shooting " + weapon.Data.name);
            }
           
        }

    }

    public void Dropped()
    {
        isDropped = true;
        
         triggerCollider.enabled = true;
         physicalCollider.enabled = true;
        _rb.simulated = true;
        if (currentAmmo == 0)
        {
            DisolveWeapon();
        }
    }
    public void Equiped()
    {
        StopCoroutine("DisolveWeaponCoroutine");
        isDropped = false;
        triggerCollider.enabled = false;
        physicalCollider.enabled = false;
        _rb.simulated = false;
        WeaponEquipped?.Invoke();
    }


    public void DisolveWeapon()
    {
        print("Disolving weapon: "+this.gameObject.name );
        StartCoroutine("DisolveWeaponCoroutine");
    }
    public void DestroyWeapon()
    {
        PhotonNetwork.Destroy(PV);
    }

    private IEnumerator DisolveWeaponCoroutine()
    {
        yield return new WaitForSeconds(2);
        print("2 Second past disolving start");
        PhotonNetwork.Destroy(PV);
        yield break;
    }
}


public interface IWeapon
{   public WeaponDataSO Data{ get; }
   
    public bool isDropped { get; }
    public void Dropped();
    public void Equiped();
    public void DisolveWeapon();
    public event Action WeaponEquipped;
    public event Action BulletShot;
    public int currentAmmo { get; }
    public void Shoot(IWeapon weapon);
    

}
