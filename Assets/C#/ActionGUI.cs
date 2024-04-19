using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class ActionGUI : MonoBehaviour
{
    public static TMP_InputField TextInput;
    
    
    public static ActionData CurrentAction;

    
    //for action return only
    public static List<Card> ReturnedCards;
    public static List<int> ReturnedQuantities;
    
    //for action display only
    public static List<Card> InputCards;
    public static Card ActionRef;
    
    //instance
    public static ActionGUI This; // Singleton instance

    public static MeshRenderer MeshRenderer;
    public static TMP_Text FlavorText;
    private static BoardState BoardState;
    public static Transform CardPos1;
    public static Transform CardPos2;
    public static Transform CardPos3;
    public static Transform CardPos4;
    private static AudioSource AudioSource;
    private static TMP_Text TitleText;

    void Awake()
    {
        if (This == null)
        {
            This = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (This != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ActionRef = null;
        InputCards = new List<Card>();
        ReturnedCards = new List<Card>();
        ReturnedQuantities = new List<int>();
        TextInput = FindObjectOfType<TMP_InputField>();
        BoardState = FindObjectOfType<BoardState>();
        SetPanelActive(false);

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public static void CancelActionPanel()
    {
        
    }
    
    public static void ExecuteReturnPanel()
    { 
        print("closing return panel");
        if (ReturnedCards.Count > 0)
        {
                foreach (Card c in ReturnedCards)
                {
                    c.transform.SetParent(null);
                    c.transform.localPosition = new Vector3(1,1,1);
                    c.transform.localScale = new Vector3(1, 1, 1);

                }

                foreach (Deck d in BoardState.Decks.Values)
                {
                    d.SetCardPositions();
                }
                
        }
        SetPanelActive(false);
        ReturnedCards = new List<Card>();

    }
    
    public static void DisplayActionPanel(ActionData actionData)
    {
        CurrentAction = actionData;
        SetPanelActive(true);
        

        switch (InputCards.Count)
        {
            case 1:
                AudioSource = This.transform.Find("2Panel").GetComponent<AudioSource>();
                ActionRef.transform.SetParent(This.transform.FindDeepChild("2Panel/LeftCard"));
                InputCards[0].transform.SetParent(This.transform.FindDeepChild("2Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 2:
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                ActionRef.transform.SetParent(This.transform.FindDeepChild("3Panel/LeftCard"));
                InputCards[0].transform.SetParent(This.transform.FindDeepChild("3Panel/MiddleCard"));
                InputCards[1].transform.SetParent(This.transform.FindDeepChild("3Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 3:
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                ActionRef.transform.SetParent(This.transform.FindDeepChild("4Panel/TopCard"));
                InputCards[0].transform.SetParent(This.transform.FindDeepChild("4Panel/LeftCard"));
                InputCards[1].transform.SetParent(This.transform.FindDeepChild("4Panel/RightCard"));
                InputCards[2].transform.SetParent(This.transform.FindDeepChild("4Panel/BottomCard"));
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                break;
        }
         
        TitleText.text = actionData.ActionResult.Title;
        FlavorText.text = actionData.ActionResult.FlavorText;
         
                 
        ActionRef.transform.localScale = new Vector3(.95f, .89f, 1f);
        ActionRef.transform.localPosition = new Vector3(0f, 0f, 0f);       
        
        foreach (Card c in InputCards)
        {
             
            c.transform.localScale = new Vector3(.95f, .89f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
         
         
    }

    public static void DisplayReturnPanel(Card OpenedActionCard)
    {
         
        for(int i = 0; i < OpenedActionCard.CurrentActionResult.ReturnedCardIDs.Count; i++)
        {
            string id = OpenedActionCard.CurrentActionResult.ReturnedCardIDs[i];
            int qty = OpenedActionCard.CurrentActionResult.ReturnedQuantities[i];
            ReturnedCards.Add(BoardState.GetInstance().AddCard(id, qty, false));
        }
        
        SetPanelActive(true);
        
        switch (ReturnedCards.Count)
        {
            case 1:
                AudioSource = This.transform.Find("1Panel").GetComponent<AudioSource>();
                ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("1Panel/OnlyCard"));
                TitleText = This.transform.FindDeepChild("1Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("1Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 2:
                AudioSource = This.transform.Find("2Panel").GetComponent<AudioSource>();
                ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("2Panel/LeftCard"));
                ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("2Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 3:
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("3Panel/LeftCard"));
                ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("3Panel/MiddleCard"));
                ReturnedCards[2].transform.SetParent(This.transform.FindDeepChild("3Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 4:
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("4Panel/TopCard"));
                ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("4Panel/LeftCard"));
                ReturnedCards[2].transform.SetParent(This.transform.FindDeepChild("4Panel/RightCard"));
                ReturnedCards[3].transform.SetParent(This.transform.FindDeepChild("4Panel/BottomCard"));
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                break;
        }
         
        TitleText.text = OpenedActionCard.CurrentActionResult.Title;
        FlavorText.text = OpenedActionCard.CurrentActionResult.OutcomeText;
        
        foreach (Card c in ReturnedCards)
        {
         
            c.transform.localScale = new Vector3(.95f, .89f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
            c.SetFaceUpState(true);

        }
    }
    
    public void ExecuteActionPanel()
    {
        PlayerState.Instance.AttributeMap[CurrentAction.ActionResult.AttributeModified] += CurrentAction.ActionResult.AttributeModifier;
        PlayerState.Instance.IncrementActionRepetition(CurrentAction);

        ActionRef.CookActionResult();
        ActionRef.transform.SetParent(null);
        ActionRef = null;

        SetPanelActive(false);

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
        
        CurrentAction = null;
        InputCards = new List<Card>(); // Clear other cards
    
    }
    
    public static void SetPanelActive(bool bActive)
    {
        if (ActionGUI.This == null)
        {
            Debug.LogError("Panel or ActionManager instance is not initialized!");
            return;
        }

        int cardsToDisplay = InputCards.Count > 0 ? InputCards.Count + 1  : ReturnedCards.Count;

        switch(cardsToDisplay)
        {
            case 1:
                ActionGUI.This.gameObject.transform.Find("1Panel").gameObject.SetActive(bActive);
                break;
            case 2:
                ActionGUI.This.gameObject.transform.Find("2Panel").gameObject.SetActive(bActive);
                break;
            case 3:
                ActionGUI.This.gameObject.transform.Find("3Panel").gameObject.SetActive(bActive);
                break;
            case 4:
                ActionGUI.This.gameObject.transform.Find("4Panel").gameObject.SetActive(bActive);
                break;
        }

        Time.timeScale = bActive ? 0.0f : 1.0f;
    }
    
    public static void FindInputCards(string[] words)
    {
        if (ActionRef != null && words.Length >= 2)
        {
            InputCards = new List<Card>();
            
            // Check all other words
            for (int i = 1; i < words.Length; i++)
            {
                string currentWord = words[i];
                bool matchFound = false;

                foreach (KeyValuePair<string, Deck> kvp in BoardState.Decks)
                {
                    // Skip the Action deck for these checks
                    if (kvp.Key == "Action")
                    {
                        continue;
                    }
                    
                    Card card = kvp.Value.Cards.FirstOrDefault(c => c.Name.ToLower() == currentWord);
                    
                    if (card != null)
                    {
                        matchFound = true;
                        InputCards.Add(card);
                        break;
                    }
                }

                if (!matchFound)
                {
                    ActionRef = null;
                    InputCards = new List<Card>();
                    break;
                }
            }
        }
    }

    public static void ExecuteInput(string[] words)
    {
        
     //set true to start
         ActionRef = BoardState.Decks["Action"].FirstOrDefault(c => c.Name.ToLower() == words[0]);
        
         // Check if the first word matches an action card
         if (words.Length == 1 && ActionRef != null)
         {
             ActionRef.OpenAction(); //early exit if just a action and we open the action
             ActionRef = null;
             return;
         }

         ActionGUI.FindInputCards(words);
         
         if (InputCards.Count > 0 && words.Length > 1)
         {
             List<CardSpecifier> cardSpecifiers = new List<CardSpecifier>();
             for (int i = 1; i < InputCards.Count; i++)
             {
                 CardData cd = CardDB.CardDataLookup[InputCards[i].ID];
                 CardSpecifier cs = new CardSpecifier(cd.ID, cd.Type, cd.Property);
                 cardSpecifiers.Add(cs);
             }
             
             ActionKey actionKeyToFind = new ActionKey(ActionRef.Name, InputCards[0].ID, PlayerState.Instance.Location, cardSpecifiers);
             
             CardData mainCardData = CardDB.CardDataLookup[InputCards[0].ID];

             List<CardData> cdList = new List<CardData>();
             foreach (Card c in InputCards)
             {
                 cdList.Add(CardDB.CardDataLookup[c.ID]);
             }

             ActionData ad = mainCardData.FindActionData(ActionRef.Name, cdList);

             if (ad == null)
             {
                 TextInput.text = "IMPOSSIBLE.";
                 return; //no action found
             }
             
             ActionRef.CurrentActionResult = ad.ActionResult;
             DisplayActionPanel(ad);
         }
    }

}
