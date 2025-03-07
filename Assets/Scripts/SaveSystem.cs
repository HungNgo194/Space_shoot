using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private static string saveFilePath = Application.persistentDataPath + "/leaderboard.json";

    public static void SaveGameData(List<GameData> data)
    {
        string json = JsonUtility.ToJson(new Wrapper<GameData> { Items = data });
        File.WriteAllText(saveFilePath, json);
        Debug.Log(saveFilePath);
    }

    public static List<GameData> LoadGameData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<Wrapper<GameData>>(json).Items;
        }
        return new List<GameData>();
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> Items;
    }
}