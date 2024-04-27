using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;


public class Card : MonoBehaviour
{
    public Image PieTimer;
    public MeshRenderer BorderMesh;
    public Timer Timer;
    public Color TypeColor;
    public Animator AnimController;
    public CardData Data;
    public string ID;
    public Vector3 Position;
    public string Name;
    public string ImagePath;
    public int Quantity;
    public TextMeshProUGUI TMP_Name;
    public TextMeshProUGUI TMP_Quantity;
    public ActionData CurrentActionData;
    public ActionData CurrentActionHint;
    private Coroutine ScaleCoroutine;
    private Coroutine PositionCoroutine;
    public string DeckType;
    public TextMeshProUGUI TMP_TopName;
    public TextMeshProUGUI TMP_MouseOverProperty;
    public TextMeshProUGUI TMP_MouseOverFlavor;
    public TextMeshProUGUI TMP_MouseOverName;
    public TextMeshProUGUI TMP_MouseOverFlavorLeft;
    public TextMeshProUGUI TMP_MouseOverNameLeft;
    public MeshRenderer LeftSmoke;
    public MeshRenderer RightSmoke;
    public MeshRenderer BottomSmoke;
    public Camera MainCamera;
    public Vector3 OriginalPosition;
    public bool bActionFinished;
    
    void Awake()
    {
        SetFaceUpState(false);
    }
    void Start()
    {
        Timer = gameObject.GetComponent<Timer>();
        DeckType = GetDeckType();
        Rigidbody RigidBody = gameObject.GetComponent<Rigidbody>();
        MainCamera = FindObjectOfType<Camera>();
        bActionFinished = false;


    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ActionGUI.IsActionPanelOpen() && !ActionGUI.IsReturnPanelOpen() &&!ActionGUI.IsHintPanelOpen())  // 0 is the left mouse button
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)  // Check if the hit object is this GameObject
                {
                    OnClicked(hit.transform.gameObject.GetComponent<Card>());
                }
            }
        }
    }

    private void OnClicked(Card c)
    {
        if (c.Timer.timeRemaining <= 0 && (c.CurrentActionData != null || c.CurrentActionHint != null))
        {
            Player.State.SetActionCard(c);
            Player.State.GetActionCard().OpenAction();
        }
        else
        {
            Terminal.AppendText(c.Name);
        }
    }

    public string GetDeckType()
    {
        switch (Data.Type)
        {
            case "Engine":
            case "Power Supply":
            case "Portal Drive":
            case "Cargo Hold":
            case "Energy Weapon":
            case "Thrusters":
            case "Kinetic Weapon":
            case "Shield Generator":
                return "Starship";
            case "Hull":
                return "Fleet";
            default:
                return Data.Type;
        }
    }
    
    public void SetFaceUpState(bool bIsFaceUp)
    {
            AnimController.SetBool("bIsFaceUp", bIsFaceUp);
    }

    public void Initialize(string cardID)
    {
        Data = CardDB.CardDataLookup[cardID];
        
        DeckType = GetDeckType();
        ID = cardID;
        Name = Data.Name;
        Quantity = 1;
        ImagePath = Data.ImagePath.Substring(0, Data.ImagePath.Length - 4);
        GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/" + ImagePath);
        

        SetCardColor();
        if (DeckType == "Action")
        {
            TMP_Name.text = Name;
            TMP_TopName.text = "";

        }
        else
        {
            TMP_Name.text = "";
            TMP_TopName.text = Name;
        }

        TMP_TopName.color = TypeColor;
        TMP_Name.color = TypeColor;

        BorderMesh.material.color = TypeColor;
    }

    public void SetCardColor()
    {
        switch (DeckType)
        {
            case "Crafting":
                TypeColor = new Color(51 / 255f, 204 / 255f, 255 / 255f);
                break;
            case "Object":
                TypeColor = new Color(0 / 255f, 255 / 255f, 153 / 255f);
                break;
            case "Character":
                TypeColor = new Color(0 / 255f, 0 / 255f, 204 / 255f);
                break;
            case "Fleet":
                TypeColor = new Color(255 / 255f, 255 / 255f, 0 / 255f);
                break;
            case "Cargo":
                TypeColor = new Color(153 / 255f, 102 / 255f, 51 / 255f);
                break;
            case "Habitat":
                TypeColor = new Color(153 / 255f, 51 / 255f, 255 / 255f);
                break;
            case "System":
                TypeColor = new Color(204 / 255f, 0 / 255f, 204 / 255f);
                break;
            case "Action":
                TypeColor = new Color(255 / 255f, 128 / 255f, 0 / 255f);
                break;
            case "Ambition":
                TypeColor = new Color(255 / 255f, 51 / 255f, 133 / 255f);
                break;
            case "Currency":
                TypeColor = new Color(0 / 255f, 102 / 255f, 0 / 255f);
                break;
            case "Starship":
                TypeColor = new Color(153/255, 255/255, 51/255);
                break;
            case "Quest":
                TypeColor = new Color(255 / 255f, 191 / 255f, 0 / 255f);
                break;
            case "Enemy":
                TypeColor = new Color(255/255f, 0, 0);
                break;
            case "EndState":
                TypeColor = new Color(1f, 1f, 1f);     
                break;
            default:
                TypeColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
                break;
        }
    }

    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
        OriginalPosition = newPos;
        RevertPosition();
    }

    public void ModifyQuantity(int modifier)
    {
        Quantity = Mathf.Clamp(Quantity + modifier, 0, Int32.MaxValue);

        if (Quantity > 1)
        {
            TMP_Quantity.enabled = true;
        }

        if (Quantity == 0)
        {
            TMP_Quantity.enabled = false;
        }

        TMP_Quantity.text = $"{Quantity}";
    }

    public void CookActionResult()
    {
        gameObject.transform.SetParent(null);
        gameObject.transform.localScale = new Vector3(5, 5, 1);
        StartCoroutine(ScaleToSize(new Vector3(1, 1, 1), .40f));
        Board.Decks["Action"].SetCardPositions();
        
        Timer.StartTimer(CurrentActionData.ActionResult.Duration);
        Timer.OnTimerUpdate += UpdateTimerBar;
        Timer.OnTimerComplete += ActionFinished;

    }

    private void ActionFinished()
    {
        TMP_Name.color = new Color(191/255, 1, 0);
        Timer.timerText.text = "";
        bActionFinished = true;
    }

    private void UpdateTimerBar()
    {
        PieTimer.fillAmount = 1 - (Timer.timeRemaining / Timer.duration);

    }

    public void OpenAction()
    {
        print("before");
        if (IsTimerFinished() && CurrentActionData.ActionResult != null)
        {
            ActionGUI.Instance.DisplayReturnPanel(this);
            print("after display");
            Timer.timerText.faceColor = new Color32(255, 255, 255, 255);
            TMP_Name.color = TypeColor;
            PieTimer.fillAmount = 0;
            CurrentActionData = null;
            Player.State.NullActionCard();
            //handle quantity xd
        }
        print("after");
    }



    public bool IsTimerFinished()
    {
        return Timer.timeRemaining <= 0;
    }

    void OnMouseOver()
    {
    }

    void OnMouseExit()
    {

    }

    public void StartMoving(Vector3 newPos, float Duration)
    {

        if (this != null && this.PositionCoroutine != null)
        {
            StopCoroutine(this.PositionCoroutine);
        }
        
        this.PositionCoroutine = StartCoroutine(this.MoveToPosition(newPos, Duration));  // Lerp position back to original

    }
    public void StartScaling(Vector3 targetScale, float Duration)
    {
        if (this != null && this.ScaleCoroutine != null)
        {
            StopCoroutine(this.ScaleCoroutine);
        }
        
        this.ScaleCoroutine = StartCoroutine(this.ScaleToSize(targetScale, Duration));
    }

    public void RevertPosition()
    {

        if (this != null && this.PositionCoroutine != null)
        {
            StopCoroutine(this.PositionCoroutine);
        }
        this.PositionCoroutine = StartCoroutine(this.MoveToPosition(this.OriginalPosition, .15f));  // Lerp position back to original

    }
    public void RevertScaling()
    {
        
        if (this != null && this.ScaleCoroutine != null)
        {
            StopCoroutine(this.ScaleCoroutine);
        }

        this.ScaleCoroutine = StartCoroutine(this.ScaleToSize(Vector3.one, .15f));
    }

    
    private IEnumerator ScaleToSize(Vector3 targetScale, float duration)
    {
        float time = 0;
        Vector3 startScale = transform.localScale;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Pow(time / duration, 2));
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure the target scale is set after interpolation
    }
    
    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.localPosition;
        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetPosition;  // Ensure the position is exactly the target position at the end.
    }

    
}

