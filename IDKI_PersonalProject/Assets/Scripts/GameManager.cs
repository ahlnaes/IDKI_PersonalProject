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
    [SerializeField] private float uiSpawnDistance = 2f;

    private Camera vrCamera;
    private bool isGameActive;
    private bool isPaused;
    private bool isGameOver;
    private Scene currentScene;

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    private void OnEnable()
    {
        if (pauseAction != null && currentScene.buildIndex == 1)
        {
            pauseAction.action.Enable();
            pauseAction.action.performed += OnPausePerformed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null && currentScene.buildIndex == 1)
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
        vrCamera = Camera.main;
        if (currentScene.buildIndex == 1)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        pauseScreen.SetActive(false);
        inGameScreen.SetActive(true);
        isGameActive = true;
        Time.timeScale = 1;
    }

    private void PlaceInFrontOfPlayer(GameObject panel)
    {
        if (vrCamera == null || panel == null) return;
        var camTransform = vrCamera.transform;
        var forward = camTransform.forward;
        forward.y = 0f;
        forward.Normalize();
        panel.transform.position = camTransform.position + forward * uiSpawnDistance;
        panel.transform.rotation = Quaternion.LookRotation(forward);
    }

    private void PauseGame()
    {
        if (isGameOver) return;
        isPaused = !isPaused;
        isGameActive = !isGameActive;
        if (isPaused) PlaceInFrontOfPlayer(pauseScreen);
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
        PlaceInFrontOfPlayer(gameOverScreen);
        gameOverScreen.SetActive(true);
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenuVR");
    }

    public void Restart()
    {
        StartGame();
        SceneManager.LoadScene("GameScene");
    }
}
