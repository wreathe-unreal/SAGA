using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attributes
{
    public int Valor;
    public int Intrepidity;
    public int Infamy;
    public int Prestige;
    public int Credit;
}


public class PlayerState : MonoBehaviour
{
    public string Location;
    public Attributes Attribute;
    private static PlayerState _Instance;

    // Property to access the instance
    public static PlayerState Instance
    {
        get
        {
            if (_Instance == null)
            {
                // This will only happen the first time this reference is used.
                _Instance = FindObjectOfType<PlayerState>();
                if (_Instance == null)
                {
                    // Create a new GameObject with PlayerState if one does not exist.
                    GameObject singleton = new GameObject(typeof(PlayerState).Name);
                    _Instance = singleton.AddComponent<PlayerState>();
                }
            }
            return _Instance;
        }
    }

    // Ensure that the instance is not destroyed between scenes
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_Instance != this)
        {
            // Destroy any duplicate instances that might be created
            Destroy(gameObject);
        }
    }

    // Example function that can be called on this singleton
    public PlayerState GetPlayerState()
    {
        return this;
    }

    // Other methods and properties to manage player state can be added here
}