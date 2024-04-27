using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    
    public ActionData FindAction(string[] words)
    {
        SetInputCards(words);

        if (InputCards.Count <= 0 || words.Length <= 1)
        {
            return null;
        }
         
        List<CardSpecifier> cardSpecifiers = new List<CardSpecifier>();
        for (int i = 1; i < InputCards.Count; i++)
        {
            CardData cd = CardDB.CardDataLookup[InputCards[i].ID];
            CardSpecifier cs = new CardSpecifier(cd.ID, cd.Type, cd.Property);
            cardSpecifiers.Add(cs);
        }
             
        ActionKey actionKeyToFind = new ActionKey(ActionCard.Name, InputCards[0].ID, Location, cardSpecifiers);
             
        CardData mainCardData = CardDB.CardDataLookup[InputCards[0].ID];

        List<CardData> cdList = new List<CardData>();
        foreach (Card c in InputCards)
        {
            cdList.Add(CardDB.CardDataLookup[c.ID]);
        }

        List<ActionData> MatchHint = mainCardData.FindActionData(ActionCard.Name, cdList);

        if (MatchHint[0] != null)
        {
            GetActionCard().CurrentActionData = MatchHint[0];
            return MatchHint[0];
        }
         
        if(MatchHint[1] != null)
        {
            GetActionCard().CurrentActionHint = MatchHint[1];
            return MatchHint[1];
        }

        return null;
    }
        
    public void ExecuteAction()
    {
        print(ActionCard.Name);
        print(ActionCard.CurrentActionData.ActionResult.Title);
        AttributeMap[ActionCard.CurrentActionData.ActionResult.AttributeModified] += ActionCard.CurrentActionData.ActionResult.AttributeModifier;
        
        IncrementActionRepetition(ActionCard.Name, InputCards[0].Data);

        ActionCard.CookActionResult();
        ActionCard.transform.SetParent(null);
        NullActionCard();

        List<Card> cardsToRemove = new List<Card>();
        
        foreach (Card c in InputCards)
        {
            cardsToRemove.Add(c);
        }

        foreach (Card c in cardsToRemove)
        {
            Board.DestroyCard(c);
            InputCards.Remove(c);
        }
        
        InputCards = new List<Card>(); // Clear other cards
    
    }

    public void SetInputCards(string[] words)
    {
        InputCards = new List<Card>();

        if (ActionCard == null || words.Length < 2)
        {
            return;
        }
        
        // Check all other words
        for (int i = 1; i < words.Length; i++)
        {
            string currentWord = words[i];
            bool matchFound = false;

            foreach (KeyValuePair<string, Deck> kvp in Board.Decks)
            {
                // Skip the Action deck for these checks
                if (kvp.Key == "Action")
                {
                    continue;
                }
                    
                Card card = kvp.Value.Cards
                    .Where(c => c != null && c.Name.ToLower() == currentWord).FirstOrDefault();
                    
                if (card != null)
                {
                    matchFound = true;
                    InputCards.Add(card);
                    break;
                }
            }

            if (!matchFound)
            {
                InputCards = new List<Card>();
                break;
            }
        }
    }

    public List<Card> GetInputCards()
    {
        return InputCards;
    }

    public List<Card> GetReturnedCards()
    {
        return ReturnedCards;
    }

    public List<int> GetReturnedQtys()
    {
        return ReturnedQuantities;
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
}