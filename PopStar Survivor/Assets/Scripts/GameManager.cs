using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{ 

    public static GameManager instance;

    // Se definen los diferentes estados del juego.
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    // Guarda el estado actual del juego.
    public GameState currentState;

    // Guarda el estado anterior del juego.
    public GameState previousState;

    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;

    // Display de los stats
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    public bool isGameOver = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("SE HA BORRADO UNA INSTANCIA EXTRA DE: " + this);
        }

        DisableScreens();
    }

    void Update()
    {


        // Se define el comportamiento de cada estado del juego.
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndresume();
                break;

            case GameState.Paused:
                CheckForPauseAndresume();
                break;

            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("GAME OVER");
                    DisplayResults();
                }
                break;

            default:
                Debug.LogWarning("NO EXISTE ESL ESTADO DE JUEGO.");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Pausa el juego.

            pauseScreen.SetActive(true);

            Debug.Log("Juego pausado.");
        }
       
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);  
            Time.timeScale = 1f;

            pauseScreen.SetActive(false);

            Debug.Log("Juego reanudado.");
        }
    }

    void CheckForPauseAndresume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }


}
