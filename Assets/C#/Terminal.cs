using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using System.Text.RegularExpressions;
using Unity.VisualScripting; // Include this namespace for Regex
using UnityEngine.UI;
public class Terminal : MonoBehaviour
{
     public ActionGUI ActionPanel;
     private static TMP_InputField TextInput;
     public static string[] ParsedText;
     private static bool bStatusText = true;
     
     void Start()
     {
         TextInput = gameObject.GetComponent<TMP_InputField>();


     }
     
     void Update()
     {
         // Listen for key down event only once in Update
         if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
         {
             ActionPanel.StartInputCoroutine();
         }

         if (Input.GetKeyDown(KeyCode.Escape) && ActionGUI.IsActionPanelOpen())
         {
             ActionGUI.CancelActionPanel();
             TextInput.interactable = true;
         }
     }

     

     public static void ParseText()
     {
         ParsedText = Array.Empty<string>();
         string input = TextInput.text;
         string[] words = input.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

         for (int i = 0; i < words.Length; i++)
         {
             words[i] = words[i].ToLower();
         }

         ParsedText = words;
     }

     public static void AppendText(string appendText)
     {
         if (bStatusText || TextInput.text.Length == 0)
         {
             TextInput.text = appendText;
             
         }
         else
         {
             TextInput.text += " + " + appendText;
         }
         bStatusText = false;
     }

     public static void SetText(string newText, bool bIsStatusText)
     {
         TextInput.text = newText;
         if (bIsStatusText)
         {
             TextInput.interactable = true;
             Player.State.NullActionCard();
         }
         bStatusText = bIsStatusText;
     }

}