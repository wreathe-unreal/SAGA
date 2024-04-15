using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public CardData Data;
    public string ID;
    public Vector3 Position;
    public string Name;
    public string ImagePath;

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
        ImagePath = Data.ImagePath;
        GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Images/" + ImagePath);
        GetComponentInChildren<TextMeshProUGUI>().text = Name;

    }
    
    public void SetPosition(Vector3 vec3)
    {
        Position = vec3;
        gameObject.transform.position = Position;
    }
    
}
