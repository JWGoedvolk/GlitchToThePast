using System.IO;
using UnityEngine;

namespace GameData
{
    /// <summary>
    /// All functions you need to handle game saving and loading!!
    /// </summary>
   
    public static class GameSaveSystem
    {
        private static string SavePath => Path.Combine(Application.persistentDataPath, "gameSaveFiles.json");

        #region Public Functions
        #region Save Game File
        public static void SaveGame(GameSaveData data)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            Debug.Log("Saving the game at" + SavePath);
        }
        #endregion

        #region Load Game File
        public static GameSaveData LoadGame()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("No saved files found!");
                return null;
            }

            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<GameSaveData>(json);
        }
        #endregion

        #region Delete Game File
        public static void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("Save file has been successfully deleted");
            }
        }
        #endregion

        public static bool SaveExists() => File.Exists(SavePath);
        #endregion
    }
}