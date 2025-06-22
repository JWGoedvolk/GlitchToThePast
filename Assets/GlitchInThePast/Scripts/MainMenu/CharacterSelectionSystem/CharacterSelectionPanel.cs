using UnityEngine;
using UnityEngine.SceneManagement;
using GameData;

public class CharacterSelectionPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputConnectionManager inputConnectionManager;
    [SerializeField] private GameObject[] characterPrefabs = new GameObject[2]; //TODO: publicise it when needed

    bool p1Confirmed, p2Confirmed;
    #endregion

    void OnEnable()
    {
        inputConnectionManager.AssignInputs();
        p1Confirmed = p2Confirmed = false;
    }

    #region Public Functions
    public void OnPlayer1Confirmed()
    {
        p1Confirmed = true;
        TryStartGame();
    }

    public void OnPlayer2Confirmed()
    {
        p2Confirmed = true;
        TryStartGame();
    }
    #endregion

    #region Private Functions
    private void TryStartGame()
    {
        if (!p1Confirmed || !p2Confirmed)
            return;

        int slot1 = PlayerBarMover.p1Index;
        int slot2 = PlayerBarMover.p2Index;

        bool valid = (slot1 == 0 && slot2 == 2) || (slot1 == 2 && slot2 == 0);
        if (!valid)
        {
            Debug.LogWarning("Both players must choose opposite characters to start");
            return;
        }

        #region Save Selection
        int char1Id = (slot1 == 0) ? 0 : 1;
        int char2Id = (slot2 == 0) ? 0 : 1;

        var save = new GameSaveData
        {
            Player1Character = characterPrefabs[char1Id].name,
            Player2Character = characterPrefabs[char2Id].name,
            Player1Input = InputConnectionManager.Player1InputType.ToString(),
            Player2Input = InputConnectionManager.Player2InputType.ToString(),
            CurrentChapter = 1,
            CurrentLevel = 1
        };
        GameSaveSystem.SaveGame(save);
        #endregion

        SceneManager.LoadScene(1);
    }
    #endregion
}