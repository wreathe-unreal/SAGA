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
    public static TMP_Text PanelText;
    private static BoardState BoardState;
    public static Transform CardPos1;
    public static Transform CardPos2;
    public static Transform CardPos3;
    public static Transform CardPos4;
    private static AudioSource AudioSource;
    
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
        CardPos1 = gameObject.transform.Find("Panel/LeftCard");
        CardPos2 = gameObject.transform.Find("Panel/RightCard");
        CardPos3 = gameObject.transform.Find("Panel/BottomCard");
        CardPos4 = gameObject.transform.Find("Panel/TopCard");
        PanelText = gameObject.transform.Find("Panel/FlavorText").GetComponent<TMP_Text>();
        MeshRenderer = gameObject.transform.Find("Panel").GetComponent<MeshRenderer>();
        AudioSource = gameObject.transform.Find("Panel").GetComponent<AudioSource>();
        SetPanelActive(false);

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void CloseReturnPanel()
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

                ReturnedCards = new List<Card>();
        }
        
        SetPanelActive(false);
    }
    
    public static void DisplayActionPanel(ActionData actionData)
    {
        CurrentAction = actionData;
        SetPanelActive(true);
        PanelText.text = actionData.ActionResult.FlavorText;

        switch (InputCards.Count)
        {
            case 1:
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                ActionRef.transform.SetParent(CardPos1);
                InputCards[0].transform.SetParent(CardPos2);


                break;
            case 2:
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/3Panel");
                ActionRef.transform.SetParent(CardPos1);
                InputCards[0].transform.SetParent(CardPos2);
                InputCards[1].transform.SetParent(CardPos3);

                break;
            case 3:
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/4Panel");
                ActionRef.transform.SetParent(CardPos1);
                InputCards[0].transform.SetParent(CardPos2);
                InputCards[1].transform.SetParent(CardPos3);
                InputCards[2].transform.SetParent(CardPos4);
                break;
        }
         
         
                 
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
        PanelText.text = OpenedActionCard.CurrentActionResult.OutcomeText;
        SetPanelActive(true);

        

        for(int i = 0; i < OpenedActionCard.CurrentActionResult.ReturnedCardIDs.Count; i++)
        {
            string id = OpenedActionCard.CurrentActionResult.ReturnedCardIDs[i];
            int qty = OpenedActionCard.CurrentActionResult.ReturnedQuantities[i];
            ReturnedCards.Add(BoardState.GetInstance().AddCard(id, qty, false));
        }
        
        switch (ReturnedCards.Count)
        {
            case 1:
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                ReturnedCards[0].transform.SetParent(ActionGUI.CardPos1);
                break;
            case 2:
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                ReturnedCards[0].transform.SetParent(ActionGUI.CardPos1);
                ReturnedCards[1].transform.SetParent(ActionGUI.CardPos2);
                break;
            case 3:
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/3Panel");
                ReturnedCards[0].transform.SetParent(ActionGUI.CardPos1);
                ReturnedCards[1].transform.SetParent(ActionGUI.CardPos2);
                ReturnedCards[2].transform.SetParent(ActionGUI.CardPos4);
                break;
            case 4:
                ActionGUI.PanelText.transform.localPosition = new Vector3(-0.181f, 0.679f, 0f);
                ActionGUI.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/4Panel");
                ReturnedCards[0].transform.SetParent(ActionGUI.CardPos1);
                ReturnedCards[1].transform.SetParent(ActionGUI.CardPos2);
                ReturnedCards[2].transform.SetParent(ActionGUI.CardPos4);
                ReturnedCards[3].transform.SetParent(ActionGUI.CardPos3);
                break;
        }
        
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

        ActionRef.CookActionResult();
        SetPanelActive(false);
        ActionRef = null;
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
    
        ActionGUI.This.gameObject.SetActive(bActive);
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
