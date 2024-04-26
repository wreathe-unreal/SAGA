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
    private static Board _board;
    private static AudioSource AudioSource;
    private static TMP_Text TitleText;
    private static Transform OriginalTransform;
    private static Transform DisplayedPanel;
    private static List<Transform> OnePanelTransforms;
    private static List<Transform> TwoPanelTransforms;
    private static List<Transform> ThreePanelTransforms;
    private static List<Transform> FourPanelTransforms;
    private static List<Transform> FivePanelTransforms;
    private static List<Transform> BeginActionButtons;


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
        _board = FindObjectOfType<Board>();

        OnePanelTransforms = new List<Transform>();
        OnePanelTransforms.Add(This.transform.FindDeepChild("1Panel/OnlyCard"));
        TwoPanelTransforms = new List<Transform>();
        TwoPanelTransforms.Add(This.transform.FindDeepChild("2Panel/LeftCard"));
        TwoPanelTransforms.Add(This.transform.FindDeepChild("2Panel/RightCard"));
        ThreePanelTransforms = new List<Transform>();
        ThreePanelTransforms.Add(This.transform.FindDeepChild("3Panel/LeftCard"));
        ThreePanelTransforms.Add(This.transform.FindDeepChild("3Panel/MiddleCard"));
        ThreePanelTransforms.Add(This.transform.FindDeepChild("3Panel/RightCard"));
        FourPanelTransforms = new List<Transform>();
        FourPanelTransforms.Add(This.transform.FindDeepChild("4Panel/TopCard"));
        FourPanelTransforms.Add(This.transform.FindDeepChild("4Panel/LeftCard"));
        FourPanelTransforms.Add(This.transform.FindDeepChild("4Panel/RightCard"));
        FourPanelTransforms.Add(This.transform.FindDeepChild("4Panel/BottomCard"));
        FivePanelTransforms = new List<Transform>();
        FivePanelTransforms.Add(This.transform.FindDeepChild("5Panel/TopCard"));
        FivePanelTransforms.Add(This.transform.FindDeepChild("5Panel/LeftCard"));
        FivePanelTransforms.Add(This.transform.FindDeepChild("5Panel/RightCard"));
        FivePanelTransforms.Add(This.transform.FindDeepChild("5Panel/BottomLeftCard"));
        FivePanelTransforms.Add(This.transform.FindDeepChild("5Panel/BottomRightCard"));
        BeginActionButtons = new List<Transform>();
        BeginActionButtons.Add(This.transform.FindDeepChild("2Panel/Canvas/ActionButton"));
        BeginActionButtons.Add(This.transform.FindDeepChild("3Panel/Canvas/ActionButton"));
        BeginActionButtons.Add(This.transform.FindDeepChild("4Panel/Canvas/ActionButton"));
        BeginActionButtons.Add(This.transform.FindDeepChild("5Panel/Canvas/ActionButton"));
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
         
         Terminal.SetText("> Action + Card", true);

         if (Terminal.ParsedText.Length > 5)
         {
             Terminal.SetText("TOO MANY CARDS.", true);
             yield break;
         }

         Card NewAction = Board.Decks["Action"].FirstOrDefault(c => c.Name.ToLower() == Terminal.ParsedText[0]);

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
         Player.State.FindAction(Terminal.ParsedText);
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

        List<Card> PanelCards = Player.State.GetInputCards();
        
        if (PanelCards.Count > 0)
        {
            foreach (Card c in PanelCards)
            {
                c.transform.SetParent(null);
                c.transform.localScale = new Vector3(1, 1, 1);
            }

            foreach (Deck d in Board.Decks.Values)
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

            foreach (Deck d in Board.Decks.Values)
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
                Player.State.GetActionCard().transform.SetParent(TwoPanelTransforms[0]);
                Player.State.InputCards[0].transform.SetParent(TwoPanelTransforms[1]);
                This.transform.FindDeepChild("2Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 2:
                Player.State.GetActionCard().transform.SetParent(ThreePanelTransforms[0]);
                Player.State.InputCards[0].transform.SetParent(ThreePanelTransforms[1]);
                Player.State.InputCards[1].transform.SetParent(ThreePanelTransforms[2]);
                This.transform.FindDeepChild("3Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 3:
                Player.State.GetActionCard().transform.SetParent(FourPanelTransforms[0]);
                Player.State.InputCards[0].transform.SetParent(FourPanelTransforms[1]);
                Player.State.InputCards[1].transform.SetParent(FourPanelTransforms[2]);
                Player.State.InputCards[2].transform.SetParent(FourPanelTransforms[3]);
                This.transform.FindDeepChild("4Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 4:
                Player.State.GetActionCard().transform.SetParent(FivePanelTransforms[0]);
                Player.State.InputCards[0].transform.SetParent(FivePanelTransforms[1]);
                Player.State.InputCards[1].transform.SetParent(FivePanelTransforms[2]);
                Player.State.InputCards[2].transform.SetParent(FivePanelTransforms[3]);
                Player.State.InputCards[3].transform.SetParent(FivePanelTransforms[4]);
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
            Player.State.ReturnedCards.Add(Board.GetInstance().AddCard(OpenedActionCard.ID, 1, false));
            Player.State.ReturnedCards.Add(Board.GetInstance().AddCard(Player.State.GetBattleOpponent().ID, 1, false));
        }
        else //otherwise proceed as normal
        {
            for(int i = 0; i < OpenedActionCard.CurrentActionData.ActionResult.ReturnedCardIDs.Count; i++)
            {
                string id = OpenedActionCard.CurrentActionData.ActionResult.ReturnedCardIDs[i];
                int qty = OpenedActionCard.CurrentActionData.ActionResult.ReturnedQuantities[i];
                
                Player.State.ReturnedCards.Add(Board.GetInstance().AddCard(id, qty, false));
            } 
        }

        List<Card> Returned = Player.State.GetReturnedCards();

        SetPanelActive(true);

        switch (Returned.Count)
        {
            case 1:
                Returned[0].transform.SetParent(OnePanelTransforms[0]);
                
                break;
            case 2:
                Returned[0].transform.SetParent(TwoPanelTransforms[0]);
                Returned[1].transform.SetParent(TwoPanelTransforms[1]);
                break;
            case 3:
                Returned[0].transform.SetParent(ThreePanelTransforms[0]);
                Returned[1].transform.SetParent(ThreePanelTransforms[1]);
                Returned[2].transform.SetParent(ThreePanelTransforms[2]);
                break;
            case 4:
                Returned[0].transform.SetParent(FourPanelTransforms[0]);
                Returned[1].transform.SetParent(FourPanelTransforms[1]);
                Returned[2].transform.SetParent(FourPanelTransforms[2]);
                Returned[3].transform.SetParent(FourPanelTransforms[3]);
                break;
            case 5:
                Returned[0].transform.SetParent(FivePanelTransforms[0]);
                Returned[1].transform.SetParent(FivePanelTransforms[1]);
                Returned[2].transform.SetParent(FivePanelTransforms[2]);
                Returned[3].transform.SetParent(FivePanelTransforms[3]);
                Returned[4].transform.SetParent(FivePanelTransforms[4]);
                break;
        }
         
        TitleText.text = OpenedActionCard.CurrentActionData.ActionResult.Title;
        FlavorText.text = OpenedActionCard.CurrentActionData.ActionResult.OutcomeText;
        
        foreach (Card c in Returned)
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

        if (!bActive)
        {
            TextInput.interactable = true;
            DisplayedPanel.gameObject.SetActive(false);
            Time.timeScale = 1.0f; //return to real time when panel is closed
            return;
        }

        SetDisplayedPanel();

        if (DisplayedPanel == null)
        {
            return;
        }

        DisplayedPanel = NormalizePanel(DisplayedPanel);
        DisplayedPanel.gameObject.SetActive(true);
        Time.timeScale = 0.0f; //pause game time when panel is active
        TextInput.interactable = false;
    }

    public static Transform SetDisplayedPanel()
    {
        int cardsToDisplay = Player.State.InputCards.Count > 0 ? Player.State.InputCards.Count + 1  : Player.State.ReturnedCards.Count;
        
        switch(cardsToDisplay)
        {
            case 1:
                DisplayedPanel = This.gameObject.transform.Find("1Panel");
                TitleText = This.transform.FindDeepChild("1Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("1Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = This.transform.Find("1Panel").GetComponent<AudioSource>();
                break;
            case 2:
                DisplayedPanel = This.gameObject.transform.Find("2Panel");
                TitleText = This.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = This.transform.Find("2Panel").GetComponent<AudioSource>();
                break;
            case 3:
                DisplayedPanel = This.gameObject.transform.Find("3Panel");
                TitleText = This.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = This.transform.Find("3Panel").GetComponent<AudioSource>();
                break;
            case 4:
                DisplayedPanel = This.gameObject.transform.Find("4Panel");
                TitleText = This.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = This.transform.Find("4Panel").GetComponent<AudioSource>();
                break;
            case 5:
                DisplayedPanel = This.gameObject.transform.Find("5Panel");
                TitleText = This.transform.FindDeepChild("5Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = This.transform.FindDeepChild("5Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = This.transform.Find("5Panel").GetComponent<AudioSource>();
                break;
            default:
                DisplayedPanel = null;
                TitleText = null;
                FlavorText = null;
                AudioSource = null;
                break;
        }

        return DisplayedPanel;
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
    
    

    public static void ExecuteActionPanel()
    {
        Player.State.ExecuteAction();
        SetPanelActive(false);
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

        foreach (Deck d in Board.Decks.Values)
        {
            d.SetCardPositions();
        }
        Player.State.InputCards = new List<Card>(); // Clear other cards
    }

    public static SpriteRenderer CreateQuestionMark(Transform targetParent)
    {
        SpriteRenderer questionMark = new SpriteRenderer();
        questionMark.sprite = Resources.Load<Sprite>("Images/QuestionMark");
        questionMark.gameObject.transform.SetParent(targetParent);
        questionMark.transform.localPosition = new Vector3(-0.1f, 0.94f, -1f);
        questionMark.transform.localRotation = Quaternion.identity;
        questionMark.transform.localScale = new Vector3(5.75f, 5.8936f, 6.3468f);
        return questionMark;
    }
    
    public static void DisplayHintPanel()
    {
        List<Card> InputCards = Player.State.GetInputCards();
        
        int firstIncorrectIndex = 0;
        List<CardSpecifier> Hints = Player.State.GetActionCard().CurrentActionHint.ActionKey.SecondaryCardSpecifiersReal;
        
        for(int i = 0; i < Hints.Count; i++)
        {
            if (InputCards[i + 1] != null)
            {
                if (!Hints[i].MatchCard(InputCards[i + 1].Data))
                {   
                    firstIncorrectIndex = i + 1;
                    break;
                }
            }
        }
        
        switch (Hints.Count)
        {
            case 1:
                DisplayedPanel = This.gameObject.transform.Find("3Panel");
                Player.State.GetActionCard().transform.SetParent(ThreePanelTransforms[0]);
                InputCards[0].transform.SetParent(ThreePanelTransforms[1]);
                CreateQuestionMark(ThreePanelTransforms[2]);
                break;
            case 2:
                DisplayedPanel = This.gameObject.transform.Find("4Panel");
                Player.State.GetActionCard().transform.SetParent(FourPanelTransforms[0]);
                InputCards[0].transform.SetParent(FourPanelTransforms[1]);
                if(firstIncorrectIndex == 1) { InputCards[1].transform.SetParent(FourPanelTransforms[2]); } else { CreateQuestionMark(FourPanelTransforms[2]); }
                if(firstIncorrectIndex <= 2) { InputCards[2].transform.SetParent(FourPanelTransforms[3]); } else { CreateQuestionMark(FourPanelTransforms[3]); }
                
                break;
            case 3:
                DisplayedPanel = This.gameObject.transform.Find("5Panel");
                Player.State.GetActionCard().transform.SetParent(FivePanelTransforms[0]);
                InputCards[0].transform.SetParent(FivePanelTransforms[1]);
                if(firstIncorrectIndex == 1) { InputCards[1].transform.SetParent(FivePanelTransforms[2]); } else { CreateQuestionMark(FivePanelTransforms[2]); }
                if(firstIncorrectIndex <= 2) { InputCards[2].transform.SetParent(FivePanelTransforms[3]); } else { CreateQuestionMark(FivePanelTransforms[3]); }
                if(firstIncorrectIndex <= 3) { InputCards[3].transform.SetParent(FivePanelTransforms[4]); } else { CreateQuestionMark(FivePanelTransforms[4]); }
                break;
        }
        
        DisplayedPanel = NormalizePanel(DisplayedPanel);
        DisplayedPanel.gameObject.SetActive(true); 
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
        foreach (Transform t in BeginActionButtons)
        {
            t.gameObject.SetActive(false);
        }
    }


    private static void CloseAndExecutePanels()
    {
        if (IsHintPanelOpen())
        {
            CloseHintPanel();
            return;
        }
        
        if(IsActionPanelOpen())
        {
            ExecuteActionPanel();
            ActionGUI.DisableAllBeginButtons();
            return;
        }
         
        if(IsReturnPanelOpen())
        {
            ExecuteReturnPanel();
            return;

        }
    }
}
