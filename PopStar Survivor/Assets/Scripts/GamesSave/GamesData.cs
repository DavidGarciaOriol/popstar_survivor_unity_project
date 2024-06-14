using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

[System.Serializable]
public class GamesData
{

    /*
      Transforma el tiempo de partida en minutos y segundos para el timer en display.
      

      Actualiza el timer en display.
      stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds); 
     */

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
        chosenCharacterImagePath = GetSpritePath(manager.chosenCharacterImage.sprite);
        chosenCharacterName = manager.chosenCharacterName.text;

        chosenWeaponsUIPaths = new List<string>();
        foreach (var weapon in manager.chosenWeaponsUI)
        {
            chosenWeaponsUIPaths.Add(GetSpritePath(weapon.sprite));
        }

        chosenPassiveItemsUIPaths = new List<string>();
        foreach (var item in manager.chosenPassiveItemsUI)
        {
            chosenPassiveItemsUIPaths.Add(GetSpritePath(item.sprite));
        }
    }

    private string GetSpritePath(Sprite sprite)
    {
        if (sprite != null)
        {
            string path = AssetDatabase.GetAssetPath(sprite);
            int startIndex = path.IndexOf("Resources/") + 10;
            path = path.Substring(startIndex);
            path = path.Replace(".png", "");
            return path;
        }
        return null;
    }



}
