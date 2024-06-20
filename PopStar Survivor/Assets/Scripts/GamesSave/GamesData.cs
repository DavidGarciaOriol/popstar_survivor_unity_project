using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

[System.Serializable]
public class GamesData
{
    public string timeSurvived;
    public int levelReached;
    public string chosenCharacterImagePath;
    public string chosenCharacterName;
    public List<string> chosenWeaponsUIPaths;
    public List<string> chosenPassiveItemsUIPaths;

    public GamesData(GameManager manager)
    {
        timeSurvived = manager.timeSurvivedDisplay.text;
        levelReached = int.Parse(manager.levelReachedDisplay.text);
        chosenCharacterName = manager.chosenCharacterName.text;
    }
}
