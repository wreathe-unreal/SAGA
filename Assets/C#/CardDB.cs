using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Newtonsoft.Json;

public class CardDB
{
    public static List<CardData> Database;
    public static Dictionary<string, CardData> CardDataLookup; //string is the ID

    public CardDB()
    {
        List <RawCardData> rawCardData = JsonDeserializer.DeserializeCardJson();
        Database = new List<CardData>();
        CardDataLookup = new Dictionary<string, CardData>();

        foreach (RawCardData rcd in rawCardData)
        {
            
            Database.Add(new CardData(rcd));
            CardDataLookup[rcd.ID] = new CardData(rcd);
        }
    }
    
    CardData GetCardData(string CardID)
    {
        return CardDataLookup[CardID];
    }
}

public class JsonDeserializer
{
    public static List<RawCardData> DeserializeCardJson()
    {
        
        // Load the JSON file from the Resources folder
        TextAsset jsonFile = Resources.Load<TextAsset>("cards");
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            // Now you can use jsonString to deserialize into your objects
            Debug.Log("Loaded JSON: " + jsonString);
            return JsonConvert.DeserializeObject<List<RawCardData>>(jsonString);

        }
        else
        {
            Debug.LogError("Unable to load the JSON file.");
            return new List<RawCardData>();
        }
    }
    public static Attribute DeserializeEnumJson()
    {
        
        // Load the JSON file from the Resources folder
        TextAsset jsonFile = Resources.Load<TextAsset>("enums");
        if (jsonFile != null)
        {
            string jsonString = jsonFile.text;
            // Now you can use jsonString to deserialize into your objects
            Debug.Log("Loaded JSON: " + jsonString);
            return JsonConvert.DeserializeObject<Attribute>(jsonString);

        }
        else
        {
            Debug.LogError("Unable to load the JSON file.");
            return new Attribute();
        }
    }
    
}