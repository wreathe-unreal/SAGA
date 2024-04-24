using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Text.RegularExpressions; // Include this namespace for Regex
using UnityEngine.UI;
public class Terminal : MonoBehaviour
{
     public ActionGUI ActionPanelGUI;
     public TMP_Text PanelText;
     public static TMP_InputField TextInput;
     private float LastActionTime;
     private float CooldownDuration;
     private bool bFirstCoro;
     private string[] ParsedText;
     
     void Start()
     {
         CooldownDuration = .5f;
         bFirstCoro = true;
         TextInput = gameObject.GetComponent<TMP_InputField>();


     }
     
     void Update()
     {
         // Listen for key down event only once in Update
         if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
         {
             StartInputCoroutine();
             bFirstCoro = false; //no cooldown for first enter coro
         }

         if (Input.GetKeyDown(KeyCode.Escape) && ActionGUI.IsActionPanelOpen())
         {
             ActionGUI.CancelActionPanel();
             TextInput.interactable = true;
         }
     }

     public void StartInputCoroutine()
     {
         StartCoroutine(HandleEnterInput());
     }
     
    private IEnumerator HandleEnterInput()
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
         
         if(ActionGUI.ReturnedCards.Count > 0 )
         {
             ActionGUI.ExecuteReturnPanel();
             yield break;
         }
         
         // Handling no action card scenario
         if (ActionGUI.ActionCard == null) 
         {
             
             if (!ActionGUI.IsActionPanelOpen() && !ActionGUI.IsReturnPanelOpen())
             {
                 TextInput.interactable = false;
                 ParseText();
                 ActionGUI.ActionCard = BoardState.Decks["Action"].FirstOrDefault(c => c.Name.ToLower() == ParsedText[0]);

                if (ActionGUI.ActionCard != null)
                {
                     // Check if the first word matches an action card
                     if (ParsedText.Length == 1 && ActionGUI.ActionCard.Timer.timeRemaining <= 0)
                     {
                         ActionGUI.ActionCard.OpenAction(); //early exit if just a action and we open the action
                         yield break;
                     }

                     ActionGUI.ActionCard.CurrentAction = ActionGUI.FindAction(ParsedText);

                     print("here");
                     if (ActionGUI.ActionCard.CurrentAction == null)
                     {
                         TextInput.text = "IMPOSSIBLE."; // Get the current ColorBlock from the TextInput
                         ActionGUI.ActionCard = null;
                     }
                     else
                     {
                        ActionGUI.DisplayActionPanel();
                         yield break;
                     }
                }
                else
                {
                    TextInput.text = "NO ACTION FOUND.";
                }
                 

             }
         }
         if(ActionGUI.IsActionPanelOpen())
         {
             ActionPanelGUI.ExecuteActionPanel();
             TextInput.interactable = true;
         }
         // Wait for the cooldown duration before allowing another action, unaffected by Time.timeScale
         yield return new WaitForSecondsRealtime(CooldownDuration);
     }

     public void ParseText()
     {
         ParsedText = Array.Empty<string>();
         string input = TextInput.text;
         string[] words = input.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

         if (words.Length > 4)
         {
             TextInput.text = "ERROR.";
         }

         for (int i = 0; i < words.Length; i++)
         {
             words[i] = words[i].ToLower();
         }

         ParsedText = words;
     }

     public static void AddCardName(string cardName)
     {
         if (TextInput.text.ToLower() == "> action + card" || TextInput.text.ToLower() == "impossible.")
         {
             TextInput.text = "";
         }

         string append = "";
         if (TextInput.text.Length > 0)
         {
             append = " + " + cardName;

         }
         else
         {
             append = cardName;

         }
        TextInput.text += append;
     }
}