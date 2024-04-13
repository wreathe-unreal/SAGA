using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardData Data;
    public string ID;
    public Vector3 Position;
    public string Name;

    Card(string ID)
    {
        Data = CardDB.CardDataLookup[ID];
        ID = Data.ID;
        Name = Data.Name;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
