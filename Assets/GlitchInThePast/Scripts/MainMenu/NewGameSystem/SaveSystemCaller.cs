using UnityEngine;

namespace GameData
{
    /// <summary>
    /// Some functions to assign to buttons so that players can 'save' 
    /// a game/ load a game/ Delete a saved game!
    /// </summary>

    public class SaveSystemCaller : MonoBehaviour
    {
        [SerializeField] private GameObject CharacterSelection;

        #region Public Functions
        #region New Game Button
        public void StartNewGame()
        {
            CharacterSelection.SetActive(true); // TODO: Let there be a way to exit the selection screen which also results in deleting the new game file.
            // TODO: Load the first scene (Narrartive here)
        }
        #endregion

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