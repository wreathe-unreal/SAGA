using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


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
    
    public Dictionary<ActionKey, ActionResult> ActionMap;
    
    ActionResult GetActionResult(string actionName, List<CardData> cardData)
    {
        foreach (ActionKey k in ActionMap.Keys)
        {
            if (k.Match(actionName, cardData, PlayerState.Instance.GetPlayerState()))
            {
                return ActionMap[k];
            }
        }
        
        return null;
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
        this.ActionMap = new Dictionary<ActionKey, ActionResult>();
        foreach (ActionData a in rcd.Actions)
        {
            a.ActionKey.ConvertSecondaryCardSpecifiers(); //converts all of the list of list of strings to a list of CardSpecifier objects
            this.ActionMap[a.ActionKey] = a.ActionResult; //add the processed action key and action result to the cardData map
        }
    }
    
    public string GetDeckType()
    {
        switch (this.Type)
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
                return this.Type;
        }
    }
}


