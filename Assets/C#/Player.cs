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

public class Player : MonoBehaviour
{
    
    
    //for action return only
    public List<Card> ReturnedCards;
    public List<int> ReturnedQuantities;
    
    //for action display only
    public List<Card> InputCards;
    
    private static Card ActionCard;

    private static Card BattleOpponent;
    
    private Starship Starship;
    public string Location;
    public Dictionary<string, float> AttributeMap = new Dictionary<string, float>();
    private Dictionary<ActionData, int> ActionRepetitionsMap;
    private static Player _State;

    private void Start()
    {
        ActionRepetitionsMap = new Dictionary<ActionData, int>();
        SetLocation("Glint");
        NullActionCard();;
        InputCards = new List<Card>();
        ReturnedCards = new List<Card>();
        ReturnedQuantities = new List<int>();

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
    public static Player State
    {
        get
        {
            if (_State == null)
            {
                _State = FindObjectOfType<Player>();
                if (_State == null)
                {
                    GameObject singleton = new GameObject("PlayerState");
                    _State = singleton.AddComponent<Player>();
                }
            }
            return _State;
        }
    }

    // Ensure that the instance is not destroyed between scenes
    private void Awake()
    {
        if (_State == null)
        {
            _State = this;
            DontDestroyOnLoad(gameObject);
            InitializeAttributeMap();
        }
        else if (_State != this)
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

    public Card GetActionCard()
    {
        return ActionCard;
    }

    public void SetActionCard(Card CurrentActionCard)
    {
        if (ActionCard != null)
        {
            Debug.Log("WARNING: ActionCard has not been nulled.");
        }
        
        ActionCard = CurrentActionCard;
    }

    public void NullActionCard()
    {
        Debug.Log("Nulling ActionCard: " + ActionCard.ID);

        ActionCard = null;
    }

    public string GetLocation()
    {
        return Location;
    }

    public void SetLocation(string NewLocation)
    {
        Location = NewLocation;
    }

    public Card GetBattleOpponent()
    {
        return BattleOpponent;
    }

    public void SetBattleOpponent(Card NewBattleOpponent)
    {
        BattleOpponent = NewBattleOpponent;
    }

    public Starship GetStarship()
    {
        return Starship;
    }
}