using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Terminal : MonoBehaviour
{
     public PanelController PanelController;
     public TMP_Text PanelText;
     public TMP_InputField TextInput;
     private float LastActionTime;
     private float CooldownDuration;
     private bool bFirstCoro;
     
     void Start()
     {
         
         CooldownDuration = .5f;
         bFirstCoro = true;


     }
     
     void Update()
     {
         // Listen for key down event only once in Update
         if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
         {
             StartCoroutine(HandleInput());
             bFirstCoro = false; //no cooldown for first enter coro
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
        
         
         
         if(PanelController.gameObject != null && PanelController.gameObject.activeSelf && PanelController.ReturnedCards.Count > 0 )
         {
             print("closing return panel");
             PanelController.CloseReturnPanel();
             yield return new WaitForSecondsRealtime(CooldownDuration);
         }
         
         // Handling no action card scenario
         if (PanelController.ActionRef == null) 
         {
             
             if (TextInput.text != "> ACTION + CARD" && TextInput.text != "CARD(S) NOT FOUND."
                                                     && TextInput.text != "ACTION NOT FOUND." && TextInput.text != "")
             {
                 TextInput.interactable = false;
                 ParseText(TextInput.text); // Parse and search
                 TextInput.interactable = true;
             }
         }
         else
         {
             PanelController.CloseActionPanel();
         }

         TextInput.text = "> ACTION + CARD";
         // Wait for the cooldown duration before allowing another action, unaffected by Time.timeScale
         yield return new WaitForSecondsRealtime(CooldownDuration);
     }

     public void ParseText(string input)
     {
         string error = "ERROR.";
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

         PanelController.ExecuteInput(words);
         
     }
     
}