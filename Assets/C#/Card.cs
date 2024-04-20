using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class Card : MonoBehaviour
{
    public Image PieTimer;
    public MeshRenderer BorderMesh;
    public Timer Timer;
    private Color TypeColor;
    public Animator AnimController;
    public CardData Data;
    public string ID;
    public Vector3 Position;
    public string Name;
    public string ImagePath;
    public int Quantity;
    public TextMeshProUGUI TMP_Name;
    public TextMeshProUGUI TMP_Quantity;
    public ActionResult CurrentActionResult; //for verb cards only
    private Vector3 OriginalScale;
    private Coroutine ScaleCoroutine;
    public string DeckType;
    public TextMeshProUGUI TMP_MouseOverFlavor;
    public TextMeshProUGUI TMP_MouseOverName;
    public TextMeshProUGUI TMP_MouseOverFlavorLeft;
    public TextMeshProUGUI TMP_MouseOverNameLeft;
    public Camera MainCamera;

    void Awake()
    {
        SetFaceUpState(false);
    }
    void Start()
    {
        DeckType = GetDeckType();
        Rigidbody RigidBody = gameObject.GetComponent<Rigidbody>();
        MainCamera = FindObjectOfType<Camera>();

    }

    void Update()
    {
    }

    private string GetDeckType()
    {
        switch (Data.Type)
        {
            case "Engine":
                return "Spaceship";
            case "Power Supply":
                return "Spaceship";
            case "Portal Drive":
                return "Spaceship";
            case "Cargo Hold":
                return "Spaceship";
            case "Thermal Weapon":
                return "Spaceship";
            case "Laser Weapon":
                return "Spaceship";
            case "Psychic Weapon":
                return "Spaceship";
            case "Arcane Weapon":
                return "Spaceship";
            case "Ramming Weapon":
                return "Spaceship";
            case "Kinetic Weapon":
                return "Spaceship";
            case "Shield Generator":
                return "Spaceship";
            case "Engine Coolant":
                return "Spaceship";
            case "Armor Plating":
                return "Spaceship";
            case "Magical Ward":
                return "Spaceship";
            case "Hull":
                return "Spaceship";
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
        TMP_Name.text = Name;
        Quantity = 1;
        ImagePath = Data.ImagePath.Substring(0, Data.ImagePath.Length - 4);
        GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/" + ImagePath);
        

        SetCardColor();
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
            default:
                TypeColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
                break;
        }
    }

    public void SetPosition(Vector3 newPos)
    {
        
        transform.position = newPos;

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
        BoardState.Decks["Action"].SetCardPositions();
        Timer.StartTimer(CurrentActionResult.Duration);
        Timer.OnTimerComplete += UpdateTimerText;
        Timer.OnTimerUpdate += UpdateTimerBar;

    }

    private void UpdateTimerBar()
    {
        PieTimer.fillAmount = 1 - (Timer.timeRemaining / Timer.duration);

    }
    
    private void UpdateTimerText()
    {
        Timer.timerText.faceColor = new Color32(153, 255, 51, 255);
        Timer.timerText.text = "Done";
    }

    public void OpenAction()
    {
        if (IsTimerFinished() && CurrentActionResult != null)
        {
            ActionGUI.DisplayReturnPanel(this);
            Timer.timerText.text = "";
            Timer.timerText.faceColor = new Color32(255, 255, 255, 255);
            PieTimer.fillAmount = 0;
            CurrentActionResult = null;
            
            
            
            //handle quantity xd
        }
    }



    public bool IsTimerFinished()
    {
        return Timer.timeRemaining <= 0;
    }

    void OnMouseOver()
    {
        if (ScaleCoroutine != null)
            StopCoroutine(ScaleCoroutine);
        
        Vector3 targetScale = new Vector3(2.25f, 2.25f, 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale * (MainCamera.orthographicSize / 157), Time.deltaTime * 10);

        Vector3 originalPosition = transform.localPosition;
        originalPosition.z = -5;
        transform.localPosition = originalPosition;
        
        if (transform.localScale.x  > (1.0f * (MainCamera.orthographicSize / 157)) && transform.localRotation.y == 0)
        {
            if (this.GetDeckType() != "System" && this.GetDeckType() != "Character" && this.GetDeckType() != "Action")
            {
                TMP_MouseOverName.text = Name;
                TMP_MouseOverFlavor.text = CardDB.CardDataLookup[ID].FlavorText;
            }
            else
            {
                TMP_MouseOverNameLeft.text = Name;
                TMP_MouseOverFlavorLeft.text = CardDB.CardDataLookup[ID].FlavorText;
            }
        }
    }

    void OnMouseExit()
    {
        TMP_MouseOverName.text = "";
        TMP_MouseOverFlavor.text = "";
        TMP_MouseOverNameLeft.text = "";
        TMP_MouseOverFlavorLeft.text = "";

        Vector3 originalPosition = transform.localPosition;
        originalPosition.z = 0;
        transform.localPosition = originalPosition;
        
        if (ScaleCoroutine != null)
        {
            StopCoroutine(ScaleCoroutine);
        }
        
        ScaleCoroutine = StartCoroutine(ScaleToSize(new Vector3(1f, 1f, 1f), .15f));
    }

    public IEnumerator ScaleToSize(Vector3 targetScale, float duration)
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
    
}