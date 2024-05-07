using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BinaryCharm.TextMeshProAlpha;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum EPanelState
{
    Inactive,
    Action,
    Return,
    Hint,
    TimePasses,
    EndState
}

public class ActionGUI : MonoBehaviour
{
    public static TMP_InputField TextInput;

    private TextFade TextFader;
    private static float LastActionTime;
    private static float CooldownDuration;
    private static bool bFirstCoro;

    //instance
    public static ActionGUI Instance; // Singleton instance
    public static Camera MainCamera;
    public static MeshRenderer MeshRenderer;
    public static TMP_Text FlavorText;
    private static Board _board;
    private static AudioSource AudioSource;
    private static TMP_Text TitleText;
    private static Transform OriginalTransform;
    private static Transform DisplayedPanel;
    private static List<Transform> BeginActionButtons;
    public static EPanelState PanelState;
    public Terminal TerminalRef;
    public static bool bTextIsBeingPrinted = false;

    
    //inspector values
    public Transform OnePanel;
    public Transform OnePanel_OnlyCard;
    public Transform TwoPanel;
    public Transform TwoPanel_LeftCard;
    public Transform TwoPanel_RightCard;
    public Transform ThreePanel;
    public Transform ThreePanel_LeftCard;
    public Transform ThreePanel_MiddleCard;
    public Transform ThreePanel_RightCard;
    public TMP_Text ThreePanel_RightHint;
    public Transform FourPanel;
    public Transform FourPanel_TopCard;
    public Transform FourPanel_LeftCard;
    public Transform FourPanel_RightCard;
    public Transform FourPanel_BottomCard;
    public TMP_Text FourPanel_RightHint;
    public TMP_Text FourPanel_BottomHint;
    public Transform FivePanel;
    public Transform FivePanel_TopCard;
    public Transform FivePanel_LeftCard;
    public Transform FivePanel_RightCard;
    public Transform FivePanel_BottomLeftCard;
    public Transform FivePanel_BottomRightCard;
    public TMP_Text FivePanel_RightHint;
    public TMP_Text FivePanel_BottomLeftHint;
    public TMP_Text FivePanel_BottomRightHint;
    public GameObject EndGamePanel;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PanelState = EPanelState.Inactive;
        CooldownDuration = .5f;
        bFirstCoro = true;
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        TextInput = FindObjectOfType<TMP_InputField>();
        _board = FindObjectOfType<Board>();

