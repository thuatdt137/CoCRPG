using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    // Path where the save file will be stored
    private string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    // Save the game data to a file
    public void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true); // Convert GameData to JSON string (true for pretty print)
        File.WriteAllText(SavePath, json);            // Write the JSON string to a file
        Debug.Log("Game saved to: " + SavePath);
    }

    // Load the game data from a file
    public GameData LoadGame()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath); // Read the JSON string from the file
            GameData data = JsonUtility.FromJson<GameData>(json); // Convert JSON back to GameData
            Debug.Log("Game loaded from: " + SavePath);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found at: " + SavePath);
            return null; // Return null if no save file exists
        }
    }
}