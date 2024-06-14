using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class GamesSaveManager
{
    public static void SaveGameData(GameManager manager)
    {
        GamesData gamesData = new GamesData(manager);
        string dataPath = Application.persistentDataPath + "/games.record";
        FileStream fileStream = new FileStream(dataPath, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, gamesData);
        fileStream.Close();
    }

    public static GamesData LoadGameData()
    {
        string dataPath = Application.persistentDataPath + "/games.record";

        if (File.Exists(dataPath))
        {
            FileStream fileStream = new FileStream(dataPath, FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            GamesData gamesData = (GamesData)binaryFormatter.Deserialize(fileStream);
            return gamesData;
        }
        else
        {
            Debug.LogError("No se ha encontrado ningún archivo de registro de partidas.");
            return null;
        }
    }
}
