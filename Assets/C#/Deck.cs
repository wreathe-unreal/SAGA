using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : IEnumerable<Card>
{
    public string DeckTypeGroupName;
    public List<Vector3> Positions;
    public List<Card> Cards = new List<Card>();

    
    
    // Start is called before the first frame update
    public Deck(string DeckTypeGroupName)
    {
        GameObject CardPositions = GameObject.Find("CardPositions");
        if (CardPositions != null)
        {
            Positions = CollectPositions(CardPositions, DeckTypeGroupName);
        }
        else
        {
            Debug.LogError("CardPositions parent GameObject not found in the scene.");
        }
        
    }
    
    List<Vector3> CollectPositions(GameObject parentObject, string groupName)
    {
        Transform groupTransform = parentObject.transform.Find(groupName);
        List<Vector3> positions = new List<Vector3>();

        if (groupTransform != null)
        {
            foreach (Transform child in groupTransform)
            {
                positions.Add(child.position);
            }
        }
        else
        {
            Debug.LogWarning($"Group not found: {groupName}");
        }

        return positions;
    }

    public void SetCardPositions()
    {
            for (int i = 0; i < Positions.Count; i++)
            {
                if (i < Cards.Count && Cards[i].Position != Positions[i])
                {
                    // Assign position to card
                    Cards[i].Position = Positions[i];
                }
            }

            // If there are more cards than positions, remove the extra cards
            if (Cards.Count > Positions.Count)
            {
                Cards.RemoveRange(Positions.Count, Cards.Count - Positions.Count);
            }
    }
    
    // Implementation of IEnumerable<Card>
    public IEnumerator<Card> GetEnumerator()
    {
        return Cards.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
