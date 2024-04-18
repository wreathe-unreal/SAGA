using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyDetector
{
    public KeyCode currentKey;
    private bool keyPressed;

    void Update()
    {
        keyPressed = false; // Reset the key pressed flag each frame

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                currentKey = key;
                keyPressed = true; // Set the flag to true when any key is pressed
                Debug.Log("Current key held down: " + currentKey);
                break; // Exit the loop once a key is found pressed
            }
        }

        if (!keyPressed)
        {
            currentKey = KeyCode.None; // Set currentKey to None if no keys were pressed
            Debug.Log("No key is currently being pressed.");
        }
    }
}

public class Selector : MonoBehaviour
{
    public KeyDetector InputController = new KeyDetector();
    public int Index = 0;
    private int DeckColumns;
    private int DeckRows;
    private Deck CurrentDeck;
    public TMP_InputField InputField;
    private bool bSelecting;

    void Start()
    {
        if (BoardState.GetInstance() == null)
        {
            Debug.LogError("BoardRef not set in the inspector");
            return; // Add return here
        }
        if (BoardState.Decks == null || BoardState.Decks.Count == 0)
        {
            Debug.LogError("BoardRef.Decks is not initialized or empty");
            return; // Add return here
        }
        CurrentDeck = BoardState.Decks["Object"];
    }
    void Awake()
    {

    }

    void Update()
    {
            
        // HandleInput(InputController.currentKey);
        // if (transform.childCount > 0)
        // {
        //     Transform childTransform = transform.GetChild(0); // Get first child
        //     childTransform.position = Board.Decks[CurrentDeck.Name].Positions[Index];
        // }


    }

    public void HandleInput(KeyCode key)
    {
        //print(Index);
        if (key == KeyCode.None)
        {
            return;
        }

        switch (key)
        {
            case (KeyCode.W):
                if (Index - GetDeckRows(CurrentDeck.Name) >= 0 && Index % GetDeckRows(CurrentDeck.Name) > 1.0f)
                {
                    Index -= GetDeckRows(CurrentDeck.Name);
                }
                else
                {
                    CurrentDeck = BoardState.Decks[HandleDeckTransfer(key)];
                    Index = 0;
                }

                break;
            case (KeyCode.A):
                if (Index > 0)
                {
                    Index--;
                }
                else
                {
                    CurrentDeck = BoardState.Decks[HandleDeckTransfer(key)];
                    Index = 0;
                }

                break;
            case (KeyCode.S):
                if (Index + GetDeckRows(CurrentDeck.Name) <= CurrentDeck.Cards.Count)
                {
                    Index += GetDeckRows(CurrentDeck.Name);
                }
                else
                {
                    CurrentDeck = BoardState.Decks[HandleDeckTransfer(key)];
                    Index = 0;
                }

                break;
            case (KeyCode.D):
                if (Index < CurrentDeck.Cards.Count)
                {
                    Index++;
                }
                else
                {
                    CurrentDeck = BoardState.Decks[HandleDeckTransfer(key)];
                    Index = 0;
                }

                break;
        }
    }


    public string HandleDeckTransfer(UnityEngine.KeyCode key)
    {
        switch (CurrentDeck.Name)
        {
            case "Cargo":
                switch(key)
                {
                    case KeyCode.W:
                        return "Object";
                    case KeyCode.A:
                        return "Action";
                    case KeyCode.S:
                        return "Currency";
                    case KeyCode.D:
                        return "Action";
                        
                }
                break;
            case "Crafting":
                switch(key)
                {
                    case KeyCode.W:
                        return "Action";
                    case KeyCode.A:
                        return "Currency";
                    case KeyCode.S:
                        return "Starship";
                    case KeyCode.D:
                        return "System";
                }
                break;
            case "Object":
                switch(key)
                {
                    case KeyCode.W:
                        return "Currency";
                    case KeyCode.A:
                        return "Character";
                    case KeyCode.S:
                        return "Cargo";
                    case KeyCode.D:
                        return "Ambition";
                }
                break;
            case "Character":
                switch(key)
                {
                    case KeyCode.W:
                        return "System";
                    case KeyCode.A:
                        return "Fleet";
                    case KeyCode.S:
                        return "Action";
                    case KeyCode.D:
                        return "Object";
                }
                break;
            case "Fleet":
                switch(key)
                {
                    case KeyCode.W:
                        return "Habitat";
                    case KeyCode.A:
                        return "Ambition";
                    case KeyCode.S:
                        return "Action";
                    case KeyCode.D:
                        return "Character";
                }
                break;
            case "Starship": 
                switch(key)
                {
                    case KeyCode.W:
                        return "Crafting";
                    case KeyCode.A:
                        return "Object";
                    case KeyCode.S:
                        return "Ambition";
                    case KeyCode.D:
                        return "Habitat";
                }
                break;
            case "Habitat":
                switch(key)
                {
                    case KeyCode.W:
                        return "Crafting";
                    case KeyCode.A:
                        return "Starship";
                    case KeyCode.S:
                        return "Fleet";
                    case KeyCode.D:
                        return "System";
                }
                break;
            case "System":
                switch(key)
                {
                    case KeyCode.W:
                        return "Action";
                    case KeyCode.A:
                        return "Habitat";
                    case KeyCode.S:
                        return "Character";
                    case KeyCode.D:
                        return "Object";
                }
                break;
            case "Action":
                switch(key)
                {
                    case KeyCode.W:
                        return "Fleet";
                    case KeyCode.A:
                        return "Cargo";
                    case KeyCode.S:
                        return "Crafting";
                    case KeyCode.D:
                        return "Cargo";
                }
                break;
            case "Ambition":
                switch(key)
                {
                    case KeyCode.W:
                        return "Starship";
                    case KeyCode.A:
                        return "Object";
                    case KeyCode.S:
                        return "Action";
                    case KeyCode.D:
                        return "Fleet";
                }
                break;
            case "Currency":
                switch(key)
                {
                    case KeyCode.W:
                        return "Cargo";
                    case KeyCode.A:
                        return "System";
                    case KeyCode.S:
                        return "Object";
                    case KeyCode.D:
                        return "Crafting";
            }
            break;
        }

        return "Object";
    }

    public int GetDeckColumns(string DeckName)
    {
        switch (DeckName)
        {
            case "Starship":
                if (Index < 3 || Index >= 7)
                {
                    return 2;
                }
                else if (Index >= 3 && Index < 7)
                {
                    return 4;
                }

                break;
            case "Crafting":
                return 8;
            case "Object":
                return 5;
            case "Character":
                return 3;
            case "Fleet":
                return 3;
            case "Cargo":
                return 7;
            case "Habitat":
                return 4;
            case "System":
                return 2;
            case "Action":
                return 10;
            case "Ambition":
                return 5;
            case "Currency":
                return 6;
        }

        return -1;
    }
    
    public int GetDeckRows(string DeckName)
    {
        switch (DeckName)
        {
            case "Starship":
                return 3;
            case "Crafting":
                return 1;
            case "Object":
                return 4;
            case "Character":
                return 2;
            case "Fleet":
                return 2;
            case "Cargo":
                return 1;
            case "Habitat":
                return 1;
            case "System":
                return 3;
            case "Action":
                return 1;
            case "Ambition":
                return 1;
            case "Currency":
                return 1;
        }

        return -1;
    }
}
