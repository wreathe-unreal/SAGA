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
    private Dictionary<string, int> ActionRepetitionsMap;
    private static Player _State;

    private void Start()
    {
        ActionRepetitionsMap = new Dictionary<string, int>();
        SetLocation("Deepmine");
        NullActionCard();;
        InputCards = new List<Card>();
        ReturnedCards = new List<Card>();
        ReturnedQuantities = new List<int>();

    }
    
        
    public void ExecuteAction()
    {
        

        print("Action Card Is Null?" + (Player.State.GetActionCard() == null).ToString());
        
        print(ActionCard.Name);
        AttributeMap[ActionCard.CurrentActionData.ActionResult.AttributeModified] += ActionCard.CurrentActionData.ActionResult.AttributeModifier;
        
        print(ActionCard.CurrentActionData.ActionResult.Title);
        
        IncrementActionRepetition(ActionCard.Name, InputCards[0].Data);

        ActionCard.CookActionResult();
        ActionCard.transform.SetParent(null);
        NullActionCard();

        ActionGUI.SetPanelActive(false);

        List<Card> cardsToRemove = new List<Card>();
        
        foreach (Card c in InputCards)
        {
            cardsToRemove.Add(c);
        }

        foreach (Card c in cardsToRemove)
        {
            BoardState.DestroyCard(c);
            InputCards.Remove(c);
        }
        
        InputCards = new List<Card>(); // Clear other cards
    
    }

    

    public int GetActionRepetition(string ActionName, CardData FirstCard)
    {
        string Key = ActionName + FirstCard.ID;
        if (!ActionRepetitionsMap.ContainsKey(Key))
        {
            ActionRepetitionsMap[Key] = 0;
        }

        return ActionRepetitionsMap[Key];

    }

    public void IncrementActionRepetition(string ActionName, CardData FirstCard)
    {
        string Key = ActionName + FirstCard.ID;
        if (!ActionRepetitionsMap.ContainsKey(Key))
        {
            ActionRepetitionsMap[Key] = 1;
        }

        ActionRepetitionsMap[Key]++;
    }
    
    public void DecrementActionRepetition(string ActionName, CardData FirstCard)
    {
        string Key = ActionName + FirstCard.ID;
        ActionRepetitionsMap[Key]--;
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
        Debug.Log("Setting ActionCard To: " + CurrentActionCard.Name);

        
        ActionCard = CurrentActionCard;
    }

    public void NullActionCard()
    {
        if (GetActionCard() != null)
        {
            Debug.Log("Nulling ActionCard: " + ActionCard.ID);
        }

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