public class CardData
{
    public int Lifespan;
    public string ID;
    public string Name;
    public int Price;
    public string ImagePath;
    public string FlavorText;
    public string System;
    public string Habitat;
    public string Type;
    public string Property;
    public List<ActionData> Actions;
    public string DeckType;
    
    public List<ActionData> FindActionData(string actionName, List<CardData> cardData)
    {
        List<ActionData> MatchHint = new List<ActionData>();
        MatchHint.Add(null);  // Match
        MatchHint.Add(null);  // Hint
        
        foreach (ActionData ad in Actions)
        {
            //Debug.Log(ad.ActionResult.Title + " Card Specifiers Count: " + ad.ActionKey.SecondaryCardSpecifiersReal.Count);
            if (ad.ActionKey.HasKeyMatch(actionName, cardData, Player.State, ad))
            {
                MatchHint[0] = ad;
            }
            else if(ad.ActionKey.IsKeyHint(actionName, cardData, Player.State, ad))
            {
                MatchHint[1] = ad;
            }
        }

        return MatchHint;
    }
    
    

    public CardData(RawCardData rcd)
    {
        Lifespan = rcd.Lifespan;
        ID = rcd.ID;
        Name = rcd.Name;
        Price = rcd.Price;
        ImagePath = rcd.ImagePath;
        FlavorText = rcd.FlavorText;
        System = rcd.System;
        Habitat = rcd.Habitat;
        Type = rcd.Type;
        Property = rcd.Property;
        Actions = rcd.Actions;
        Actions.Sort((x, y) => y.ActionKey.AttributeMinimum.CompareTo(x.ActionKey.AttributeMinimum));  // Sorting by age in ascending order
        
        
        
        
        foreach (ActionData a in Actions)
        {
            a.ActionKey.ConvertSecondaryCardSpecifiers(); //converts all of the list of list of strings to a list of CardSpecifier objects
        }

        // foreach (ActionData a in Actions)
        // {
        //     Debug.Log(a.ActionResult.Title + " Card Specifiers Count: " + a.ActionKey.SecondaryCardSpecifiersReal.Count);
        //
        // }
        
        DeckType = GetDeckType();
    }
    
    private string GetDeckType()
    {
        switch (this.Type)
        { 
            case "Engine":
            case "Power Supply":
            case "Portal Drive":
            case "Cargo Hold":
            case "Energy Weapon":
            case "Kinetic Weapon":
            case "Shield Generator":
            case "Thrusters":
                return "Starship";
            case "Hull":
            case "Ally":
                return "Fleet";
            default:
                return this.Type;
        }
    }
}

public class RawCardData
{
    public int Lifespan;
    public string ID;
    public string Name;
    public int Price;
    public string ImagePath;
    public string FlavorText;
    public string System;
    public string Habitat;
    public string Type;
    public string Property;
    public List<ActionData> Actions;
}