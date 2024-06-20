using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RecordScreenController : MonoBehaviour
{

    public static RecordScreenController instance;

    public GameObject recordScreen;

    public Image chosenCharacterImage;
    public TMP_Text chosenCharacterName;
    public TMP_Text levelReachedDisplay;
    public TMP_Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    bool isRecordScreenOn;

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

        HideRecordScreen();
    }

    public void ShowRecordScreen()
    {
        isRecordScreenOn = true;
        recordScreen.SetActive(true);
        GamesData data = GamesSaveManager.LoadGameData();

        if (data != null)
        {
            chosenCharacterName.text = data.chosenCharacterName;
            levelReachedDisplay.text = data.levelReached.ToString();
            timeSurvivedDisplay.text = data.timeSurvived;
        }
        else
        {
            Debug.LogWarning("Error al cargar los datos de la partida anterior.");
        }
    }

    public void HideRecordScreen()
    {
        isRecordScreenOn = false;
        recordScreen.SetActive(false);
    }
}
