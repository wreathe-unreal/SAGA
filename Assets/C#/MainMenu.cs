using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadBasicSceneAfterDelay());
    }

    IEnumerator LoadBasicSceneAfterDelay()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2);

        // Load the scene named "Basic Scene"
        SceneManager.LoadScene("Base Scene");
    }
}
