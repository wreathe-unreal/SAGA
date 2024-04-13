using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public CardDB Database;
    // Start is called before the first frame update
    void Start()
    {
        Database = new CardDB();
        
        foreach(CardData c in CardDB.Database)
        {
            print("ID:" + c.ID);
            print("Name: " + c.Name);
            print("ImagePath: " + c.ImagePath);
            print("FlavorText: " + c.FlavorText);
            print("System:" + c.System);
            print("Habitat: " + c.Habitat);
            print("Type: " + c.Type);
            print("Property:" + c.Property);
            print("Lifespan:" + c.Lifespan);
            print("Price: " + c.Price);
            print("Raw Action Count: " + c.Actions.Count);
            print("Action Count Transformed: " + c.ActionMap.Count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
