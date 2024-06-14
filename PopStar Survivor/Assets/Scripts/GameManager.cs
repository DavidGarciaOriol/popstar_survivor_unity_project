using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{ 

    public static GameManager instance;

    // Se definen los diferentes estados del juego.
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    // Guarda el estado actual del juego.
    public GameState currentState;

    // Guarda el estado anterior del juego.
    public GameState previousState;

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;


    // Display de los stats.
    [Header("Current Stat Displayers")]
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentMightDisplay;
    public TMP_Text currentProjectileSpeedDisplay;
    public TMP_Text currentMagnetDisplay;

    [Header("Results Screen Displayers")]
    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    [Header("Stopwatch")]
    public float timeLimit;
    float stopwatchTime;
    public TMP_Text stopwatchDisplay;

    // Controla el estado de fin de partida.
    public bool isGameOver = false;

    // Controla el estado de estar eligiendo mejora tras subir de nivel.
    public bool choosingUpgrade;

    // Referencia al GameObject del jugador (Player GameObject).
    public GameObject playerObject;

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
                UpdateStopwatch();
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

            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    Debug.Log("Mostrando Pantalla de Mejora");
                    levelUpScreen.SetActive(true);
                }
                break;

            default:
                Debug.LogWarning("NO EXISTE EL ESTADO DE JUEGO.");
                break;
        }
    }

    // Genera el texto flotante encima del enemigo cuando le hacemos daño.
    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration = 0.5f, float speed = 30f)
    {
        GameObject textObject = new GameObject("Damage Floating Text");
        RectTransform rect = textObject.AddComponent<RectTransform>();
        TextMeshProUGUI tmPro = textObject.AddComponent<TextMeshProUGUI>();

        tmPro.text = text;
        tmPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        tmPro.fontSize = textFontSize;

        if (textFont)
        {
            tmPro.font = textFont;
        }

        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObject, duration);

        textObject.transform.SetParent(instance.damageTextCanvas.transform);
        textObject.transform.SetSiblingIndex(0);

        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        float time = 0;
        float yOffset = 0;
        Vector3 lastKnownPosition = target.position;
        while (time < duration)
        {
            if (!rect)
            {
                break;
            }

            tmPro.color = new Color(tmPro.color.r, tmPro.color.g, tmPro.color.b, 1 - time / duration);

            if (target)
            {
                lastKnownPosition = target.position;
            }

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(lastKnownPosition + new Vector3(0, yOffset));
            
            yield return wait;
            time += Time.deltaTime;
        }
    }

    public static void GenerateFloatingText(string text, Transform target, float duration = 1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas)
        {
            return;
        }

        if (!instance.referenceCamera)
        {
            instance.referenceCamera = Camera.main;
        }

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed
            ));
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
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text;
        GamesSaveManager.SaveGameData(this);
        Debug.Log("Datos de la partida guardados correctamente.");
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignChosenCharacterUI(CharacterData chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon;
        chosenCharacterName.text = chosenCharacterData.Name;

    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsAndPassiveItemsUI(List<PlayerInventory.Slot> chosenWeaponData, List<PlayerInventory.Slot> chosenPassiveItemsData)
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
            if (chosenWeaponData[i].image.sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponData[i].image.sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }

        // Asigna los datos del tesoro (passive item) escogido al UI de resultados.
        for (int i = 0; i < chosenPassiveItemsUI.Count; i++)
        {
            if (chosenWeaponData[i].image.sprite)
            {
                chosenPassiveItemsUI[i].enabled = true;
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].image.sprite;
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false;
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;

        UpdateStopwatchOnDisplay();

        if (stopwatchTime >= timeLimit)
        {
            playerObject.SendMessage("Kill");
        }
    }

    void UpdateStopwatchOnDisplay()
    {
        // Transforma el tiempo de partida en minutos y segundos para el timer en display.
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);

        // Actualiza el timer en display.
        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades");

    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

}
