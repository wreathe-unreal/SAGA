using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute
{
    public List<string> EProperty { get; set; }
    public List<string> EType { get; set; }
    public List<string> ESystem { get; set; }
    public List<string> EHabitat { get; set; }
    public List<string> EAttribute { get; set; }
}

public class PlayerState : MonoBehaviour
{
    public Starship Starship;
    public string Location;
    public Dictionary<string, float> AttributeMap = new Dictionary<string, float>();
    private Dictionary<ActionData, int> ActionRepetitionsMap;
    private static PlayerState _Instance;

    private void Start()
    {
        ActionRepetitionsMap = new Dictionary<ActionData, int>();

    }

    public int GetActionRepetition(ActionData actionData)
    {
        if (!ActionRepetitionsMap.ContainsKey(actionData))
        {
            ActionRepetitionsMap[actionData] = 0;
        }

        return ActionRepetitionsMap[actionData];

    }

    public void IncrementActionRepetition(ActionData actionData)
    {
        if (!ActionRepetitionsMap.ContainsKey(actionData))
        {
            ActionRepetitionsMap[actionData] = 1;
        }

        ActionRepetitionsMap[actionData]++;
    }
    
    public void DecrementActionRepetition(ActionData actionData)
    {

        ActionRepetitionsMap[actionData]--;
    }
    
    // Property to access the instance
    public static PlayerState Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<PlayerState>();
                if (_Instance == null)
                {
                    GameObject singleton = new GameObject("PlayerState");
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
            InitializeAttributeMap();
        }
        else if (_Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAttributeMap()
    {
        foreach (string s in JsonDeserializer.DeserializeEnumJson().EAttribute)
        {
            AttributeMap[s] = 0.0f;
        }

        AttributeMap[""] = 0.0f;
    }

    // Example function that can be called on this singleton
    public PlayerState GetPlayerState()
    {
        return this;
    }

    // Other methods and properties to manage player state can be added here
}