using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Terminal : MonoBehaviour
{
     public TMP_Text PanelText;
     public TMP_InputField TextInput;
     static public GameObject Panel;
     static private Board Board;
     static public Transform CardPos1;
     static public Transform CardPos2;
     static public Transform CardPos3;
     static public Transform CardPos4;
     static private Card ActionCard;
     static private List<Card> SupportingCards;
     private float LastActionTime;
     private float CooldownDuration;
     private bool bFirstCoro;

     public static void SetPanelActive(bool State)
     {
         if (State)
         {
             Panel.SetActive(true);
             Time.timeScale = 0.0f;
         }
         else
         {             
             Panel.SetActive(false);
             Time.timeScale = 1.0f;
         }
     }
     
     void Start()
     {
         // Find the Board component on any GameObject in the scene.
         Board = FindObjectOfType<Board>();
         Transform childTransform = Board.transform.Find("ActionPanel");
         Panel = childTransform.gameObject;
         CardPos1 = Panel.transform.Find("LeftCard");
         CardPos2 = Panel.transform.Find("RightCard");
         CardPos3 = Panel.transform.Find("TopCard");
         CardPos4 = Panel.transform.Find("BottomCard");
         CooldownDuration = .5f;
         bFirstCoro = true;


     }
     
     void Update()
     {
         // Listen for key down event only once in Update
         if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
         {
             
             if (Board.ReturnedCards.Count > 0)
             {
                 print("in if");
                 Board.ReturnAndClean();
             }
             
             StartCoroutine(HandleInput());
             bFirstCoro = false;
         }
     }

     IEnumerator HandleInput()
     {
         if (!bFirstCoro)
         {
             // Early exit if cooldown has not elapsed
             float timeSinceLastAction = Time.realtimeSinceStartup - LastActionTime;
             if (timeSinceLastAction <= CooldownDuration)
                 yield break;
         }

         // Update the last action time immediately
         LastActionTime = Time.realtimeSinceStartup;

         // Handling no action card scenario
         if (ActionCard == null)
         {
             
             if (TextInput.text != "> ACTION + CARD" && TextInput.text != "CARD(S) NOT FOUND."
                                                     && TextInput.text != "ACTION NOT FOUND." && TextInput.text != "")
             {
                 TextInput.interactable = false;
                 SupportingCards = new List<Card>(); // Clear other cards
                 ActionCard = null; // Clear action card
                 

                 ParseAndSearchForActionResult(TextInput.text); // Parse and search
                 TextInput.interactable = true;
             }
         }
         else
         {
             // Handle scenario where an action card is present and panel is active
             if (Panel != null && Panel.activeSelf && ActionCard.CurrentActionResult != null)
             {
                 List<Card> cardsToRemove = new List<Card>();

                 foreach (Card c in SupportingCards)
                 {
                     cardsToRemove.Add(c);
                 }

                 foreach (Card c in cardsToRemove)
                 {
                     Board.DestroyCard(c);
                     SupportingCards.Remove(c);
                 }

                 ActionCard.ExecuteActionResult();
                 SetPanelActive(false);
                 ActionCard = null;
             }
         }

         // Wait for the cooldown duration before allowing another action, unaffected by Time.timeScale
         yield return new WaitForSecondsRealtime(CooldownDuration);
     }

     public void ParseAndSearchForActionResult(string input)
     {
         string error = "> ACTION + CARD";
         var words = input.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
         
         if (words.Length > 4)
         {
             TextInput.text = error;
             return;
         }

         for (int i = 0; i < words.Length; i++)
         {
             words[i] = words[i].ToLower();
         }

         // Check if the first word matches an action card
         ActionCard = Board.Decks["Action"].FirstOrDefault(c => c.Name.ToLower() == words[0]);

         if (words.Length == 1 && ActionCard != null)
         {
             ActionCard.OpenAction();
         }
         
         if (ActionCard != null && words.Length >= 2)
         {
             bool allMatches = true; //set true to start

             // Check all other words
             for (int i = 1; i < words.Length; i++)
             {
                 string currentWord = words[i];
                 bool matchFound = false;

                 foreach (KeyValuePair<string, Deck> kvp in Board.Decks)
                 {
                     // Skip the Action deck for these checks
                     if (kvp.Key == "Action") continue;

                     Card card = kvp.Value.Cards.FirstOrDefault(c => c.Name.ToLower() == currentWord);
                     if (card != null)
                     {
                         matchFound = true;
                         SupportingCards.Add(card);
                         break;
                     }
                 }

                 if (!matchFound)
                 {
                     allMatches = false;
                     break;
                 }
             }

             if (allMatches)
             {
                 List<CardSpecifier> cardSpecifiers = new List<CardSpecifier>();
                 for (int i = 1; i < SupportingCards.Count; i++)
                 {
                     CardData cd = CardDB.CardDataLookup[SupportingCards[i].ID];
                     CardSpecifier cs = new CardSpecifier(cd.ID, cd.Type, cd.Property);
                     cardSpecifiers.Add(cs);
                 }
                 
                 ActionKey actionKeyToFind = new ActionKey(ActionCard.Name, SupportingCards[0].ID, PlayerState.Instance.Location, cardSpecifiers);

                 CardData mainCardData = CardDB.CardDataLookup[SupportingCards[0].ID];

                 List<CardData> cdList = new List<CardData>();
                 foreach (Card c in SupportingCards)
                 {
                     cdList.Add(CardDB.CardDataLookup[c.ID]);
                 }

                 ActionResult ar = mainCardData.GetActionResult(ActionCard.Name, cdList);

                 ActionCard.CurrentActionResult = ar;
                 
                 DisplayActionPanel(ar);
             }
             else
             {
                 TextInput.text = "CARD(S) NOT FOUND.";
             }
         }
         else
         {
             TextInput.text = "ACTION NOT FOUND.";
         }
         
     }

     void DisplayActionPanel(ActionResult AR)
     {
         SetPanelActive(true);
         PanelText.text = AR.FlavorText;

         switch (SupportingCards.Count)
         {
             case 1:
                 Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/2Panel");
                 
                 ActionCard.transform.SetParent(CardPos1);
                 
                 SupportingCards[0].transform.SetParent(CardPos2);


                 break;
             case 2:
                 Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/3Panel");
                 ActionCard.transform.SetParent(CardPos1);
                 SupportingCards[0].transform.SetParent(CardPos2);
                 SupportingCards[1].transform.SetParent(CardPos3);

                 break;
             case 3:
                 Panel.GetComponent<MeshRenderer>().material.mainTexture = Resources.Load<Texture>("Images/4Panel");
                 ActionCard.transform.SetParent(CardPos1);
                 SupportingCards[0].transform.SetParent(CardPos2);
                 SupportingCards[1].transform.SetParent(CardPos3);
                 SupportingCards[2].transform.SetParent(CardPos4);
                 break;
         }
         
         
                 
         ActionCard.transform.localScale = new Vector3(1f, 1f, 1f);
         ActionCard.transform.localPosition = new Vector3(0f, 0f, 0f);                 
         foreach (Card c in SupportingCards)
         {
             
             c.transform.localScale = new Vector3(1f, 1f, 1f);
             c.transform.localPosition = new Vector3(0f, 0f, 0f);
         }
         
         
     }
}