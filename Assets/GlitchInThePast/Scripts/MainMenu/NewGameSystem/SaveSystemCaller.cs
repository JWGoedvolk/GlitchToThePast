using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameData
{
    /// <summary>
    /// Some functions to assign to buttons so that players can 'save' 
    /// a game/ load a game/ Delete a saved game!
    /// </summary>

    public class SaveSystemCaller : MonoBehaviour
    {
        #region Public Functions
        #region Load Game Button
        public void LoadSavedGame()
        {
            GameSaveData loaded = GameSaveSystem.LoadGame();
            if (loaded != null)
            {
                // TODO: Open a panel which displays saved game files.
                // Use loaded.CurrentLevel to actually load the levels
                Debug.Log("Loaded Level: " + loaded.CurrentLevel);
            }
        }
        #endregion

        #region Delete Saved Game Button
        public void DeleteSavedGame()
        {
            // TODO: Be able to target a specific game file to delete
            GameSaveSystem.DeleteSave();
        }
        #endregion
        #endregion
    }
}