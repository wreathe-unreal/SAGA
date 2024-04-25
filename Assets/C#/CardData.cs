using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


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
            case "Hull":
            case "Thrusters":
                return "Starship";
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