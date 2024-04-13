using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText; // Use public Text timerText; if you're not using TextMeshPro

    public event Action OnTimerComplete; // Event to subscribe to for when the timer finishes

    private float timeRemaining;
    private bool timerRunning;

    void Awake()
    {
        // Initialize to ensure there are no null reference issues
        OnTimerComplete += () => { };
    }

    public void StartTimer(float duration)
    {
        if (timerRunning)
        {
            // Optionally handle the case where the timer is already running
            Debug.Log("Timer is already running. Resetting timer.");
        }

        timeRemaining = duration;
        timerRunning = true;
        StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimeDisplay();
            yield return null;
        }

        timerRunning = false;
        // Notify all subscribers that the timer has finished
        timerText.text = "";
        OnTimerComplete.Invoke();
    }

    private void UpdateTimeDisplay()
    {
        // Update the timer text
        timerText.text = $"{(int)timeRemaining / 60:D2}:{(int)timeRemaining % 60:D2}";
    }
}