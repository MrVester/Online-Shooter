using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyList<T> : MonoBehaviour
{
    [SerializeField] private List<T> items;
    private List<T> sortedItems;

 /*   public K GetItemByName<K>(string name)
    {
        int index = sortedItems.FindIndex(go => gameObject.name == name);
        if (index < 0)
            return null;
        return sortedItems[index];
    }*/

    private void Sort()
    {
        sortedItems = items.Distinct().ToList();
      
    }
}
