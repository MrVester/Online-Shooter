using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponList : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private List<Weapon> sortedWeapons;

    private void Awake()
    {
        Sort();
        /*foreach (var weapon in weapons)
        {
            sortedWeapons[weapon.ID]= weapon;
        }
        print("COUNT SORTED: " + sortedWeapons.Count);*/
    }
    public Weapon GetWeaponByID(int ID)
    {
        return sortedWeapons[ID];
    }

    public int WeaponCount()
    {
        return sortedWeapons.Count;
    }
    
    private void Sort()
    {
        sortedWeapons =weapons.Distinct().OrderBy(x => x.ID).ToList();
        //sortedWeapons = weapons.OrderBy(x => x.ID)
    }

}
