using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;  // Needed for UI elements like Panels


public class Board : MonoBehaviour
{
    static public Board board;
    static public CardDB Database;
    static public Dictionary<string, Deck> Decks;
    public Card CardToAdd;
    public static List<Card> ReturnedCards;

    public static Board GetInstance()
    {
        return board;
    }

    public void DestroyCard(Card CardToDestroy)
    {
        Deck SearchDeck = Decks[CardToDestroy.GetDeckType()];

        
        for(int i = SearchDeck.Cards.Count - 1; i >= 0; i--)
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
        board = gameObject.GetComponent<Board>();
        Database = new CardDB();
        ReturnedCards = new List<Card>();
        InitializeDecks();
        AddStartingCards();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Card AddCard(string cardID, bool bSetCardPositionsAfterAdding)
    {
        CardData cd = CardDB.CardDataLookup[cardID];
        string deckType = cd.GetDeckType();
        Card foundCard = null;
        
        bool bExistsInDeckAlready = false;
        
        foreach (Card c in Decks[deckType].Cards)
        {
            if (c.ID == cd.ID)
            {
                c.ModifyQuantity(1);
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

    private void InitializeDecks()
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
    }

    private void AddStartingCards()
    {
        List<string> InitialCards = new List<string> { "work", "glint", "deep_miner_mining", "deepmine" };

        foreach (string s in InitialCards)
        {
            AddCard(s, true);
        }
    }

    public static void ReturnAndClean()
    {
        foreach (Card c in ReturnedCards)
        {
            c.transform.SetParent(null);
            c.transform.localScale = new Vector3(55f, 55f, 1f);

        }

        foreach (Deck d in Decks.Values)
        {
            d.SetCardPositions();
        }

        ReturnedCards = new List<Card>();
        Terminal.SetPanelActive(false);
    }

}
