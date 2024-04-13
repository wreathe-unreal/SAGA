using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ParseText : MonoBehaviour
{
//     public TMP_Text Timer; // Ensure this is assigned in the Inspector
//     public TMP_InputField TextInput;
//     private List<GameObject> Deck;
//
//     void Start()
//     {
//         Deck DeckClass = FindObjectOfType<Deck>(); // Ensure DeckClass is found correctly
//         if (DeckClass != null)
//         {
//             Deck = DeckClass.CardDeck;
//         }
//     }
//
//     void Update()
//     {
//         if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter))
//         {
//             SearchAndExecute(TextInput.text, Deck);
//         }
//     }
//
//     public void SearchAndExecute(string input, List<GameObject> deck)
//     {
//         string error = "Input is not of shape: Action + CardName.";
//         var words = input.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
//         
//         if (words.Length != 2)
//         {
//             TextInput.text = error;
//             return;
//         }
//
//         var firstWord = words[0].ToLower();
//         var secondWord = words[1].ToLower();
//
//         var foundAction = deck.FirstOrDefault(card => card.GetComponentInChildren<TMP_Text>().text.ToLower().Contains(firstWord));
//         var foundCard = deck.FirstOrDefault(card => card.GetComponentInChildren<TMP_Text>().text.ToLower().Contains(secondWord));
//
//         if (foundAction != null && foundCard != null)
//         {
//             StartCoroutine(ProcessAction(foundCard, foundAction, 1.0f));
//         }
//     }
//
//     IEnumerator ProcessAction(GameObject card, GameObject destinationCard, float duration)
//     {
//         float time = 0;
//         Vector3 startPosition = card.transform.position;
//
//         while (time < duration)
//         {
//             card.transform.position = Vector3.Lerp(startPosition, destinationCard.transform.position, time / duration);
//             time += Time.deltaTime;
//             yield return null;
//         }
//
//         card.transform.position = destinationCard.transform.position;
//         card.SetActive(false);
//
//         Timer timer = Timer.GetComponent<Timer>();
//         
//         if (timer != null)
//         {
//             timer.StartTimer(6f);
//             timer.OnTimerComplete += () => TimerFinished(card);
//         }
//     }
//
//     private void TimerFinished(GameObject card)
//     {
//         
//         StartCoroutine(ReturnCard(card, 1.0f));
//         
//     }
//
//     // Ensure ReturnCard logic matches your intention
//     IEnumerator ReturnCard(GameObject card, float duration)
//     {
//         card.SetActive(true);
//         
//         // Update text
//         var textComponent = card.GetComponentInChildren<TMP_Text>();
//         if (textComponent != null)
//         {
//             textComponent.text = "Gems";
//             Debug.Log("Updated text to Gems.");
//         }
//         else
//         {
//             Debug.LogError("TMP_Text component not found on the card.");
//         }
//
//         // Update image
//         var image = card.GetComponentInChildren<Image>();
//         if (image != null)
//         {
//             Sprite newSprite = Resources.Load<Sprite>("Images/gems");
//             if (newSprite != null)
//             {
//                 image.sprite = newSprite;
//                 Debug.Log("Updated image sprite.");
//             }
//             else
//             {
//                 Debug.LogError("Failed to load sprite 'gems'.");
//             }
//         }
//         else
//         {
//             Debug.LogError("Image component not found on the card.");
//         }
//
//         // Move card
//         float time = 0;
//         Vector3 startPosition = card.transform.position;
//         while (time < duration)
//         {
//             card.transform.position = Vector3.Lerp(startPosition, new Vector3(0f,0f,0f), time / duration);
//             time += Time.deltaTime;
//             yield return null;
//         }
//    }
}