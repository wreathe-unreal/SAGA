using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sound : MonoBehaviour
{
    public static Sound Manager { get; private set; }

    public float soundDelay = 0f; // Public float to delay the sound being fired, set to 0 by default.
    private bool queueFlipCard = false; // Flag to determine if RevealCards sounds should be queued.

    public AudioSource audioSource;
    private Queue<AudioClip> flipCardQueue = new Queue<AudioClip>();

    private void Awake()
    {
        if (Manager != null && Manager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Manager = this;
        }
    }

    

    public void PlayAmbience()
    {
        Play("Ambience.mp3");
    }
    
    public void PlayKeyAudio()
    {
        Play("KeyClick.mp3");
    }

    public void PlayBubbling()
    {
        Play("Bubbling.mp3");
    }

    public void PlayError()
    {
        Play("Error");
    }

    public void PlayFlipCard()
    {
        Play("FlipCard");
        // queueFlipCard = true;
        // AudioClip clip = Resources.Load<AudioClip>("Sounds/FlipCard.mp3");
        // QueueSound(clip);

    }

    public void PlayIchRufZuDir()
    {
        Play("Ich ruf zu dir Herr Jesu Christ, BWV 639 (Arr. for Flugelhorn & Organ)");
    }

    public void PlayUnexploredMoon()
    {
        Play("Miguel Johnson - Unexplored Moon");
    }

    public void PlayRattlingBy()
    {
        Play("RattlingBy");
    }

    public void PlayRevealCards()
    {
        Play("RevealCards");

    }
    
    public void PlayPlaceCards()
    {
        Play("PlaceCards");

    }

    public void PlaySolzero()
    {
        Play("solzero - telomere");
    }

    public void PlaySwordRingReadyTicking()
    {
        Play("SwordRingReadyTicking");
    }

    public void Play4Flip()
    {
        Play("4Flip");
    }

    public void Play3Flip()
    {
        Play("3Flip");
    }

    public void Play2Flip()
    {
        Play("2Flip");
    }

    public void Play1Flip()
    {
        Play("1Flip");
    }

    public void PlaySharpenSword()
    {
        Play("sharpensword");
    }
    public void PlaySwordRingReady()
    {
        Play("SwordRingReady");
    }

    public void PlayTransmissionSent()
    {
        Play("Transmission Sent");
    }

    public void PlayTarotWhoosh()
    {
        Play("tarotwoosh");
    }
    public void PlayWhoosh()
    {
        Play("Whoosh");
    }
    
    public void PlayChurchBell()
    {
        Play("churchbell");
    }

    private void Play(string soundFileName)
    {
        StartCoroutine(PlaySoundWithDelay(soundFileName, soundDelay));
    }

    private IEnumerator PlaySoundWithDelay(string soundFileName, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/{soundFileName}");
        audioSource.PlayOneShot(clip);
    }

    private void QueueSound(AudioClip clip)
    {
        if (queueFlipCard && clip.name == "FlipCards")
        {
            flipCardQueue.Enqueue(clip);
            if (flipCardQueue.Count == 1) // Start the coroutine if it's the first sound
            {
                StartCoroutine(PlayFlipCardQueue());
            }
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator PlayFlipCardQueue()
    {
        while (queueFlipCard && flipCardQueue.Count > 0)
        {
            AudioClip clip = flipCardQueue.Dequeue();
            audioSource.PlayOneShot(clip);
            yield return new WaitForSecondsRealtime(0.3f); // Wait for 0.3 seconds in real-time
        }
        queueFlipCard = false; // Reset the flag after playing all sounds
    }

    // Call this method to stop queueing and clear the current queue
    public void StopRevealCardsQueue()
    {
        StopCoroutine(PlayFlipCardQueue());
        flipCardQueue.Clear();
        queueFlipCard = false;
    }

    public void PlayClickCard()
    {
        Play("ClickCard");
    }

    public void PlayTickTock()
    {
        Play("ticktock");
    }
}