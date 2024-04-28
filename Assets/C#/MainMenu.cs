using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string SceneName;
    
    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
    
    public void ChangeScene(float delay)
    {
        Invoke("ChangeScene", delay);
    }
    
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
