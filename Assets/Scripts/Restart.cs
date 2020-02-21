using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    
    public static bool GameOver = false;

    public GameObject GameOverUI;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        GameOverUI.SetActive(false);
        Time.timeScale = 1f;
        GameOver = false;
        Ball.ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Pause()
    {
        GameOverUI.SetActive(true);
        Time.timeScale = 0f;
        GameOver = true;
    }
}
