using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager Instance { get; private set; }
    [SerializeField] Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        foreach (var menu in menus)
        {
            menu.Close();
            menu.isOpened = false;
        }
    }
    public void OpenMenu(string menuName)
    {
        foreach (var menu in menus)
        {
            if (menu.menuName == menuName)
            {
               menu.Open();
            }
            else if (menu.isOpened)
            {
                CloseMenu(menu);
            }
        }
    }
    public void OpenMenu(Menu menuObj)
    {
        foreach (var menu in menus)
        {
            if (menu.isOpened)
         
            {
                CloseMenu(menu);
            }
        }
        menuObj.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
}
