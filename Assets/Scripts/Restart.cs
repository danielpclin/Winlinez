using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    
    public static bool GameIsOver = false;
    public static bool GamePaused = false;

    public GameObject restartMenu;
    public GameObject pauseMenu;
    public GameObject pauseButton;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Resume()
    {
        restartMenu.SetActive(false);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    public void GameOver()
    {
        restartMenu.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        GamePaused = true;
        GameIsOver = true;
    }
    
    public void GameRestart()
    {
        restartMenu.SetActive(false);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
        GameIsOver = false;
        GamePaused = false;
        Ball.ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
