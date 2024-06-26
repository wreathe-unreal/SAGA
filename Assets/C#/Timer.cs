using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public event Action OnTimerComplete; // Event to subscribe to for when the timer finishes
    public event Action OnTimerUpdate;
    public TMP_Text timerText; // Use public Text timerText; if you're not using TextMeshPro
    public float timeRemaining; // Current timer value
    private bool timerActive = false; // Tracks if the timer is currently running
    public float duration;


    public void StartTimer(int Duration)
    {
        ResetTimer(Duration);
        timeRemaining = Duration;
        duration = timeRemaining;
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
            
            if (OnTimerUpdate != null)
            {
                OnTimerUpdate.Invoke(); // Invoke the complete event
            }
        }
        else if (timerActive)
        {
            timerText.text = FormatTime(0);
            timerActive = false; // Stop the timer
            if (OnTimerComplete != null)
            {
                Sound.Manager.PlaySwordRingReadyTicking();
                OnTimerComplete.Invoke(); // Invoke the complete event
            }
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.CeilToInt(time % 60);
        minutes = Mathf.Clamp(minutes, 0, 9);

        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}