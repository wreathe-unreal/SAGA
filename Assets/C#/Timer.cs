using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public event Action OnTimerComplete; // Event to subscribe to for when the timer finishes
    public TMP_Text timerText; // Use public Text timerText; if you're not using TextMeshPro
    public float timeRemaining; // Current timer value
    private bool timerActive = false; // Tracks if the timer is currently running


    public void StartTimer(int Duration)
    {
        ResetTimer(Duration);
        timeRemaining = Duration;
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void ResetTimer(int Duration)
    {
        StopTimer();
        timerText.text = FormatTime(Duration);
    }

    void Update()
    {
        if (timerActive && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = FormatTime(timeRemaining);
        }
        else if (timerActive)
        {
            timerText.text = FormatTime(0);
            timerActive = false; // Stop the timer
            if (OnTimerComplete != null)
            {
                OnTimerComplete.Invoke(); // Invoke the complete event
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        minutes = Mathf.Clamp(minutes, 0, 9);

        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}