        BeginActionButtons = new List<Transform>();
        BeginActionButtons.Add(Instance.transform.FindDeepChild("2Panel/Canvas/ActionButton"));
        BeginActionButtons.Add(Instance.transform.FindDeepChild("3Panel/Canvas/ActionButton"));
        BeginActionButtons.Add(Instance.transform.FindDeepChild("4Panel/Canvas/ActionButton"));
        BeginActionButtons.Add(Instance.transform.FindDeepChild("5Panel/Canvas/ActionButton"));
        TerminalRef = FindObjectOfType<Terminal>();
        TextFader = GetComponent<TextFade>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale >= 1f || Board.State.GetGameState() == EGameState.Unpaused)
        {
            if (ActionGUI.PanelState != EPanelState.EndState)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    StartInputCoroutine();
                }

                if (Input.GetKeyDown(KeyCode.Escape) && ActionGUI.IsActionPanelOpen())
                {
                    ActionGUI.Instance.CancelActionPanel();
                    Terminal.TMP_Input.interactable = true;
                }
            }
        }

        TimePasses();
    }

    public void xButtonClicked()
    {
        switch (PanelState)
        {
            case EPanelState.Action:
                CancelActionPanel();
                break;
            case EPanelState.Return:
                ExecuteReturnPanel();

                break;
            case EPanelState.Hint:
                CloseHintPanel();
                break;
            case EPanelState.TimePasses:
                CloseTimePassesPanel();
                break;
            default:
                StartCoroutine(HandleUserInput());
                break;
        }

        Terminal.SetText("> Action + Data", true);
    }

    private void CloseTimePassesPanel()
    {
        Sound.Manager.PlayTickTock();
        SetPanelActive(false);
        foreach (Card c in Board.Decks["Currency"])
        {
            if (c.ID == "gold" && c.Quantity > 0)
            {

                c.TMP_Quantity.color = new Color(1f, 1f, 1f);
                c.TMP_Quantity.text = $"{c.Quantity}";
                Board.State.ResetCardPositionAndList(new List<Card> { c });
                Board.DestroyCard(c);
            }
        }

        PanelState = EPanelState.Inactive;
    }

    public void TimePasses()
    {
        if (Board.State.GoldCard.GoldTimer.fillAmount < 1f)
        {
            Board.State.GoldCard.GoldTimer.fillAmount += Time.deltaTime / 120;
            return;
        }

        if (Board.State.GoldCard.GoldTimer.fillAmount >= 1f && PanelState == EPanelState.Inactive)
        {
            if (Board.State.GoldCard.Quantity <= 0)
            {
                Card bankruptCard = Instantiate<Card>(Board.State.CardToAdd);
                bankruptCard.Initialize("bankrupt");
                ActionGUI.Instance.DisplayEndGame(bankruptCard);
                return;
            }

            ActionGUI.Instance.DisplayTimePassesPanel(Board.State.GoldCard);
            Board.State.GoldCard.GoldTimer.fillAmount = 0f;
            return;
        }
    }

    public void StartInputCoroutine()
    {
        if (Board.State.GetGameState() == EGameState.Paused)
        {
            return;
        }

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<Card>() != null) // Check if the hit object is this GameObject
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

        if (bTextIsBeingPrinted == false)
        {
            if (CloseAndExecutePanels()) //if we close a panel we leave
            {
                yield break;
            }
        }
        else
        {
            yield break;
        }


        TextInput.interactable = false;
        Terminal.ParseText();

        Terminal.SetText("> Action + Card(s)", true);

        if (Terminal.ParsedText.Length > 5)
        {
            Terminal.SetText("TOO MANY CARDS.", true);
            Sound.Manager.PlayError();
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
            Sound.Manager.PlayError();
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
            Sound.Manager.PlayError();
            yield break;
        }

        //TRY TO FIND AN ACTION
        Player.State.FindAction(Terminal.ParsedText);
        //TRY TO FIND AN ACTION


        if (Player.State.GetActionCard().CurrentActionData != null)
        {
            DisplayActionPanel();
            Sound.Manager.PlayPlaceCards();
            yield break;
        }

        if (Player.State.GetActionCard().CurrentActionHint != null)
        {
            DisplayHintPanel();
            yield break;
        }

        Terminal.SetText("IMPOSSIBLE.", true);
        Sound.Manager.PlayError();
        yield break;
    }

    public void CancelActionPanel()
    {
        DisableAllBeginButtons();

        Sound.Manager.PlayError();

        switch (Player.State.InputCards.Count)
        {
            case 0:
                Sound.Manager.Play1Flip();
                break;
            case 1:
                Sound.Manager.Play2Flip();
                break;
            case 2:
                Sound.Manager.Play3Flip();
                break;
            case 3:
                Sound.Manager.Play4Flip();
                break;
            case 4:
                Sound.Manager.Play4Flip();
                break;
        }


        Player.State.GetActionCard().transform.SetParent(null);
        Player.State.GetActionCard().CurrentActionData = null;
        Player.State.GetActionCard().CurrentActionHint = null;
        Player.State.GetActionCard().RevertScaling();
        Player.State.NullActionCard();
        Board.State.ResetCardPositionAndList(Player.State.InputCards);

        SetPanelActive(false);

        Player.State.InputCards = new List<Card>(); // Clear other cards
        Terminal.SetText("> Action + Data", true);
    }

    public void ExecuteReturnPanel()
    {
        Sound.Manager.PlayPlaceCards();
        if (EndGamePanel.activeSelf == false)
        {
            Board.State.ResetCardPositionAndList(Player.State.ReturnedCards);
        }

        SetPanelActive(false);

        Player.State.GetLocation().UpdateCardGlow();
        if (Board.State.GetStarship().CurrentHealth <= 0)
        {
            Card deathCard = Instantiate<Card>(Board.State.CardToAdd);
            deathCard.Initialize("death");
            DisplayEndGame(deathCard);
        }
    }

    public static bool IsActionPanelOpen()
    {
        return PanelState == EPanelState.Action;

    }

    public static bool IsHintPanelOpen()
    {
        return PanelState == EPanelState.Hint;
    }

    public static bool IsReturnPanelOpen()
    {
        return PanelState == EPanelState.Return;

    }

    public void DisplayActionPanel()
    {
        Sound.Manager.PlayTransmissionSent();
        Sound.Manager.Play1Flip();

        PanelState = EPanelState.Action;

        SetPanelActive(true);


        switch (Player.State.InputCards.Count)
        {
            case 1:
                Player.State.GetActionCard().Reparent(TwoPanel_LeftCard);
                Player.State.InputCards[0].Reparent(TwoPanel_RightCard);
                Instance.transform.FindDeepChild("2Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 2:
                Player.State.GetActionCard().Reparent(ThreePanel_LeftCard);
                Player.State.InputCards[0].Reparent(ThreePanel_MiddleCard);
                Player.State.InputCards[1].Reparent(ThreePanel_RightCard);
                Instance.transform.FindDeepChild("3Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 3:
                Player.State.GetActionCard().Reparent(FourPanel_TopCard);
                Player.State.InputCards[0].Reparent(FourPanel_LeftCard);
                Player.State.InputCards[1].Reparent(FourPanel_RightCard);
                Player.State.InputCards[2].Reparent(FourPanel_BottomCard);
                Instance.transform.FindDeepChild("4Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
            case 4:
                Player.State.GetActionCard().Reparent(FivePanel_TopCard);
                Player.State.InputCards[0].Reparent(FivePanel_LeftCard);
                Player.State.InputCards[1].Reparent(FivePanel_RightCard);
                Player.State.InputCards[2].Reparent(FivePanel_BottomLeftCard);
                Player.State.InputCards[3].Reparent(FivePanel_BottomRightCard);
                Instance.transform.FindDeepChild("5Panel/Canvas/ActionButton").gameObject.SetActive(true);
                break;
        }



        TitleText.text = Player.State.GetActionCard().CurrentActionData.ActionResult.Title;
        FlavorText.text = Player.State.GetActionCard().CurrentActionData.ActionResult.FlavorText;
        TextFader.TriggerFade(FlavorText, OnTextFadeComplete);


    }

    public void DisplayReturnPanel(Card OpenedActionCard)
    {

        PanelState = EPanelState.Return;

        OpenedActionCard.bActionFinished = false;

        if (!HandleSpecialActions(OpenedActionCard)) //if it wasn't a special action card proceed normally
        {
            for (int i = 0; i < OpenedActionCard.CurrentActionData.ActionResult.ReturnedCardIDs.Count; i++)
            {
                string id = OpenedActionCard.CurrentActionData.ActionResult.ReturnedCardIDs[i];
                int qty = OpenedActionCard.CurrentActionData.ActionResult.ReturnedQuantities[i];

                Player.State.ReturnedCards.Add(Board.GetInstance().AddCard(id, qty, false));
            }
        }

        List<Card> Returned = Player.State.GetReturnedCards();

        SetPanelActive(true);


        TitleText.text = OpenedActionCard.CurrentActionData.ActionResult.Title;
        FlavorText.text = OpenedActionCard.CurrentActionData.ActionResult.OutcomeText;
        
        switch (Returned.Count)
        {
            case 1:
                Returned[0].Reparent(OnePanel_OnlyCard);

                break;
            case 2:
                Returned[0].Reparent(TwoPanel_LeftCard);
                Returned[1].Reparent(TwoPanel_RightCard);
                break;
            case 3:
                Returned[0].Reparent(ThreePanel_LeftCard);
                Returned[1].Reparent(ThreePanel_MiddleCard);
                Returned[2].Reparent(ThreePanel_RightCard);
                break;
            case 4:
                Returned[0].Reparent(FourPanel_TopCard);
                Returned[1].Reparent(FourPanel_LeftCard);
                Returned[2].Reparent(FourPanel_RightCard);
                Returned[3].Reparent(FourPanel_BottomCard);
                break;
            case 5:
                Returned[0].Reparent(FivePanel_TopCard);
                Returned[1].Reparent(FivePanel_LeftCard);
                Returned[2].Reparent(FivePanel_RightCard);
                Returned[3].Reparent(FivePanel_BottomLeftCard);
                Returned[4].Reparent(FivePanel_BottomRightCard);
                break;
        }

        TextFader.TriggerFade(FlavorText, OnTextFadeComplete);

        if (Player.State.GetActionCard().ID == "battle" && !Player.bLastBattleWasWin)
        {
            TitleText.text = "Defeat";
            FlavorText.text = "Our ship sustained severe damage from the battle.";
            return;
        }
    }


    public void PresentCards(List<Card> CardsToPresent)
    {
        if (ActionGUI.IsActionPanelOpen())
        {
            return;
        }

        foreach (Card c in CardsToPresent)
        {
            if (!c.AnimController.GetBool("bIsFaceUp"))
            {
                c.AnimController.Play("FaceDown");
                c.SetFaceUpState(true);
            }
        }
    }

    private bool HandleSpecialActions(Card OpenedActionCard)
    {
        if (OpenedActionCard.ID == "travel")
        {
            Player.State.HandleTravel(OpenedActionCard.CurrentActionData);
            return false;
        }

        if (OpenedActionCard.ID == "battle")
        {
            if (!Board.State.GetStarship()
                    .GetBattleResults(Player.State.GetBattleOpponent().Data
                        .Price)) //if the action is a battle and the player loses
            {
                Player.State.DecrementActionRepetition(OpenedActionCard.Name, Player.State.GetBattleOpponent().Data);
                Player.State.ReturnedCards.Add(Board.State.AddCard(OpenedActionCard.ID, 1, false));
                Player.State.ReturnedCards.Add(Board.State.AddCard(Player.State.GetBattleOpponent().ID, 1, false));
                Player.State.SetBattleOpponent(null);
                return true;
            }
        }

        return false;
    }

    public void DisplayEndGame(Card card)
    {
        PanelState = EPanelState.EndState;
        foreach (Deck d in Board.Decks.Values)
        {
            foreach (Card c in d.Cards)
            {
                if (c != card && c != null)
                {
                    c.gameObject.SetActive(false);
                }
            }
        }


        EndGamePanel.gameObject.SetActive(true);
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, MainCamera.nearClipPlane);
        Vector3 worldCenter = MainCamera.ScreenToWorldPoint(screenCenter);
        worldCenter.z = -29.6f;
        card.SetPosition(worldCenter);
        card.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
        Time.timeScale = 0.0f;
        card.SetFaceUpState(true);
        MainCamera.GetComponent<CameraController>().enabled = false;
        GameObject.Find("BGM").GetComponent<AudioSource>().Stop();
        GameObject.Find("HUD").SetActive(false);
        Sound.Manager.PlayIchRufZuDir();
    }

    public void SetPanelActive(bool bActive)
    {
        if (Instance == null)
        {
            Debug.LogError("Panel or ActionManager instance is not initialized!");
            return;
        }

        if (!bActive)
        {
            PanelState = EPanelState.Inactive;
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

    public Transform SetDisplayedPanel()
    {
        int cardsToDisplay;
        switch (PanelState)
        {
            case EPanelState.Action:
                cardsToDisplay = Player.State.InputCards.Count + 1;
                break;
            case EPanelState.Hint:
                cardsToDisplay = Player.State.GetActionCard().CurrentActionHint.ActionKey.SecondaryCardSpecifiersReal
                    .Count + 2;
                break;
            case EPanelState.Return:
                cardsToDisplay = Player.State.ReturnedCards.Count;
                break;
            case EPanelState.TimePasses:
                cardsToDisplay = 1;
                break;
            default:
                cardsToDisplay = 1;
                break;
        }

        switch (cardsToDisplay)
        {
            case 1:
                DisplayedPanel = OnePanel;
                TitleText = Instance.transform.FindDeepChild("1Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = Instance.transform.FindDeepChild("1Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = Instance.transform.Find("1Panel").GetComponent<AudioSource>();
                break;
            case 2:
                DisplayedPanel = TwoPanel;
                TitleText = Instance.transform.FindDeepChild("2Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = Instance.transform.FindDeepChild("2Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = Instance.transform.Find("2Panel").GetComponent<AudioSource>();
                break;
            case 3:
                DisplayedPanel = ThreePanel;
                TitleText = Instance.transform.FindDeepChild("3Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = Instance.transform.FindDeepChild("3Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = Instance.transform.Find("3Panel").GetComponent<AudioSource>();
                break;
            case 4:
                DisplayedPanel = FourPanel;
                TitleText = Instance.transform.FindDeepChild("4Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = Instance.transform.FindDeepChild("4Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = Instance.transform.Find("4Panel").GetComponent<AudioSource>();
                break;
            case 5:
                DisplayedPanel = FivePanel;
                TitleText = Instance.transform.FindDeepChild("5Panel/TitleText").GetComponent<TMP_Text>();
                FlavorText = Instance.transform.FindDeepChild("5Panel/FlavorText").GetComponent<TMP_Text>();
                AudioSource = Instance.transform.Find("5Panel").GetComponent<AudioSource>();
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



    public void ExecuteActionPanel()
    {
        if (bTextIsBeingPrinted) return;
        
        Player.State.ExecuteAction();
        SetPanelActive(false);
        Sound.Manager.PlayWhoosh();
    }



    public void CloseHintPanel()
    {
        Player.State.GetActionCard().Reparent(null);
        Player.State.GetActionCard().CurrentActionData = null;
        Player.State.GetActionCard().CurrentActionHint = null;
        Player.State.NullActionCard();

        foreach (Card c in Player.State.InputCards)
        {
            c.Reparent(null);
        }

        ThreePanel_RightHint.gameObject.SetActive(false);
        FourPanel_BottomHint.gameObject.SetActive(false);
        FourPanel_RightHint.gameObject.SetActive(false);
        FivePanel_BottomRightHint.gameObject.SetActive(false);
        FivePanel_BottomLeftHint.gameObject.SetActive(false);
        FivePanel_RightHint.gameObject.SetActive(false);

        switch (Player.State.InputCards.Count)
        {
            case 0:
                Sound.Manager.Play1Flip();
                break;
            case 1:
                Sound.Manager.Play2Flip();
                break;
            case 2:
                Sound.Manager.Play3Flip();
                break;
            case 3:
                Sound.Manager.Play4Flip();
                break;
        }

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

        SetPanelActive(false);

        foreach (Deck d in Board.Decks.Values)
        {
            d.SetCardPositions();
        }

        PanelState = EPanelState.Inactive;
    }

    public static SpriteRenderer CreateQuestionMark(Transform targetParent)
    {
        GameObject questionMarkObject = new GameObject("QuestionMark");
        SpriteRenderer questionMark = questionMarkObject.AddComponent<SpriteRenderer>();
        Sprite questionMarkSprite = Resources.Load<Sprite>("Images/QuestionMark");
        if (questionMarkSprite == null)
        {
            Debug.LogError("Failed to load QuestionMark sprite.");
            return null; // Exit early if the sprite couldn't be loaded.
        }

        questionMark.sprite = questionMarkSprite;

        questionMark.transform.SetParent(targetParent, false);
        questionMark.transform.localPosition = new Vector3(-0.1f, 2.7f, -1f);
        questionMark.transform.localRotation = Quaternion.identity;
        questionMark.transform.localScale = new Vector3(4.68f, 4.79f, 5.168f);
        questionMark.gameObject.layer = 3;

        return questionMark;
    }

    public void DisplayHintPanel()
    {

        Sound.Manager.PlayError();
        Sound.Manager.PlayPlaceCards();

        PanelState = EPanelState.Hint;

        List<Card> InputCards = Player.State.GetInputCards();

        List<CardSpecifier> SecondaryCardSpecifiers =
            Player.State.GetActionCard().CurrentActionHint.ActionKey.SecondaryCardSpecifiersReal;

        // foreach (Deck d in Board.Decks.Values)
        // {
        //     d.SetCardPositions();
        // }


        SetPanelActive(true);

        List<string> SecondaryCardHints = new List<string>();

        for (int i = 1; i <= SecondaryCardSpecifiers.Count; i++)
        {
            //Debug.Log(SecondaryCardSpecifiersReal[i - 1].GetSpecifierText() + " vs " + new CardSpecifier(cardData[i].ID, cardData[i].Type, cardData[i].Property).GetSpecifierText());
            //Debug.Log(SecondaryCardSpecifiersReal[i-1].MatchCard(cardData[i]));

            if (i < InputCards.Count && SecondaryCardSpecifiers[i - 1].MatchCard(InputCards[i].Data))
            {
                SecondaryCardHints.Add("correct");
            }
            else
            {
                SecondaryCardHints.Add(SecondaryCardSpecifiers[i - 1].GetSpecifierText());
            }
        }


        switch (SecondaryCardSpecifiers.Count)
        {
            case 1:
                DisplayedPanel = ThreePanel;
                Player.State.GetActionCard().Reparent(ThreePanel_LeftCard);
                InputCards[0].Reparent(ThreePanel_MiddleCard);
                if (SecondaryCardHints[0] != "correct")
                {
                    CreateQuestionMark(ThreePanel_RightCard);
                    ThreePanel_RightHint.gameObject.SetActive(true);
                    ActivateSetHintText(ThreePanel_RightHint.gameObject.GetComponent<TMP_Text>(),
                        SecondaryCardHints[0]);

                }
                else
                {
                    InputCards[1].Reparent(ThreePanel_RightCard);
                }

                break;
            case 2:
                DisplayedPanel = FourPanel;
                Player.State.GetActionCard().Reparent(FourPanel_TopCard);
                InputCards[0].Reparent(FourPanel_LeftCard);
                if (SecondaryCardHints[0] != "correct")
                {
                    CreateQuestionMark(FourPanel_RightCard);
                    FourPanel_RightHint.gameObject.SetActive(true);
                    ActivateSetHintText(FourPanel_RightHint.gameObject.GetComponent<TMP_Text>(), SecondaryCardHints[0]);

                }
                else
                {
                    InputCards[1].Reparent(FourPanel_RightCard);
                }

                if (SecondaryCardHints[1] != "correct")
                {
                    CreateQuestionMark(FourPanel_BottomCard);
                    FourPanel_BottomHint.gameObject.SetActive(true);
                    ActivateSetHintText(FourPanel_BottomHint.gameObject.GetComponent<TMP_Text>(),
                        SecondaryCardHints[1]);
                }
                else
                {
                    InputCards[2].Reparent(FourPanel_BottomCard);
                }

                break;
            case 3:
                DisplayedPanel = FivePanel;
                Player.State.GetActionCard().transform.SetParent(FivePanel_TopCard);
                InputCards[0].Reparent(FivePanel_LeftCard);
                if (SecondaryCardHints[0] != "correct")
                {
                    CreateQuestionMark(FivePanel_RightCard);
                    FivePanel_RightHint.gameObject.SetActive(true);
                    ActivateSetHintText(FivePanel_RightHint.gameObject.GetComponent<TMP_Text>(), SecondaryCardHints[0]);
                }
                else
                {
                    InputCards[1].Reparent(FivePanel_RightCard);
                }

                if (SecondaryCardHints[1] != "correct")
                {
                    CreateQuestionMark(FivePanel_BottomLeftCard);
                    FivePanel_BottomLeftHint.gameObject.SetActive(true);
                    ActivateSetHintText(FivePanel_BottomLeftHint.gameObject.GetComponent<TMP_Text>(),
                        SecondaryCardHints[1]);
                }
                else
                {
                    InputCards[2].Reparent(FivePanel_BottomLeftCard);
                }

                if (SecondaryCardHints[2] != "correct")
                {
                    CreateQuestionMark(FivePanel_BottomRightCard);
                    FivePanel_BottomRightHint.gameObject.SetActive(true);
                    ActivateSetHintText(FivePanel_BottomRightHint.gameObject.GetComponent<TMP_Text>(),
                        SecondaryCardHints[2]);
                }
                else
                {
                    InputCards[3].Reparent(FivePanel_BottomRightCard);
                }

                break;
        }




        foreach (Card c in InputCards)
        {
            c.transform.localPosition.Set(0f, 0f, 0f);
        }

        DisableAllBeginButtons();
        TitleText.text = "";
        FlavorText.text = "";

    }

    private static void ActivateSetHintText(TMP_Text textObject, string newText)
    {
        textObject.text = newText;
        textObject.gameObject.SetActive(true);
    }

    private static void DisableAllBeginButtons()
    {
        foreach (Transform t in BeginActionButtons)
        {
            t.gameObject.SetActive(false);
        }
    }


    private bool CloseAndExecutePanels()
    {
        if (IsTimePassesPanelOpen())
        {
            CloseTimePassesPanel();
            return true;
        }

        if (IsHintPanelOpen())
        {
            CloseHintPanel();
            ActionGUI.DisableAllBeginButtons();
            return true;
        }

        if (IsActionPanelOpen())
        {
            ExecuteActionPanel();
            ActionGUI.DisableAllBeginButtons();
            return true;
        }

        if (IsReturnPanelOpen())
        {
            ExecuteReturnPanel();
            return true;

        }

        return false;
    }

    public static bool IsTimePassesPanelOpen()
    {
        return PanelState == EPanelState.TimePasses;

    }

    public static bool AllPanelsAreClosed()
    {
        return !IsHintPanelOpen() && !IsActionPanelOpen() && !IsReturnPanelOpen() && !IsTimePassesPanelOpen() &&
               !IsEndScreenPanelOpen();
    }

    private static bool IsEndScreenPanelOpen()
    {
        return PanelState == EPanelState.EndState;
    }

    public void DisplayTimePassesPanel(Card goldCard)
    {
        PanelState = EPanelState.TimePasses;
        Sound.Manager.PlayChurchBell();
        Sound.Manager.Play1Flip();

        SetPanelActive(true);

        goldCard.Reparent(OnePanel_OnlyCard);
        goldCard.TMP_Quantity.enabled = true;
        goldCard.TMP_Quantity.color = new Color(1f, 0f, 0f);
        goldCard.TMP_Quantity.text = "-1";
        TitleText.text = "Time Passes";
        FlavorText.text =
            "Time slips like sand through clenched fists, seasons fade, the toll of life grows heavier and it's burdens carve deeper into the soul's weary map.";
        
        TextFader.TriggerFade(FlavorText, OnTextFadeComplete);
        bTextIsBeingPrinted = false; //allow the user to click out early


    }
    
    private void OnTextFadeComplete()
    {
        bTextIsBeingPrinted = false;
        if (IsReturnPanelOpen())
        {
            PresentCards(Player.State.GetReturnedCards());
        }
    }
}