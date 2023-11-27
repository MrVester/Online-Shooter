using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using UnityEngine.InputSystem;

public class HashController : MonoBehaviour
{
    private Hashtable hash = new Hashtable();
    // Start is called before the first frame update
    public Hashtable GetHash()
    {
        return hash;
    }
        
    public void Add<T>(string key, T value)
    {
        hash.TryAdd(key, value);
    }
    
    
    public void Remove(string key)
    {
        hash.Remove(key);
    }

    public bool ContainsKey(string key)
    {
        return hash.ContainsKey(key);
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
