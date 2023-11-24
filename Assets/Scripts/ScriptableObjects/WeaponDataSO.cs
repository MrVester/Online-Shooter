using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWeapon", menuName = "Data/WeaponData", order = 0)]
public class WeaponDataSO : ScriptableObject
{
    public Sprite weaponSprite;
    public new string name;
    public int ammo;
    public int bulletsPerShot;
    public int spreadAngle;
    public float bulletSpeed;
    public float bulletRange;
    public float bulletSize;

}
