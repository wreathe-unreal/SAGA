using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ParseText : MonoBehaviour
{
     public TMP_InputField TextInput;
     public GameObject Panel;
     private Board Board;

     void Start()
     {
         // Find the Board component on any GameObject in the scene.
         Board = FindObjectOfType<Board>(); 
     }

     void Update()
     {
         if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter))
         {
             SearchAndExecute(TextInput.text);
         }
     }

     public void SearchAndExecute(string input)
     {
         string error = "Input is not of shape: Action + CardName.";
         var words = input.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);

         if (words.Length < 2 || words.Length > 4)
         {
             TextInput.text = error;
             return;
         }

         for (int i = 0; i < words.Length; i++)
         {
             words[i] = words[i].ToLower();
         }

         // Check if the first word matches an action card
         Card actionCard = Board.Decks["Action"].FirstOrDefault(c => c.Name.ToLower() == words[0]);
         List<Card> foundCards = new List<Card>();
         
         if (actionCard != null)
         {
             bool allMatches = true;

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
                         foundCards.Add(card);
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

                 Panel.SetActive(true);
                 switch (foundCards.Count)
                 {
                     case 1:
                         Panel.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/2Panel");
                         break;
                     case 2:
                         Panel.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/3Panel");
                         break;
                     case 3:
                         Panel.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/4Panel");
                         break;
                 }
                 // All words match and are not of type Action
                 // Processing input, assuming actionCard and the other matched cards are needed here
                 // Example process: start coroutine
                 //StartCoroutine(ProcessAction(foundCardGameObjects, actionCard.Root)); // Adjust as necessary
             }
             else
             {
                 TextInput.text = "Not all cards matched or some cards were actions.";
             }
         }
         else
         {
             TextInput.text = "Action card not found.";
         }
     }

     // IEnumerator ProcessAction(GameObject card, GameObject destinationCard, float duration)
     // {
     //     float time = 0;
     //     Vector3 startPosition = card.transform.position;
     //
     //     while (time < duration)
     //     {
     //         card.transform.position = Vector3.Lerp(startPosition, destinationCard.transform.position, time / duration);
     //         time += Time.deltaTime;
     //         yield return null;
     //     }
     //
     //     card.transform.position = destinationCard.transform.position;
     //     card.SetActive(false);
     //
     //     Timer timer = Timer.GetComponent<Timer>();
     //     
     //     if (timer != null)
     //     {
     //         timer.StartTimer(6f);
     //         timer.OnTimerComplete += () => TimerFinished(card);
     //     }

     private void TimerFinished(GameObject card)
     {
         
         StartCoroutine(ReturnCard(card, 1.0f));
         
     }

     // Ensure ReturnCard logic matches your intention
     IEnumerator ReturnCard(GameObject card, float duration)
     {
         card.SetActive(true);
         
         // Update text
         var textComponent = card.GetComponentInChildren<TMP_Text>();
         if (textComponent != null)
         {
             textComponent.text = "Gems";
             Debug.Log("Updated text to Gems.");
         }
         else
         {
             Debug.LogError("TMP_Text component not found on the card.");
         }

         // Update image
         var image = card.GetComponentInChildren<Image>();
         if (image != null)
         {
             Sprite newSprite = Resources.Load<Sprite>("Images/gems");
             if (newSprite != null)
             {
                 image.sprite = newSprite;
                 Debug.Log("Updated image sprite.");
             }
             else
             {
                 Debug.LogError("Failed to load sprite 'gems'.");
             }
         }
         else
         {
             Debug.LogError("Image component not found on the card.");
         }

         // Move card
         float time = 0;
         Vector3 startPosition = card.transform.position;
         while (time < duration)
         {
             card.transform.position = Vector3.Lerp(startPosition, new Vector3(0f,0f,0f), time / duration);
             time += Time.deltaTime;
             yield return null;
         }
    }
}