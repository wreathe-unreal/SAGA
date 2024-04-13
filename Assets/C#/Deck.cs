using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    private List<Vector3> StarshipPositions;
    private List<Vector3> CraftingPositions;
    private List<Vector3> ObjectPositions;
    private List<Vector3> CharacterPositions;
    private List<Vector3> FleetPositions;
    private List<Vector3> CargoPositions;
    private List<Vector3> HabitatPositions;
    private List<Vector3> SystemPositions;
    private List<Vector3> AmbitionPositions;
    private List<Vector3> CurrencyPositions;
    
    private List<Card> StarshipDeck;
    private List<Card> CraftingDeck;
    private List<Card> ObjectDeck;
    private List<Card> CharacterDeck;
    private List<Card> FleetDeck;
    private List<Card> CargoDeck;
    private List<Card> HabitatDeck;
    private List<Card> SystemDeck;
    private List<Card> AmbitionDeck;
    private List<Card> CurrencyDeck;

    public List<GameObject> CardDeck;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject CardPositions = GameObject.Find("CardPositions");
        if (CardPositions != null)
        {
            StarshipPositions = CollectPositions(CardPositions, "Starship");
            CraftingPositions= CollectPositions(CardPositions, "Crafting");
            ObjectPositions= CollectPositions(CardPositions, "Object");
            CharacterPositions= CollectPositions(CardPositions, "Character");
            FleetPositions= CollectPositions(CardPositions, "Fleet");
            CargoPositions= CollectPositions(CardPositions, "Cargo");
            HabitatPositions= CollectPositions(CardPositions, "Habitat");
            SystemPositions= CollectPositions(CardPositions, "System");
            AmbitionPositions= CollectPositions(CardPositions, "Ambition");
            CurrencyPositions= CollectPositions(CardPositions, "Currency");
        }
        else
        {
            Debug.LogError("CardPositions parent GameObject not found in the scene.");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void SetCardPositions(List<Card> deck, List<Vector3> positions)
    {
            for (int i = 0; i < positions.Count; i++)
            {
                if (i < deck.Count && deck[i].Position != positions[i])
                {
                    // Assign position to card
                    deck[i].Position = positions[i];
                }
            }

            // If there are more cards than positions, remove the extra cards
            if (deck.Count > positions.Count)
            {
                deck.RemoveRange(positions.Count, deck.Count - positions.Count);
            }
    }

    void UpdatePositions()
    {
        SetCardPositions(StarshipDeck, StarshipPositions);
        SetCardPositions(CraftingDeck, CraftingPositions);
        SetCardPositions(ObjectDeck, ObjectPositions);
        SetCardPositions(CharacterDeck, CharacterPositions);
        SetCardPositions(FleetDeck, FleetPositions);
        SetCardPositions(CargoDeck, CargoPositions);
        SetCardPositions(HabitatDeck, HabitatPositions);
        SetCardPositions(SystemDeck, SystemPositions);
        SetCardPositions(AmbitionDeck, AmbitionPositions);
        SetCardPositions(CurrencyDeck, CurrencyPositions);
    }
}
