using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;  // Needed for UI elements like Panels


public class Board : MonoBehaviour
{
    static public Board State;
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
        Database = new CardDB();
        InitializeDecks();
        AddStartingCards();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Card AddCard(string cardID, int quantity, bool bSetCardPositionsAfterAdding)
    {
        CardData cd = CardDB.CardDataLookup[cardID];
        string deckType = cd.DeckType;
        Card foundCard = null;

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
            }
        }
        return positions;
    }

    public void SetCardPositions() 
    {
        for (int i = 0; i < Positions.Count; i++)
        {
            if (i < Cards.Count && Cards[i].Position != Positions[i])
            {
                // Assign position to card
                Cards[i].SetPosition(Positions[i]);
            }
        }
    }
    
    // Implementation of IEnumerable<Card>
    public IEnumerator<Card> GetEnumerator()
    {
        return Cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}

