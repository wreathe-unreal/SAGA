using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Timer Timer;
    private Color CardColor;
    public Animator AnimController;
    public CardData Data;
    public string ID;
    public Vector3 Position;
    public string Name;
    public string ImagePath;
    public int Quantity;
    public TextMeshProUGUI DisplayName;
    public TextMeshProUGUI DisplayQuantity;
    public ActionResult CurrentActionResult; //for verb cards only
    

    void Start()
    {
        AnimController = GetComponent<Animator>();

    }
    void Update()
    {
    }
    
    public string GetDeckType()
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

    public void Initialize(string cardID)
    {
        Data = CardDB.CardDataLookup[cardID];
        ID = cardID;
        Name = Data.Name;
        Quantity = 1;
        ImagePath = Data.ImagePath.Substring(0, Data.ImagePath.Length - 4);
        GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/" + ImagePath);
        DisplayName.text = Name;

        SetCardColor();
        DisplayName.color = CardColor;
        foreach (Transform child in transform)
        {
            // Check if the child's name is "Cube"
            if (child.name.Contains("Border"))
            {
                Renderer renderer = child.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Set the color of the material
                    renderer.material.color = CardColor;
                }
            }

        }
        
        
    }

    public void SetCardColor()
    {
        switch (GetDeckType())
        {
            case "Crafting":
                CardColor = new Color(51 / 255f, 204 / 255f, 255 / 255f);
                break;
            case "Object":
                CardColor = new Color(0 / 255f, 255 / 255f, 153 / 255f);
                break;
            case "Character":
                CardColor = new Color(0 / 255f, 0 / 255f, 204 / 255f);
                break;
            case "Fleet":
                CardColor = new Color(255 / 255f, 255 / 255f, 0 / 255f);
                break;
            case "Cargo":
                CardColor = new Color(153 / 255f, 102 / 255f, 51 / 255f);
                break;
            case "Habitat":
                CardColor = new Color(153 / 255f, 51 / 255f, 255 / 255f);
                break;
            case "System":
                CardColor = new Color(204 / 255f, 0 / 255f, 204 / 255f);
                break;
            case "Action":
                CardColor = new Color(255 / 255f, 128 / 255f, 0 / 255f);
                break;
            case "Ambition":
                CardColor = new Color(255 / 255f, 51 / 255f, 133 / 255f);
                break;
            case "Currency":
                CardColor = new Color(0 / 255f, 102 / 255f, 0 / 255f);
                break;
            default:
                CardColor = new Color(255 / 255f, 255 / 255f, 255 / 255f);
                break;
        }
    }
    public void SetPosition(Vector3 newPos)
    {
            this.transform.position = newPos;  // This should update the GameObject's world position
            gameObject.transform.position = newPos;

    }

    public void ModifyQuantity(int modifier)
    {
        Quantity = Mathf.Clamp(Quantity + modifier, 0, Int32.MaxValue);
        
        if (Quantity > 1)
        {
            DisplayQuantity.enabled = true;
        }

        if (Quantity == 0)
        {
            DisplayQuantity.enabled = false;
        }
        
        DisplayQuantity.text = $"{Quantity}";
    }

    void FlipCard()
    {
        AnimController.SetTrigger("CardFlipTrigger");
    }


    public void ExecuteActionResult()
    {
        gameObject.transform.SetParent(null);
        gameObject.transform.localScale = new Vector3(55, 55, 1);
        Board.Decks["Action"].SetCardPositions();
        Timer.StartTimer(CurrentActionResult.Duration);
        Timer.OnTimerComplete += OnTimerExpired;


    }

    private void OnTimerExpired()
    {
        Timer.timerText.text = "Done";
        
    }

    public void OpenAction()
    {
        if (IsTimerFinished() && CurrentActionResult != null)
        {
            Time.timeScale = 0.0f;
            Terminal.SetPanelActive(true);
            Terminal.Panel.transform.Find("FlavorText").GetComponent<TMP_Text>().text = CurrentActionResult.OutcomeText;
            

            foreach (string cardID in CurrentActionResult.ReturnedCardIDs)
            {
                Board.ReturnedCards.Add(Board.GetInstance().AddCard(cardID, false));
            }
            
            switch (Board.ReturnedCards.Count)
            {
                case 1:
                    Terminal.Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                    Board.ReturnedCards[0].transform.SetParent(Terminal.CardPos1);
                    break;
                case 2:
                    Terminal.Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                    Board.ReturnedCards[0].transform.SetParent(Terminal.CardPos1);
                    Board.ReturnedCards[1].transform.SetParent(Terminal.CardPos2);
                    break;
                case 3:
                    Terminal.Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/3Panel");
                    Board.ReturnedCards[0].transform.SetParent(Terminal.CardPos1);
                    Board.ReturnedCards[1].transform.SetParent(Terminal.CardPos2);
                    Board.ReturnedCards[2].transform.SetParent(Terminal.CardPos4);
                    break;
                case 4:
                    Terminal.Panel.transform.Find("FlavorText").localPosition = new Vector3(-0.181f, 0.679f, 0f);
                    Terminal.Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/4Panel");
                    Board.ReturnedCards[0].transform.SetParent(Terminal.CardPos1);
                    Board.ReturnedCards[1].transform.SetParent(Terminal.CardPos2);
                    Board.ReturnedCards[2].transform.SetParent(Terminal.CardPos4);
                    Board.ReturnedCards[3].transform.SetParent(Terminal.CardPos3);
                    break;
            }
            
            foreach (Card c in Board.ReturnedCards)
            {
             
                c.transform.localScale = new Vector3(1f, 1f, 1f);
                c.transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            

        }
        
        //handle quantity xd
        
        //flip them
        
        Timer.timerText.text = "";
        CurrentActionResult = null;
    }

    

    public bool IsTimerFinished()
    {
        return Timer.timeRemaining <= 0;
    }
}
