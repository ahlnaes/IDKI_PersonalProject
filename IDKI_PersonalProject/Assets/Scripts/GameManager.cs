using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject inGameScreen;

    private bool isGameActive;
    private bool isPaused;

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            PauseGame();
        }
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        pauseScreen.SetActive(false);
        inGameScreen.SetActive(true);
        isGameActive = true;
    }

    private void PauseGame()
    {
        isPaused = !isPaused;
        isGameActive = !isGameActive;
        pauseScreen.SetActive(isPaused);
        inGameScreen.SetActive(!isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }


    
}
