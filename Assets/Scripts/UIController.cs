using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIController : MonoBehaviour
{
    private HealthBar hpBar;
    private TMP_Text ammoText;
    public event Action UISet;
    private bool boolUISet=false;

    private void Update()
    {
        if (!boolUISet)
        {
            hpBar = FindObjectOfType<HealthBar>();
            ammoText =FindObjectOfType<TMP_Text>();
            if (hpBar != null && ammoText != null)
            {
                boolUISet = true;
                UISet?.Invoke();
            }

        }
    }
    public void SetNoWeaponUI()
    {
        ammoText.text = "No weapon";
    }

    public void SetHealthUI(float hp)
    {
        hpBar.SetMaxHealth(hp);
    }
    public void SetAmmoUI( int ammo = 0)
    {
        ammoText.text = $"Ammo: {ammo}";
    }
    public void RefreshAmmoUI(int ammo=0)
    {
        ammoText.text = $"Ammo: {ammo}";
    }
    public void RefreshHealthUI(float hp)
    {
        hpBar.SetHealth(hp);
    }
}
