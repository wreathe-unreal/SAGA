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

    public string System;
    public string Habitat;
    public string Location;
    public Dictionary<string, float> AttributeMap = new Dictionary<string, float>();
    private Dictionary<string, int> ActionRepetitionsMap;
    private static Player _State;

    private void Start()
    {
        ActionRepetitionsMap = new Dictionary<string, int>();
        NullActionCard();
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
        AttributeMap[ActionCard.CurrentActionData.ActionResult.AttributeModified] += ActionCard.CurrentActionData.ActionResult.AttributeModifier;
        
        IncrementActionRepetition(ActionCard.Name, InputCards[0].Data);

        ActionCard.CookActionResult();

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
        Player.State.NullActionCard();
    
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
        ActionCard = CurrentActionCard;
    }

    public void NullActionCard()
    {

        ActionCard = null;
    }

    public string GetLocation()
    {
        return Location;
    }

    public Card GetBattleOpponent()
    {
        return BattleOpponent;
    }

    public void SetBattleOpponent(Card NewBattleOpponent)
    {
        BattleOpponent = NewBattleOpponent;
    }

    public void UpdatePlayerHabitat()
    {
        if (IsPlayerInHabitat())
        {
            Habitat = Location;
        }
        else
        {
            Habitat = "";

        }

        foreach (Deck d in Board.Decks.Values)
        {
            d.SetCardPositions();
            
            if (d.Name == "Habitat")
            {
                foreach (Card c in d)
                {
                    if (c.Name == Habitat)
                    {
                        c.PieTimer.fillAmount = 100;
                    }
                    else
                    {
                        c.PieTimer.fillAmount = 0;
                    }
                }
            }
        }
    }

    public bool IsPlayerInHabitat()
    {
        List<string> Systems = new List<string> { "Avalon", "Glint", "Merlin", "Nocturne", "Bane", "Macbeth IV" };

        foreach (string s in Systems)
        {
            if (Player.State.Location == s)
            {
                return false;
            }
        }
        return true;
    }

    public string UpdatePlayerSystem()
    {
        string newSystem = "";
        
        switch (Location)
        {
            case "Avalon":
            case "Roundtable Nexus":
            case "Terminus Est":
            case "The Citadel":
            case "The White Lodge":
            case "Dragonroost":
            case "Yggdrasil":
            case "Leviathan Belt":
            case "Earth":
                newSystem = "Avalon";
                break;
            case "Glint":
            case "Boulderhearth":
            case "Forge":
            case "Behemoth":
            case "The Sledge":
            case "Brewhalla":
            case "Deepmine":
            case "Jormungandr":
            case "Ben Nevis":
                newSystem = "Glint";
                break;
            case "Merlin":
            case "Jenasysz":
            case "Nostalgia V":
            case "Undine":
            case "Reverie":
            case "Lore":
            case "Longbow":
            case "Myst":
            case "Veil Lookouts":
                newSystem = "Merlin";
                break;
            case "Nocturne":
            case "Dracule":
            case "Crimsontide":
            case "One-eyed Jacks":
            case "Wreckage Bay":
            case "Smuggler's Nook":
            case "Rakshasa":
            case "Black Sun Campaign":
            case "The Feast":
                newSystem = "Nocturne";
                break;
            case "Bane":
            case "Bameth":
            case "Obsidian Conclave":
            case "Voltage":
            case "Darkwash":
            case "Golem":
            case "Blitzkrieg":
            case "Zweijager":
                newSystem = "Bane";
                break;
            case "Macbeth IV":
            case "Mausoleum":
            case "Umbressa":
            case "Sakura":
            case "Exvulsa":
            case "Triangula":
            case "Blame":
            case "Abyss Abbey":
            case "The Black Mass":
                newSystem = "Macbeth IV";
                break;

        }

        System = newSystem;

        foreach (Deck d in Board.Decks.Values)
        {
            if (d.Name == "System")
            {
                foreach (Card c in d)
                {
                    if (c.Name == System)
                    {
                        c.PieTimer.fillAmount = 100;
                    }
                    else
                    {
                        c.PieTimer.fillAmount = 0;
                    }
                }
            }
        }

        return newSystem;
    }
    
    public List<string> GetSystemHabitats(string System)
    {
        List<string> Habitats = new List<string>();
        switch (System)
        {
            case "Avalon":
                Habitats.Add("Roundtable Nexus");
                Habitats.Add("Terminus Est");
                Habitats.Add("The Citadel");
                Habitats.Add("The White Lodge");
                Habitats.Add("Dragonroost");
                Habitats.Add("Yggdrasil");
                Habitats.Add("Leviathan Belt");
                Habitats.Add("Earth");
                break;
            case "Glint":
                Habitats.Add("Boulderhearth");
                Habitats.Add("Forge");
                Habitats.Add("Behemoth");
                Habitats.Add("The Sledge");
                Habitats.Add("Brewhalla");
                Habitats.Add("Deepmine");
                Habitats.Add("Jormungandr");
                Habitats.Add("Ben Nevis");
                break;
            case "Merlin":
                Habitats.Add("Jenasysz");
                Habitats.Add("Nostalgia V");
                Habitats.Add("Undine");
                Habitats.Add("Reverie");
                Habitats.Add("Lore");
                Habitats.Add("Longbow");
                Habitats.Add("Myst");
                Habitats.Add("Veil Lookouts");
                break;
            case "Nocturne":
                Habitats.Add("Dracule");
                Habitats.Add("Crimsontide");
                Habitats.Add("One-eyed Jacks");
                Habitats.Add("Wreckage Bay");
                Habitats.Add("Smuggler's Nook");
                Habitats.Add("Rakshasa");
                Habitats.Add("Black Sun Campaign");
                Habitats.Add("The Feast");
                break;
            case "Bane":
                Habitats.Add("Bameth");
                Habitats.Add("Obsidian Conclave");
                Habitats.Add("Voltage");
                Habitats.Add("Darkwash");
                Habitats.Add("Golem");
                Habitats.Add("Blitzkrieg");
                Habitats.Add("Zweijager");
                break;
            case "Macbeth IV":
                Habitats.Add("Mausoleum");
                Habitats.Add("Umbressa");
                Habitats.Add("Sakura");
                Habitats.Add("Exvulsa");
                Habitats.Add("Triangula");
                Habitats.Add("Blame");
                Habitats.Add("Abyss Abbey");
                Habitats.Add("The Black Mass");
                break;
        }

        return Habitats;
    }

    public void HandleTravel(ActionData currentActionData)
    {
        string newLocation;
        if (currentActionData.ActionResult.ReturnedCardIDs[0] != "travel")
        {
            newLocation = currentActionData.ActionResult.ReturnedCardIDs[0];

        }
        else
        {
            newLocation = currentActionData.ActionResult.ReturnedCardIDs[1];
        }


        SetLocation(newLocation);
        
        foreach (Deck d in Board.Decks.Values)
        {
            if (d.Name == "Habitat")
            {
                d.SetCardPositions();
            }
        }
    }

    public void SetLocation(string newLocation)
    {
        Location = newLocation;
        UpdatePlayerSystem();
        UpdatePlayerHabitat();


    }
}