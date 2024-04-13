using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Newtonsoft.Json;

public class ActionData
{
    public ActionKey ActionKey;
    public ActionResult ActionResult;

    public ActionData()
    {
        
    }
}

public class CardSpecifier
{
    public string ID;
    public string Type;
    public string Property;
    
    public bool MatchCard(CardData cardData)
    {
        if(this.ID != "")
        {
            if(cardData.ID != this.ID)
            {
                return false;
            }
        }
        if(this.Type != "")
        {
            if(cardData.Type != this.Type)
            {
                return false;
            }
        }
        if(this.Property != "")
        {
            if(cardData.Property != this.Property)
            {
                return false;
            }
        }

        return true;
    }

    public CardSpecifier(string ID, string Type, string Property)
    {
        this.ID = ID;
        this.Type = Type;
        this.Property = Property;
    }
}

public class ActionKey
{
    public string ActionName;
    public string Attribute;
    public int AttributeMinimum;
    public string ID;
    public string ReqLocation;
    public List<List<string>> SecondaryCardSpecifiers;
    public List<CardSpecifier> SecondaryCardSpecifiersReal;

    
    //must be called on every actionkey after serialization
    public void ConvertSecondaryCardSpecifiers()
    {
        SecondaryCardSpecifiersReal = new List<CardSpecifier>();
        foreach (List<string> sl in SecondaryCardSpecifiers)
        {
            SecondaryCardSpecifiersReal.Add(new CardSpecifier(sl[0], sl[1], sl[2]));
        }
    }
    
    public bool Match(string actionName, List<CardData> cardData, PlayerState player)
    {
        if (actionName != ActionName && player.Location != this.ReqLocation)
        {
            return false;
        }
        
        switch (this.Attribute)
        {
            case "Valor":
                return player.Attribute.Valor <= this.AttributeMinimum;
            case "Intrepidity":
                return player.Attribute.Intrepidity <= this.AttributeMinimum;
            case "Infamy":
                return player.Attribute.Infamy <= this.AttributeMinimum;
        }

        if (cardData[0].ID != ID)
        {
            return false;
        }

        for (int i = 1; i < SecondaryCardSpecifiersReal.Count; i++)
        {
            if (!SecondaryCardSpecifiersReal[i].MatchCard(cardData[i]))
            {
                return false;
            }
        }

        return true;
    }

    public ActionKey()
    {
        
    }
}
public class ActionResult
{
    public string AttributeModified;
    public float AttributeModifier;
    public int Duration;
    public string FlavorText;
    public string OutcomeText;
    public List<string> ReturnedCardIDs;
    public List<int> ReturnedQuantities;

    public ActionResult()
    {
        
    }
}