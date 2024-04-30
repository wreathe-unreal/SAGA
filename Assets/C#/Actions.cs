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

    public string GetSpecifierText()
    {
        if(this.ID != "")
        {
            return this.ID;
        }
        if(this.Type != "" && this.Property == "")
        {
            return this.Type;
        }
        if(this.Type != "" && this.Property != "")
        {
            return this.Property + " " + this.Type;
        }
        else
        {
            return this.Property;
        }
    }
    
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
    public int MinRepetitions;
    public int MaxRepetitions;
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
        SecondaryCardSpecifiers = new List<List<string>>(); //clear the string placeholders now that they are deserialized
    }

    public bool HasKeyMatch(string actionName, List<CardData> cardData, Player player, ActionData ad)
    {
         if (actionName != ActionName)
         {
             return false;
         }
        
        if (this.ReqLocation != "" && player.Location.IsInLocation(this.ReqLocation))
        {
            return false;
        }
        
        if (player.AttributeMap[Attribute] < this.AttributeMinimum)
        {
            return false;
        }


        if (cardData[0].ID != ID)
        {
            return false;
        }

        if ( player.GetActionRepetition(actionName, cardData[0]) < MinRepetitions || player.GetActionRepetition(actionName, cardData[0]) > MaxRepetitions)
        {
            return false;
        }
        
        if(SecondaryCardSpecifiersReal.Count != (cardData.Count - 1))
        {
            return false;
        }
        
        for (int i = 1; i <= SecondaryCardSpecifiersReal.Count; i++)
        {
            //Debug.Log(SecondaryCardSpecifiersReal[i - 1].GetSpecifierText() + " vs " + new CardSpecifier(cardData[i].ID, cardData[i].Type, cardData[i].Property).GetSpecifierText());
            //Debug.Log(SecondaryCardSpecifiersReal[i-1].MatchCard(cardData[i]));
            if (!SecondaryCardSpecifiersReal[i-1].MatchCard(cardData[i]))
            { 
                
                return false;
            }
        }

        return true;
    }
    
    public bool IsKeyHint(string actionName, List<CardData> cardData, Player player, ActionData ad)
    {
        if (actionName != ActionName)
        {
            return false;
        }
        
        if (this.ReqLocation != "" && player.Location.IsInLocation(this.ReqLocation))
        {
            return false;
        }
        
        if (player.AttributeMap[Attribute] < this.AttributeMinimum)
        {
            return false;
        }

        if (cardData[0].ID != ID)
        {
            return false;
        }

        if ( player.GetActionRepetition(actionName, cardData[0]) < MinRepetitions || player.GetActionRepetition(actionName, cardData[0]) > MaxRepetitions)
        {
            return false;
        }
        
        if(SecondaryCardSpecifiersReal.Count == 0 || SecondaryCardSpecifiersReal.Count <= cardData.Count - 1)
        {
            return false;
        }

        return true;
    }
    

    //constructor for making actionkeys to check for matches
    public ActionKey(string ActionName, string ID, string ReqLocation, List<CardSpecifier> SecondaryCardSpecifiers)
    {
        
        this.ActionName = ActionName;
        this.ID = ID;
        this.ReqLocation = ReqLocation;
        this.SecondaryCardSpecifiersReal = SecondaryCardSpecifiers;
        this.Attribute = "";
        this.AttributeMinimum = 0;
        this.SecondaryCardSpecifiers = new List<List<string>>();
    }

public ActionKey()
    {
        
    }
}

public class ActionResult
{
    public string Title;
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

    Dictionary<string, int> Execute()
    {
        Player.State.AttributeMap[AttributeModified] += AttributeModifier;
        Dictionary<string, int> ReturnedCards = new Dictionary<string, int>();
        
        for(int i =0; i < ReturnedCardIDs.Count; i++)
        {
            ReturnedCards[ReturnedCardIDs[i]] = ReturnedQuantities[i];
        }

        return ReturnedCards;
    }
}


// public class ActionMap
// {
//     public List<ActionKey> ActionKeys;
//     public List<ActionResult> ActionResults;
//
//     public int Match(string actionName, List<CardData> cardData, PlayerState player)
//     {
//         for (int i = 0; i < ActionKeys.Count; i++)
//         {
//             if (actionName != ActionKeys[i].ActionName && player.Location != ActionKeys[i].ReqLocation)
//             {
//                 continue;
//             }
//
//             if (ActionKeys[i].AttributeMinimum != 0 && player.AttributeMap[ActionKeys[i].Attribute] <= ActionKeys[i].AttributeMinimum)
//             {
//                 continue;
//             }
//
//
//             if (cardData[0].ID != ActionKeys[i].ID)
//             {
//                 continue;
//             }
//
//             bool bSecondariesMatch = true;
//             for (int j = 1; j < ActionKeys[i].SecondaryCardSpecifiersReal.Count; j++)
//             {
//                 if (!ActionKeys[i].SecondaryCardSpecifiersReal[j].MatchCard(cardData[j]))
//                 {
//                     bSecondariesMatch = false;
//                     break;
//                 }
//             }
//
//             if (!bSecondariesMatch)
//             {
//                 continue;
//             }
//
//             return i;
//         }
//
//         return -1;
//     }
//     public ActionResult Lookup(string actionName, List<CardData> cardData, PlayerState player)
//     {
//         int index = Match(actionName, cardData, player);
//         if (index != -1)
//         {
//             return ActionResults[index];
//         }
//         else
//         {
//             return null;
//         }
//     }
// }