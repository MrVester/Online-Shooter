using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponList : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    private List<Weapon> sortedWeapons;

    private void Awake()
    {
        Sort();
    }
    public Weapon GetWeaponByName(string name)
    {
        int index= sortedWeapons.FindIndex(go => go.Data.name == name);
        if (index < 0)
            return null;
        return sortedWeapons[index];
    }
        
    public List<Weapon> GetList()
    {
        return sortedWeapons;
    }
    public int WeaponCount()
    {
        return sortedWeapons.Count;
    }
    
    public string RandomWeaponName()
    {
        int weaponCount = WeaponCount();
        int randWeapon = Random.Range(0, weaponCount);
        return sortedWeapons[randWeapon].Data.name;
    }
    private void Sort()
    {
        sortedWeapons = weapons.Distinct().OrderBy(x => x.Data.name).ToList();
        //sortedWeapons = weapons.OrderBy(x => x.ID)
    }

}
