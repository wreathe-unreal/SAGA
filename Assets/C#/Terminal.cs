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
     public static TMP_InputField TMP_Input;
     public static string[] ParsedText;
     private static bool bStatusText = true;
     private static bool bPaused = false;
     
     void Start()
     {
         TMP_Input = gameObject.GetComponent<TMP_InputField>();


     }
     
     void Update()
     {
         if (Input.GetKeyDown(KeyCode.Space))
         {
             PauseButtonClicked();
         }
     }

     public static void PauseButtonClicked()
     {
         if (bPaused)
         {
             Time.timeScale = 0f;
         }
         else
         {
             Time.timeScale = 1f;
         }

         bPaused = !bPaused;
     }

     public static void ParseText()
     {
         ParsedText = Array.Empty<string>();
         string input = TMP_Input.text;
         string[] words = input.Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();

         for (int i = 0; i < words.Length; i++)
         {
             words[i] = words[i].ToLower();
         }

         ParsedText = words;
     }

     public static void AppendText(string appendText)
     {
         if (bStatusText || TMP_Input.text.Length == 0)
         {
             TMP_Input.text = appendText;
             
         }
         else
         {
             TMP_Input.text += " + " + appendText;
         }
         bStatusText = false;
     }

     public static void SetText(string newText, bool bIsStatusText)
     {
         TMP_Input.text = newText;
         if (bIsStatusText)
         {
             TMP_Input.interactable = true;
             Player.State.NullActionCard();
         }
         bStatusText = bIsStatusText;
     }

}