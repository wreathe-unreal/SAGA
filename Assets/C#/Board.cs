using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;  // Needed for UI elements like Panels


public class Board : MonoBehaviour
{
    private Starship Starship;
    static public Board State;
    public TMP_Text HealthText;
    public Image HealthBar;
    static public CardDB Database;
    static public Dictionary<string, Deck> Decks;
    public Card CardToAdd;

    public static Board GetInstance()
    {
        return State;
    }

    public static void DestroyCard(Card CardToDestroy)
    {
        Deck SearchDeck = Decks[CardToDestroy.DeckType];


        for (int i = SearchDeck.Cards.Count - 1; i >= 0; i--)
        {
            if (SearchDeck.Cards[i].ID == CardToDestroy.ID)
            {
                SearchDeck.Cards[i].ModifyQuantity(-1);

                if (SearchDeck.Cards[i].Quantity == 0)
                {
                    Card cardToBeDestroyed = SearchDeck.Cards[i];

                    SearchDeck.Cards.RemoveAt(i);

                    // now it's safe to destroy the card object
                    Destroy(cardToBeDestroyed.gameObject);

                    SearchDeck.SetCardPositions();
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics.queriesHitTriggers = true;
        State = gameObject.GetComponent<Board>();
        Starship = gameObject.GetComponent<Starship>();
        Database = new CardDB();
        InitializeDecks();
        AddStartingCards();

    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.fillAmount = GetStarship().CurrentHealth / GetStarship().GetMaxHealth();
        HealthText.text = $"{GetStarship().CurrentHealth} / {GetStarship().GetMaxHealth()}";
        GetStarship().UpdateFleetCard();

        if (Board.Decks["Fleet"].Cards.Count > 0 && ActionGUI.AllPanelsAreClosed()) 
        {
            Board.Decks["Fleet"].Cards[0].SetFaceUpState(true);
        }

        
    }

    public Card AddCard(string cardID, int quantity, bool bSetCardPositionsAfterAdding)
    {
        CardData cd = CardDB.CardDataLookup[cardID];
        string deckType = cd.DeckType;
        Card foundCard = null;

        //handle starship comopnents
        if (deckType == "Starship")
        {
            Card newCard = Instantiate<Card>(CardToAdd);
            newCard.Initialize(cardID);
            GetStarship().AutoEquip(newCard);
            // if ()
            // {
            //     Decks["Starship"].SetCardPositions();
            // }
            // else
            // {
            //     Decks["Objects"].SetCardPositions();
            // }
            return newCard;
        }
        
        bool bExistsInDeckAlready = false;

        foreach (Card c in Decks[deckType].Cards)
        {
            if (c.ID == cd.ID)
            {
                if (c.DeckType != "Action")
                {
                    c.ModifyQuantity(quantity);
                }
                bExistsInDeckAlready = true;
                foundCard = c;
            }
        }

        if (!bExistsInDeckAlready)
        {
            Card newCard = Instantiate<Card>(CardToAdd);
            newCard.Initialize(cardID);
            Decks[deckType].Cards.Add(newCard);
            if (bSetCardPositionsAfterAdding)
            {
                Decks[deckType].SetCardPositions();
            }

            return newCard;
        }
        else
        {
            if (bSetCardPositionsAfterAdding)
            {
                Decks[deckType].SetCardPositions();
            }

            return foundCard;
        }
    }

    private static void InitializeDecks()
    {
        Decks = new Dictionary<string, Deck>();
        Decks[""] = new Deck("");
        Decks["Starship"] = new Deck("Starship");
        Decks["Crafting"] = new Deck("Crafting");
        Decks["Object"] = new Deck("Object");
        Decks["Character"] = new Deck("Character");
        Decks["Fleet"] = new Deck("Fleet");
        Decks["Cargo"] = new Deck("Cargo");
        Decks["Habitat"] = new Deck("Habitat");
        Decks["System"] = new Deck("System");
        Decks["Action"] = new Deck("Action");
        Decks["Ambition"] = new Deck("Ambition");
        Decks["Currency"] = new Deck("Currency");
        Decks["Quest"] = new Deck("Quest");
        Decks["Enemy"] = new Deck("Enemy");

        for (int i = 0; i < 8; i++)
        {
            Decks["Starship"].Cards.Add(null);
        }
    }

    private void AddStartingCards()
    {
        List<string> InitialCards = new List<string> { "work", "glint", "toil_in_the_depths", "deepmine", "gold", "dream"};

        foreach (string s in InitialCards)
        {
            Card c = AddCard(s, 1, true);
            c.SetFaceUpState(true);
        }
    }
    
    

    public void ResetCardPositionAndList(List<Card> Cards)
    {
        foreach (Card c in Cards)
        {
            c.transform.SetParent(null);
            c.transform.localScale = new Vector3(1, 1, 1);
        }

        foreach (Deck d in Board.Decks.Values)
        {
            d.SetCardPositions();
        }

        Cards.Clear();
    }

    

    public Starship GetStarship()
    {
        return Starship;
    }
}

public class Deck : IEnumerable<Card>
{
    public string Name;
    public List<Vector3> Positions = new List<Vector3>();
    public List<Card> Cards = new List<Card>();



    // Start is called before the first frame update
    public Deck(string deckName)
    {
        this.Name = deckName;
        GameObject CardPositions = GameObject.Find("CardPositions");
        
        if (CardPositions != null)
        {
            Positions = CollectPositions(CardPositions, this.Name);
        }
    }
    
    List<Vector3> CollectPositions(GameObject parentObject, string groupName)
    {
        Transform groupTransform = parentObject.transform.Find(groupName);
        List<Vector3> positions = new List<Vector3>();

        if (groupTransform != null)
        {
            foreach (Transform child in groupTransform)
            {
                positions.Add(child.position);
                if (groupName == "Fleet")
                {
                    Debug.Log(child.transform.name);
                }
            }
        }
        return positions;
    }

    public void SetCardPositions() 
    {
        for (int i = 0; i < Positions.Count; i++)
        {
            if (i < Cards.Count && Cards[i] != null && Cards[i].Position != Positions[i])
            {
                    // Assign position to card
                Cards[i].SetPosition(Positions[i]);
            }
        }
    }
    
    // Implementation of IEnumerable<Card>
    public IEnumerator<Card> GetEnumerator()
    {
        return Cards.Where(card => card != null).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

