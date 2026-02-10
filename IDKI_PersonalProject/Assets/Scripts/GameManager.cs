using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

//general gamemanager for game logic
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject inGameScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private InputActionReference pauseAction;

    private bool isGameActive;
    private bool isPaused;
    private bool isGameOver;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        PauseGame();
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
        Time.timeScale = 1;
    }

    private void PauseGame()
    {
        if (isGameOver) return;
        isPaused = !isPaused;
        isGameActive = !isGameActive;
        pauseScreen.SetActive(isPaused);
        inGameScreen.SetActive(!isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void GameOver()
    {
        isGameOver = true;
        isGameActive = false;
        Time.timeScale = 0;
        gameOverScreen.GetComponentInChildren<TextMeshProUGUI>().text = "Game Over\n score: " + ScoreManager.Instance.GetScore();
        inGameScreen.SetActive(false);
        gameOverScreen.SetActive(true);
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        StartGame();
        SceneManager.LoadScene("GameScene");
    }
}
