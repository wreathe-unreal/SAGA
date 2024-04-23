using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ActionGUI : MonoBehaviour
{
    public static TMP_InputField TextInput;

    
    //for action return only
    public static List<Card> ReturnedCards;
    public static List<int> ReturnedQuantities;
    
    //for action display only
    public static List<Card> InputCards;
    public static Card ActionCard;

    public static Card BattleOpponent;
    
    //instance
    public static ActionGUI This; // Singleton instance
    public static Camera MainCamera;
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
        MainCamera = FindObjectOfType<Camera>();
        ActionCard = null;
        InputCards = new List<Card>();
        ReturnedCards = new List<Card>();
        ReturnedQuantities = new List<int>();
        TextInput = FindObjectOfType<TMP_InputField>();
        BoardState = FindObjectOfType<BoardState>();

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public static void CancelActionPanel()
    {
        
        if (InputCards.Count > 0)
        {
            foreach (Card c in InputCards)
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
        ActionCard.transform.SetParent(null);
        ActionCard = null;
        SetPanelActive(false);
        InputCards = new List<Card>(); // Clear other cards
    }
    
    public static void ExecuteReturnPanel()
    { 
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

    public static bool IsActionPanelOpen()
    {
        return (InputCards.Count > 0 && ActionCard != null);

    }
    
    public static bool IsReturnPanelOpen()
    {
        return (ReturnedCards.Count > 0);

    }
    
    public static void DisplayActionPanel()
    {
        SetPanelActive(true);
        
        switch (InputCards.Count)
        {
            case 1:
                AudioSource = This.transform.Find("2Panel").GetComponent<AudioSource>();
                ActionCard.transform.SetParent(This.transform.FindDeepChild("2Panel/LeftCard"));
                InputCards[0].transform.SetParent(This.transform.FindDeepChild("2Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 2:
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                ActionCard.transform.SetParent(This.transform.FindDeepChild("3Panel/LeftCard"));
                InputCards[0].transform.SetParent(This.transform.FindDeepChild("3Panel/MiddleCard"));
                InputCards[1].transform.SetParent(This.transform.FindDeepChild("3Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 3:
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                ActionCard.transform.SetParent(This.transform.FindDeepChild("4Panel/TopCard"));
                InputCards[0].transform.SetParent(This.transform.FindDeepChild("4Panel/LeftCard"));
                InputCards[1].transform.SetParent(This.transform.FindDeepChild("4Panel/RightCard"));
                InputCards[2].transform.SetParent(This.transform.FindDeepChild("4Panel/BottomCard"));
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                break;
        }
         
        TitleText.text = ActionCard.CurrentAction.ActionResult.Title;
        FlavorText.text = ActionCard.CurrentAction.ActionResult.FlavorText;
         
                 
        ActionCard.transform.localScale = new Vector3(.95f, .89f, 1f);
        ActionCard.transform.localPosition = new Vector3(0f, 0f, 0f);
        ActionCard.OriginalPosition = ActionCard.transform.position;
        
        foreach (Card c in InputCards)
        {
            
            c.transform.localScale = new Vector3(.95f, .89f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
            c.OriginalPosition = c.transform.position;
        }
         
         
    }

    public static void DisplayReturnPanel(Card OpenedActionCard)
    {
        if (OpenedActionCard.ID == "battle" && !PlayerState.Instance.Starship.GetBattleResults(BattleOpponent.Data.Price)) //if the action is a battle and the player loses
        {
            PlayerState.Instance.DecrementActionRepetition(OpenedActionCard.CurrentAction);
            ReturnedCards.Add(BoardState.GetInstance().AddCard(OpenedActionCard.ID, 1, false));
            ReturnedCards.Add(BoardState.GetInstance().AddCard(BattleOpponent.ID, 1, false));
        }
        else //otherwise proceed as normal
        {
            for(int i = 0; i < OpenedActionCard.CurrentAction.ActionResult.ReturnedCardIDs.Count; i++)
            {
                string id = OpenedActionCard.CurrentAction.ActionResult.ReturnedCardIDs[i];
                int qty = OpenedActionCard.CurrentAction.ActionResult.ReturnedQuantities[i];
                
                    ReturnedCards.Add(BoardState.GetInstance().AddCard(id, qty, false));
            } 
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
         
        TitleText.text = OpenedActionCard.CurrentAction.ActionResult.Title;
        FlavorText.text = OpenedActionCard.CurrentAction.ActionResult.OutcomeText;
        
        foreach (Card c in ReturnedCards)
        {
         
            c.transform.localScale = new Vector3(.95f, .89f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
            c.OriginalPosition = c.transform.position;
            c.SetFaceUpState(true);

        }
    }
    
    public void ExecuteActionPanel()
    {
        PlayerState.Instance.AttributeMap[ActionCard.CurrentAction.ActionResult.AttributeModified] += ActionCard.CurrentAction.ActionResult.AttributeModifier;
        PlayerState.Instance.IncrementActionRepetition(ActionCard.CurrentAction);

        ActionCard.CookActionResult();
        ActionCard.transform.SetParent(null);
        ActionCard = null;

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
        
        InputCards = new List<Card>(); // Clear other cards
    
    }
    
    public static void SetPanelActive(bool bActive)
    {
        if (ActionGUI.This == null)
        {
            Debug.LogError("Panel or ActionManager instance is not initialized!");
            return;
        }

        if (bActive)
        {
            TextInput.interactable = false;
        }
        else
        {
            TextInput.interactable = true;
        }
        

        int cardsToDisplay = InputCards.Count > 0 ? InputCards.Count + 1  : ReturnedCards.Count;

        Transform PanelToDisplay;
        
        switch(cardsToDisplay)
        {
            case 1:
                PanelToDisplay = ActionGUI.This.gameObject.transform.Find("1Panel");
                break;
            case 2:
                PanelToDisplay = ActionGUI.This.gameObject.transform.Find("2Panel");
                break;
            case 3:
                PanelToDisplay = ActionGUI.This.gameObject.transform.Find("3Panel");
                break;
            case 4:
                PanelToDisplay = ActionGUI.This.gameObject.transform.Find("4Panel");
                break;
            default:
                PanelToDisplay = null;
                break;
        }

        if (PanelToDisplay == null)
        {
            return;
        }

        PanelToDisplay = NormalizePanel(PanelToDisplay);
        PanelToDisplay.gameObject.SetActive(bActive);

        Time.timeScale = bActive ? 0.0f : 1.0f;
    }

    public static Transform NormalizePanel(Transform transform)
    {
        
        //normalize scale
        float xScaling = transform.localScale.x * MainCamera.orthographicSize / 240;
        float ysCaling = transform.localScale.y * MainCamera.orthographicSize / 240;
        transform.localScale = new Vector3(xScaling, ysCaling, transform.localScale.z);
        
        //normalize position
        Vector3 newPos = MainCamera.ViewportToWorldPoint(Vector3.one / 2);
        newPos.z = transform.position.z;
        transform.position = newPos;

        return transform;


    }
    
    public static void SetInputCards(string[] words)
    {
        if (ActionCard != null && words.Length >= 2)
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
                    ActionCard = null;
                    InputCards = new List<Card>();
                    break;
                }
            }
        }
    }

    public static ActionData FindAction(string[] words)
    {
        if (ActionCard == null)
        {
            return null;
        }

         ActionGUI.SetInputCards(words);
         
         if (InputCards.Count > 0 && words.Length > 1)
         {
             List<CardSpecifier> cardSpecifiers = new List<CardSpecifier>();
             for (int i = 1; i < InputCards.Count; i++)
             {
                 CardData cd = CardDB.CardDataLookup[InputCards[i].ID];
                 CardSpecifier cs = new CardSpecifier(cd.ID, cd.Type, cd.Property);
                 cardSpecifiers.Add(cs);
             }
             
             ActionKey actionKeyToFind = new ActionKey(ActionCard.Name, InputCards[0].ID, PlayerState.Instance.Location, cardSpecifiers);
             
             CardData mainCardData = CardDB.CardDataLookup[InputCards[0].ID];

             List<CardData> cdList = new List<CardData>();
             foreach (Card c in InputCards)
             {
                 cdList.Add(CardDB.CardDataLookup[c.ID]);
             }

             ActionData ad = mainCardData.FindActionData(ActionCard.Name, cdList);

             if (ad == null)
             {
                 TextInput.text = "IMPOSSIBLE.";// Get the current ColorBlock from the TextInput
                 ActionCard = null;
                 return null; //no action found
             }
             
             return ad;
         }

         return null;
    }

}
