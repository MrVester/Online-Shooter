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
    }
    public Weapon GetWeaponByName(string name)
    {
        int index= sortedWeapons.FindIndex(go => go.Data.name == name);
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
    
    private void Sort()
    {
        sortedWeapons = weapons.Distinct().OrderBy(x => x.Data.name).ToList();
        //sortedWeapons = weapons.OrderBy(x => x.ID)
    }

}
