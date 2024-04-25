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

    private static float LastActionTime;
    private static float CooldownDuration;
    private static bool bFirstCoro;
    
    //instance
    public static ActionGUI This; // Singleton instance
    public static Camera MainCamera;
    public static MeshRenderer MeshRenderer;
    public static TMP_Text FlavorText;
    private static BoardState BoardState;
    private static AudioSource AudioSource;
    private static TMP_Text TitleText;
    private static Transform OriginalTransform;

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
        
        CooldownDuration = .5f;
        bFirstCoro = true;
        MainCamera = FindObjectOfType<Camera>();;
        TextInput = FindObjectOfType<TMP_InputField>();
        BoardState = FindObjectOfType<BoardState>();


    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
         
     
    public void xButtonClicked()
    {
        if (IsActionPanelOpen())
        {
            CancelActionPanel();
        }
        else
        {
            StartCoroutine(HandleUserInput());
        }
    }
    

    public void StartInputCoroutine()
     {
         Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
         RaycastHit hit;

         if (Physics.Raycast(ray, out hit))
         {
             if (hit.transform.GetComponent<Card>() != null)  // Check if the hit object is this GameObject
             {
                 return;
             }
         }
         
         StartCoroutine(HandleUserInput());
         bFirstCoro = false; //no cooldown for first enter coro

     }
     
    private IEnumerator HandleUserInput()
     {
         if (!bFirstCoro)
         {
             // Early exit if cooldown has not elapsed
             float timeSinceLastAction = Time.realtimeSinceStartup - LastActionTime;
             if (timeSinceLastAction <= CooldownDuration)
                 yield break;
         }

         // Update the last action time immediately
         LastActionTime = Time.realtimeSinceStartup;
         
         CloseAndExecutePanels();
         
         
         TextInput.interactable = false;
         
         Terminal.ParseText();
         
         if (Terminal.ParsedText.Length > 5)
         {
             Terminal.SetText("TOO MANY CARDS.", true);
             yield break;
         }

         Card NewAction = BoardState.Decks["Action"].FirstOrDefault(c => c.Name.ToLower() == Terminal.ParsedText[0]);

         if (NewAction != null)
         {
             Player.State.SetActionCard(NewAction);

         }

         if (Player.State.GetActionCard() == null)
         {
             Terminal.SetText("NO ACTION FOUND.", true);
             yield break;
         }

         
         
         //Check if the first word matches an action card
         if (Terminal.ParsedText.Length == 1 && Player.State.GetActionCard().bActionFinished)
         {
             Player.State.GetActionCard().OpenAction(); //early exit if just a action and we open the action
             yield break;
         }
         
         if (Player.State.GetActionCard().CurrentActionData != null)
         {
             Terminal.SetText("ACTION NOT READY.", true);
             yield break;
         }
         
         //TRY TO FIND AN ACTION
         Player.State.GetActionCard().CurrentActionData = FindAction(Terminal.ParsedText);
         //TRY TO FIND AN ACTION
         
         if (Player.State.GetActionCard().CurrentActionData != null)
         {
             DisplayActionPanel();
             yield break;
         }

         if (Player.State.GetActionCard().CurrentActionHint != null)
         {
             print("displaying hints");
             DisplayHintPanel();
             yield break;
         }

         Terminal.SetText("IMPOSSIBLE.", true);
         yield break;
     }
    
    public static void CancelActionPanel()
    {
        DisableAllBeginButtons();
        if (Player.State.InputCards.Count > 0)
        {
            foreach (Card c in Player.State.InputCards)
            {
                c.transform.SetParent(null);
                c.transform.localScale = new Vector3(1, 1, 1);
            }

            foreach (Deck d in BoardState.Decks.Values)
            {
                d.SetCardPositions();
            }
                
        }
        
        Player.State.GetActionCard().transform.SetParent(null);
        Player.State.GetActionCard().CurrentActionData = null;
        Player.State.GetActionCard().CurrentActionHint = null;
        Player.State.NullActionCard();
        
        SetPanelActive(false);
        Player.State.InputCards = new List<Card>(); // Clear other cards
        Terminal.SetText("> Action + Data", true);
    }
    
    public static void ExecuteReturnPanel()
    { 
        if (Player.State.ReturnedCards.Count > 0)
        {
            foreach (Card c in Player.State.ReturnedCards)
            {
                c.transform.SetParent(null);
                c.transform.localScale = new Vector3(1, 1, 1);
            }

            foreach (Deck d in BoardState.Decks.Values)
            {
                d.SetCardPositions();
            }
                
        }
        SetPanelActive(false);
        Player.State.ReturnedCards = new List<Card>();

    }

    public static bool IsActionPanelOpen()
    {
        return (Player.State.InputCards.Count > 0 && Player.State.GetActionCard() != null);

    }
    
    public static bool IsHintPanelOpen()
    {
        
        // Find all SpriteRenderer components in the scene
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in allSprites)
        {
            // Check if the sprite is the "questionMark" sprite
            if (spriteRenderer.sprite != null && spriteRenderer.sprite == Resources.Load<Sprite>("Images/QuestionMark"))
            {
                // Return true as soon as one is found
                return true;
            }
        }
        // Return false if no such sprite is found
        return false;

    }
    
    public static bool IsReturnPanelOpen()
    {
        return (Player.State.ReturnedCards.Count > 0);

    }
    
    public static void DisplayActionPanel()
    {
        SetPanelActive(true);
        
        switch (Player.State.InputCards.Count)
        {
            case 1:
                AudioSource = This.transform.Find("2Panel").GetComponent<AudioSource>();
                Player.State.GetActionCard().transform.SetParent(This.transform.FindDeepChild("2Panel/LeftCard"));
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("2Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                This.transform.FindDeepChild("2Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 2:
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                Player.State.GetActionCard().transform.SetParent(This.transform.FindDeepChild("3Panel/LeftCard"));
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("3Panel/MiddleCard"));
                Player.State.InputCards[1].transform.SetParent(This.transform.FindDeepChild("3Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                This.transform.FindDeepChild("3Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 3:
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                Player.State.GetActionCard().transform.SetParent(This.transform.FindDeepChild("4Panel/TopCard"));
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("4Panel/LeftCard"));
                Player.State.InputCards[1].transform.SetParent(This.transform.FindDeepChild("4Panel/RightCard"));
                Player.State.InputCards[2].transform.SetParent(This.transform.FindDeepChild("4Panel/BottomCard"));
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                This.transform.FindDeepChild("4Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 4:
                AudioSource = This.transform.Find("5Panel").GetComponent<AudioSource>();
                Player.State.GetActionCard().transform.SetParent(This.transform.FindDeepChild("5Panel/TopCard"));
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("5Panel/LeftCard"));
                Player.State.InputCards[1].transform.SetParent(This.transform.FindDeepChild("5Panel/RightCard"));
                Player.State.InputCards[2].transform.SetParent(This.transform.FindDeepChild("5Panel/BottomLeftCard"));
                Player.State.InputCards[3].transform.SetParent(This.transform.FindDeepChild("5Panel/BottomRightCard"));
                TitleText = This.transform.FindDeepChild("5Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("5Panel/FlavorText").GetComponent<TMP_Text>();
                This.transform.FindDeepChild("5Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
        }
         
        TitleText.text = Player.State.GetActionCard().CurrentActionData.ActionResult.Title;
        FlavorText.text = Player.State.GetActionCard().CurrentActionData.ActionResult.FlavorText;
         
                 
        Player.State.GetActionCard().transform.localScale = new Vector3(.95f, .89f, 1f);
        Player.State.GetActionCard().transform.localPosition = new Vector3(0f, 0f, 0f);
        Player.State.GetActionCard().OriginalPosition = Player.State.GetActionCard().transform.position;
        
        foreach (Card c in Player.State.InputCards)
        {
            
            c.transform.localScale = new Vector3(.95f, .89f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
            c.OriginalPosition = c.transform.position;
        }
         
         
    }

    public static void DisplayReturnPanel(Card OpenedActionCard)
    {
        OpenedActionCard.bActionFinished = false;

        if (OpenedActionCard.ID == "battle" && !Player.State.GetStarship().GetBattleResults(Player.State.GetBattleOpponent().Data.Price)) //if the action is a battle and the player loses
        {
            Player.State.DecrementActionRepetition(OpenedActionCard.Name, Player.State.GetBattleOpponent().Data);
            Player.State.ReturnedCards.Add(BoardState.GetInstance().AddCard(OpenedActionCard.ID, 1, false));
            Player.State.ReturnedCards.Add(BoardState.GetInstance().AddCard(Player.State.GetBattleOpponent().ID, 1, false));
        }
        else //otherwise proceed as normal
        {
            for(int i = 0; i < OpenedActionCard.CurrentActionData.ActionResult.ReturnedCardIDs.Count; i++)
            {
                string id = OpenedActionCard.CurrentActionData.ActionResult.ReturnedCardIDs[i];
                int qty = OpenedActionCard.CurrentActionData.ActionResult.ReturnedQuantities[i];
                
                Player.State.ReturnedCards.Add(BoardState.GetInstance().AddCard(id, qty, false));
            } 
        }

        SetPanelActive(true);

        switch (Player.State.ReturnedCards.Count)
        {
            case 1:
                AudioSource = This.transform.Find("1Panel").GetComponent<AudioSource>();
                Player.State.ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("1Panel/OnlyCard"));
                TitleText = This.transform.FindDeepChild("1Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("1Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 2:
                AudioSource = This.transform.Find("2Panel").GetComponent<AudioSource>();
                Player.State.ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("2Panel/LeftCard"));
                Player.State.ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("2Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 3:
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                Player.State.ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("3Panel/LeftCard"));
                Player.State.ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("3Panel/MiddleCard"));
                Player.State.ReturnedCards[2].transform.SetParent(This.transform.FindDeepChild("3Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 4:
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                Player.State.ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("4Panel/TopCard"));
                Player.State.ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("4Panel/LeftCard"));
                Player.State.ReturnedCards[2].transform.SetParent(This.transform.FindDeepChild("4Panel/RightCard"));
                Player.State.ReturnedCards[3].transform.SetParent(This.transform.FindDeepChild("4Panel/BottomCard"));
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 5:
                AudioSource = This.transform.Find("5Panel").GetComponent<AudioSource>();
                Player.State.ReturnedCards[0].transform.SetParent(This.transform.FindDeepChild("5Panel/TopCard"));
                Player.State.ReturnedCards[1].transform.SetParent(This.transform.FindDeepChild("5Panel/LeftCard"));
                Player.State.ReturnedCards[2].transform.SetParent(This.transform.FindDeepChild("5Panel/RightCard"));
                Player.State.ReturnedCards[3].transform.SetParent(This.transform.FindDeepChild("5Panel/BottomLeftCard"));
                Player.State.ReturnedCards[4].transform.SetParent(This.transform.FindDeepChild("5Panel/BottomRightCard"));
                TitleText = This.transform.FindDeepChild("5Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("5Panel/FlavorText").GetComponent<TMP_Text>();
                break;
        }
         
        TitleText.text = OpenedActionCard.CurrentActionData.ActionResult.Title;
        FlavorText.text = OpenedActionCard.CurrentActionData.ActionResult.OutcomeText;
        
        foreach (Card c in Player.State.ReturnedCards)
        {
         
            c.transform.localScale = new Vector3(.95f, .89f, 1f);
            c.transform.localPosition = new Vector3(0f, 0f, 0f);
            c.OriginalPosition = c.transform.position;
            c.SetFaceUpState(true);

        }
    }

    public static void SetPanelActive(bool bActive)
    {
        if (This == null)
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
        

        int cardsToDisplay = Player.State.InputCards.Count > 0 ? Player.State.InputCards.Count + 1  : Player.State.ReturnedCards.Count;

        Transform PanelToDisplay;
        
        switch(cardsToDisplay)
        {
            case 1:
                PanelToDisplay = This.gameObject.transform.Find("1Panel");
                break;
            case 2:
                PanelToDisplay = This.gameObject.transform.Find("2Panel");
                break;
            case 3:
                PanelToDisplay = This.gameObject.transform.Find("3Panel");
                break;
            case 4:
                PanelToDisplay = This.gameObject.transform.Find("4Panel");
                break;
            case 5:
                PanelToDisplay = This.gameObject.transform.Find("5Panel");
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
        transform.localScale = new Vector3(424.60388f, 282.37645f, 1.05557001f); //original transform
        //normalize scale
        float xScaling = transform.localScale.x * MainCamera.orthographicSize / 157;
        float ysCaling = transform.localScale.y * MainCamera.orthographicSize / 157;
        transform.localScale = new Vector3(xScaling, ysCaling, transform.localScale.z);
        
        //normalize position
        Vector3 newPos = MainCamera.ViewportToWorldPoint(Vector3.one / 2);
        newPos.z = transform.position.z;
        transform.position = newPos;

        return transform;


    }
    
    public static void SetInputCards(string[] words)
    {
        Player.State.InputCards = new List<Card>();

        if (Player.State.GetActionCard() == null || words.Length < 2)
        {
            return;
        }
        
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
                    Player.State.InputCards.Add(card);
                    break;
                }
            }

            if (!matchFound)
            {
                Player.State.InputCards = new List<Card>();
                break;
            }
        }
    }

    public static ActionData FindAction(string[] words)
    {
         SetInputCards(words);

         if (Player.State.InputCards.Count <= 0 || words.Length <= 1)
         {
             return null;
         }
         
         List<CardSpecifier> cardSpecifiers = new List<CardSpecifier>();
         for (int i = 1; i < Player.State.InputCards.Count; i++)
         {
             CardData cd = CardDB.CardDataLookup[Player.State.InputCards[i].ID];
             CardSpecifier cs = new CardSpecifier(cd.ID, cd.Type, cd.Property);
             cardSpecifiers.Add(cs);
         }
             
         ActionKey actionKeyToFind = new ActionKey(Player.State.GetActionCard().Name, Player.State.InputCards[0].ID, Player.State.GetLocation(), cardSpecifiers);
             
         CardData mainCardData = CardDB.CardDataLookup[Player.State.InputCards[0].ID];

         List<CardData> cdList = new List<CardData>();
         foreach (Card c in Player.State.InputCards)
         {
             cdList.Add(CardDB.CardDataLookup[c.ID]);
         }

         List<ActionData> MatchHint = mainCardData.FindActionData(Player.State.GetActionCard().Name, cdList);

         if (MatchHint[0] != null)
         {
             return MatchHint[0];
         }
         
         if(MatchHint[1] != null)
         {
             Player.State.GetActionCard().CurrentActionHint = MatchHint[1];
         }

         return null;
    }

    public static void CloseHintPanel()
    {
        // Find all SpriteRenderer components in the scene
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer spriteRenderer in allSprites)
        {
            // Check if the sprite is the "questionmark" sprite
            if (spriteRenderer.sprite != null && spriteRenderer.sprite == Resources.Load<Sprite>("Images/QuestionMark"))
            {
                // If it is, destroy the GameObject
                Destroy(spriteRenderer.gameObject);
            }
        }
        
        foreach (Card c in Player.State.InputCards)
        {
            c.transform.SetParent(null);
            c.transform.localScale = new Vector3(1, 1, 1);
        }

        foreach (Deck d in BoardState.Decks.Values)
        {
            d.SetCardPositions();
        }
        Player.State.InputCards = new List<Card>(); // Clear other cards
    }

    public static SpriteRenderer CreateQuestionMark(Transform targetParent)
    {
        SpriteRenderer questionMark2 = new SpriteRenderer();
        questionMark2.sprite = Resources.Load<Sprite>("Images/QuestionMark");
        questionMark2.gameObject.transform.SetParent(targetParent);
        questionMark2.transform.localPosition = new Vector3(-0.1f, 0.94f, -1f);
        questionMark2.transform.localRotation = Quaternion.identity;
        questionMark2.transform.localScale = new Vector3(5.75f, 5.8936f, 6.3468f);
        return questionMark2;
    }
    
    public static void DisplayHintPanel()
    {

        int firstIncorrectIndex = 0;
        List<CardSpecifier> Hints = Player.State.GetActionCard().CurrentActionHint.ActionKey.SecondaryCardSpecifiersReal;
        
        for(int i = 0; i < Hints.Count; i++)
        {
            if (Player.State.InputCards[i + 1] != null)
            {
                if (!Hints[i].MatchCard(Player.State.InputCards[i + 1].Data))
                {   
                    firstIncorrectIndex = i + 1;
                    break;
                }
            }
        }
        
        Transform PanelToDisplay = This.gameObject.transform.Find("3Panel"); //some default
        
        switch (Hints.Count)
        {
            case 1:
                PanelToDisplay = This.gameObject.transform.Find("3Panel");
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                Player.State.GetActionCard().transform.SetParent(This.transform.FindDeepChild("3Panel/LeftCard"));
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("3Panel/MiddleCard"));
                CreateQuestionMark(This.transform.FindDeepChild("3Panel/RightCard"));
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            case 2:
                PanelToDisplay = This.gameObject.transform.Find("4Panel");
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                Player.State.GetActionCard().transform.SetParent(This.transform.FindDeepChild("4Panel/TopCard"));
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("4Panel/LeftCard"));
                if(firstIncorrectIndex == 1) { Player.State.InputCards[1].transform.SetParent(This.transform.FindDeepChild("4Panel/RightCard")); } 
                                                else { CreateQuestionMark(This.transform.FindDeepChild("4Panel/RightCard")); }
                if(firstIncorrectIndex <= 2) { Player.State.InputCards[2].transform.SetParent(This.transform.FindDeepChild("4Panel/BottomCard")); } 
                                                else { CreateQuestionMark(This.transform.FindDeepChild("4Panel/BottomCard")); }
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                
                break;
            case 3:
                PanelToDisplay = This.gameObject.transform.Find("5Panel");
                AudioSource = This.transform.Find("5Panel").GetComponent<AudioSource>();
                Player.State.InputCards[0].transform.SetParent(This.transform.FindDeepChild("5Panel/TopCard"));
                if(firstIncorrectIndex == 1) { Player.State.InputCards[1].transform.SetParent(This.transform.FindDeepChild("5Panel/LeftCard")); } 
                                                else { CreateQuestionMark(This.transform.FindDeepChild("5Panel/LeftCard")); }
                if(firstIncorrectIndex <= 2) { Player.State.InputCards[2].transform.SetParent(This.transform.FindDeepChild("5Panel/RightCard")); } 
                                                else { CreateQuestionMark(This.transform.FindDeepChild("5Panel/RightCard")); }
                if(firstIncorrectIndex <= 3) { Player.State.InputCards[3].transform.SetParent(This.transform.FindDeepChild("5Panel/BottomLeftCard")); } 
                                                else { CreateQuestionMark(This.transform.FindDeepChild("5Panel/BottomLeftCard")); }
                if(firstIncorrectIndex <= 4) { Player.State.InputCards[4].transform.SetParent(This.transform.FindDeepChild("5Panel/BottomRightCard")); } 
                                                else { CreateQuestionMark(This.transform.FindDeepChild("5Panel/BottomRightCard")); }
                TitleText = This.transform.FindDeepChild("5Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("5Panel/FlavorText").GetComponent<TMP_Text>();
                break;
            
        }
        
        PanelToDisplay = NormalizePanel(PanelToDisplay);
        PanelToDisplay.gameObject.SetActive(true); 
        TitleText.text = Player.State.GetActionCard().CurrentActionData.ActionResult.Title;
        FlavorText.text = Player.State.GetActionCard().CurrentActionData.ActionResult.FlavorText;
         
                 
        Player.State.GetActionCard().transform.localScale = new Vector3(.95f, .89f, 1f);
        Player.State.GetActionCard().transform.localPosition = new Vector3(0f, 0f, 0f);
        Player.State.GetActionCard().OriginalPosition = Player.State.GetActionCard().transform.position;
        
        foreach (Card c in Player.State.InputCards)
        {
            if (c.transform.parent.name.Contains("Card"))
            {
                c.transform.localScale = new Vector3(.95f, .89f, 1f);
                c.transform.localPosition = new Vector3(0f, 0f, 0f);
                c.OriginalPosition = c.transform.position;
            }
        }
    }

    private static void DisableAllBeginButtons()
    {
        This.transform.FindDeepChild("2Panel/Canvas/ActionButton").gameObject.SetActive(false);
        This.transform.FindDeepChild("3Panel/Canvas/ActionButton").gameObject.SetActive(false);
        This.transform.FindDeepChild("4Panel/Canvas/ActionButton").gameObject.SetActive(false);
        This.transform.FindDeepChild("5Panel/Canvas/ActionButton").gameObject.SetActive(false);
    }


    private static void CloseAndExecutePanels()
    {
        if (IsHintPanelOpen())
        {
            CloseHintPanel();
            Terminal.SetText("> Action + Card", true);
            return;
        }
        
        if(IsActionPanelOpen())
        {
            Player.State.ExecuteAction();
            ActionGUI.DisableAllBeginButtons();
            Terminal.SetText("> Action + Card", true);
            return;
        }
         
        if(IsReturnPanelOpen())
        {
            ExecuteReturnPanel();
            Terminal.SetText("> Action + Card", true);
            return;

        }
    }
}
