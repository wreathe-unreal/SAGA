using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Board : MonoBehaviour
{
    public CardDB Database;
    public Dictionary<string, Deck> Decks;
    public Card CardToAdd;
    
    // Start is called before the first frame update
    void Start()
    {
        Database = new CardDB();
        InitializeDecks();
        AddInitialCards();







        //game loop

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddCard(string cardID)
    {
        CardData cd = CardDB.CardDataLookup[cardID];
        string deckType = cd.GetDeckType();
        Card newCard = Instantiate<Card>(CardToAdd);
        newCard.Initialize(cardID);
        Decks[deckType].Cards.Add(newCard);
        Decks[deckType].SetCardPositions();
    }

    private void AddInitialCards()
    {
        AddCard("work");
        AddCard("ironmanpepes_broom");
        AddCard("korean_book");

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
}
