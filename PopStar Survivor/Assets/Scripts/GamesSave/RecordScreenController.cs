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

            chosenCharacterImage.sprite = LoadSprite(data.chosenCharacterImagePath);
            chosenCharacterName.text = data.chosenCharacterName;
            levelReachedDisplay.text = data.levelReached.ToString();
            timeSurvivedDisplay.text = data.timeSurvived;

            for (int i = 0; i < data.chosenWeaponsUIPaths.Count; i++)
            {
                if (!string.IsNullOrEmpty(data.chosenWeaponsUIPaths[i]))
                {
                    chosenWeaponsUI[i].sprite = LoadSprite(data.chosenWeaponsUIPaths[i]);
                }
            }

            // Asigna los datos del tesoro (passive item) escogido al UI de resultados.
            for (int i = 0; i < data.chosenPassiveItemsUIPaths.Count; i++)
            {
                if (!string.IsNullOrEmpty(data.chosenPassiveItemsUIPaths[i]))
                {
                    chosenPassiveItemsUI[i].sprite = LoadSprite(data.chosenPassiveItemsUIPaths[i]);
                }
            }
        }
        else
        {
            Debug.LogWarning("Error al cargar los datos de la partida anterior.");
        }
    }

    private Sprite LoadSprite(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            return Resources.Load<Sprite>(path);
        }
        return null;
    }

    public void HideRecordScreen()
    {
        isRecordScreenOn = false;
        recordScreen.SetActive(false);
    }
}
