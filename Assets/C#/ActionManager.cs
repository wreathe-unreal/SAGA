using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class ActionManager : MonoBehaviour
{
    public static TMP_InputField TextInput;
    
    //for action return only
    public static List<Card> ReturnedCards;
    public static List<int> ReturnedQuantities;
    
    //for action display only
    public static List<Card> InputCards;
    public static Card ActionRef;
    
    //instance
    public static ActionManager Instance; // Singleton instance

    public static MeshRenderer MeshRenderer;
    private static Transform PanelTextTransform;
    public static TMP_Text PanelText;
    private static BoardState _boardState;
    public static Transform CardPos1;
    public static Transform CardPos2;
    public static Transform CardPos3;
    public static Transform CardPos4;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Assign the instance
            DontDestroyOnLoad(gameObject); // Optional: Keep this object alive when loading new scenes
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // Destroy this instance because it's a duplicate
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
        _boardState = FindObjectOfType<BoardState>();
        CardPos1 = gameObject.transform.Find("LeftCard");
        CardPos2 = gameObject.transform.Find("RightCard");
        CardPos3 = gameObject.transform.Find("TopCard");
        CardPos4 = gameObject.transform.Find("BottomCard");
        PanelTextTransform = gameObject.transform.Find("FlavorText");
        PanelText = PanelTextTransform.GetComponent<TMP_Text>();
        MeshRenderer = gameObject.GetComponent<MeshRenderer>();
        SetPanelActive(false);

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void CloseReturnPanel()
    {
        if (ReturnedCards.Count > 0)
        {
                foreach (Card c in ReturnedCards)
                {
                    c.transform.SetParent(null);
                    c.transform.localScale = new Vector3(55f, 55f, 1f);

                }

                foreach (Deck d in BoardState.Decks.Values)
                {
                    d.SetCardPositions();
                }

                ReturnedCards = new List<Card>();
        }
        
        SetPanelActive(false);
    }
    
    public static void DisplayActionPanel(ActionResult AR)
    {
        SetPanelActive(true);
        PanelText.text = AR.FlavorText;

        switch (InputCards.Count)
        {
            case 1:
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                ActionRef.transform.SetParent(CardPos1);
                InputCards[0].transform.SetParent(CardPos2);


                break;
            case 2:
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/3Panel");
                ActionRef.transform.SetParent(CardPos1);
                InputCards[0].transform.SetParent(CardPos2);
                InputCards[1].transform.SetParent(CardPos3);

                break;
            case 3:
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/4Panel");
                ActionRef.transform.SetParent(CardPos1);
                InputCards[0].transform.SetParent(CardPos2);
                InputCards[1].transform.SetParent(CardPos3);
                InputCards[2].transform.SetParent(CardPos4);
                break;
        }
         
         
                 
        ActionRef.transform.localScale = new Vector3(1f, 1f, 1f);
        ActionRef.transform.localPosition = new Vector3(0f, 0f, 0f);                 
        foreach (Card c in InputCards)
        {
             
            c.transform.localScale = new Vector3(1f, 1f, 1f);
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
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                ReturnedCards[0].transform.SetParent(ActionManager.CardPos1);
                break;
            case 2:
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                ReturnedCards[0].transform.SetParent(ActionManager.CardPos1);
                ReturnedCards[1].transform.SetParent(ActionManager.CardPos2);
                break;
            case 3:
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/3Panel");
                ReturnedCards[0].transform.SetParent(ActionManager.CardPos1);
                ReturnedCards[1].transform.SetParent(ActionManager.CardPos2);
                ReturnedCards[2].transform.SetParent(ActionManager.CardPos4);
                break;
            case 4:
                ActionManager.PanelText.transform.localPosition = new Vector3(-0.181f, 0.679f, 0f);
                ActionManager.MeshRenderer.material.mainTexture = Resources.Load<Texture>("Images/4Panel");
                ReturnedCards[0].transform.SetParent(ActionManager.CardPos1);
                ReturnedCards[1].transform.SetParent(ActionManager.CardPos2);
                ReturnedCards[2].transform.SetParent(ActionManager.CardPos4);
                ReturnedCards[3].transform.SetParent(ActionManager.CardPos3);
                break;
        }
        
        foreach (Card c in ReturnedCards)
        {
         
            c.transform.localScale = new Vector3(1f, 1f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
    
    public void CloseActionPanel()
    {
       
        List<Card> cardsToRemove = new List<Card>();

        foreach (Card c in InputCards)
        {
            cardsToRemove.Add(c);
        }

        foreach (Card c in cardsToRemove)
        {
            _boardState.DestroyCard(c);
            InputCards.Remove(c);
        }

        ActionRef.CookActionResult();
        SetPanelActive(false);
        ActionRef = null;
        InputCards = new List<Card>(); // Clear other cards
    
    }
    
    public static void SetPanelActive(bool bActive)
    {
        if (ActionManager.Instance.gameObject == null)
        {
            Debug.LogError("Panel is not initialized!");
            return;
        }
        
        if (bActive)
        {
            ActionManager.Instance.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else
        {             
            ActionManager.Instance.gameObject.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }
    
    public static void FindActionResult(string[] words)
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

         ActionManager.FindActionResult(words);
         
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

             ActionResult ar = mainCardData.GetActionResult(ActionRef.Name, cdList);

             if (ar == null)
             {
                 TextInput.text = "IMPOSSIBLE.";
                 return; //no action found
             }
             
             ActionRef.CurrentActionResult = ar;
             
             DisplayActionPanel(ar);
         }
    }

}
