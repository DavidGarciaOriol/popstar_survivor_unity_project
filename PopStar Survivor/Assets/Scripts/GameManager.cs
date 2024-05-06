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

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;


    // Display de los stats
    [Header("Current Stat Displayers")]
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    [Header("Results Screen Displayers")]
    public Image chosenCharacterImage;
    public Text chosenCharacterName;
    public Text levelReachedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    // public Text timeSurvivedDisplay;

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

    public void AssignChosenCharacterUI(CharacterScriptableObject chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;

    }

    public void AssignLevelReached(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<Image> chosenWeaponData, List<Image> chosenPassiveItemsData)
    {
        if (chosenWeaponData.Count != chosenWeaponsUI.Count
            || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.Log("La lista de datos de habilidades y tesoros escogidos tienen tamaños diferentes.");
            return;
        }

        // Asigna los datos de la habilidad (arma) escogida al UI de resultados.
        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        { 
            if (chosenWeaponData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponData[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // Asigna los datos del tesoro (passive item) escogido al UI de resultados.
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            if (chosenWeaponData[i].sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }
}
