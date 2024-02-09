using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "Data/WeaponData", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    public Sprite sprite;
    public new string name;
    public int ammo;
    public float damage;
    public int bulletsPerShot;
    public int spreadAngle;
    public float shootingSpeed;
    public float bulletSpeed;
    public float bulletRange;
    public float bulletSize;

}
