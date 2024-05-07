using System;
using System.Collections;
using TMPro;
using UnityEngine;
using BinaryCharm.TextMeshProAlpha;
using UnityEngine.Serialization;


public class TextFade : MonoBehaviour
{
    public TMP_Text TextToFade;
    private Coroutine FadeCoroutine;
    private event Action OnFadeComplete; // Action to notify when fading is complete

    // Call this method to start the fading effect with an optional callback
    public void TriggerFade(TMP_Text FadeText, Action FadeCompleteCallback = null)
    {
        TextToFade = FadeText;
        // Assign the callback
        OnFadeComplete = FadeCompleteCallback;

        // Stop any existing coroutine if already running
        if (FadeCoroutine != null)
        {
            StopCoroutine(FadeCoroutine);
        }

        // Start the fading coroutine
        FadeCoroutine = StartCoroutine(FadeTextOverTime());
    }

    private IEnumerator FadeTextOverTime()
    {
        ActionGUI.bTextIsBeingPrinted = true;
        int intPart = 0;
        float fracPart = 0f;
        int textLength = TextToFade.text.Length;

        while (intPart < textLength)
        {

            fracPart += Time.unscaledDeltaTime * 15;
            
            if (fracPart >= 1)
            {
                intPart++;
                fracPart = Mathf.Clamp(fracPart - 1, 0, 1);
            }
            
            TextToFade.setAlphaBegin();
            TextToFade.setAlphaForCharsRange(0, intPart, 255); // Adjust the starting alpha values
            TextToFade.setCharAlphaFadeHorizEx(intPart, fracPart, 0.5f, 255, 0);
            if (intPart + 1 >= textLength)
            {
                TextToFade.setAlphaEnd();
                break;
            }
            TextToFade.setAlphaForCharsRange(intPart + 1, textLength, 0);
            TextToFade.setAlphaEnd();

            yield return new WaitForEndOfFrame(); // Yield control back to the main thread
        }

        TextToFade.setAlphaBegin();
        TextToFade.setAlphaForCharsRange(0, textLength, 255);
        TextToFade.setAlphaEnd();

        // Invoke the callback, if provided
        OnFadeComplete?.Invoke();
    }
}