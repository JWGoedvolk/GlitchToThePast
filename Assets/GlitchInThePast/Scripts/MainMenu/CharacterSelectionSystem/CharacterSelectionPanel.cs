using UnityEngine;
using UnityEngine.SceneManagement;
using GameData;
using UnityEngine.UI;

public class CharacterSelectionPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputConnectionManager inputConnectionManager;
    [SerializeField] private GameObject[] characterPrefabs = new GameObject[2];
    [SerializeField] private Image p1SpriteRenderer;
    [SerializeField] private Image p2SpriteRenderer;

    [SerializeField] private Color lockedColor = Color.green;
    [SerializeField] private Color unlockedColor = Color.white;

    private bool p1Confirmed, p2Confirmed;
    #endregion

    void OnEnable()
    {
        inputConnectionManager.AssignInputs();
        p1Confirmed = p2Confirmed = false;
    }

    #region Public Functions
    public void OnPlayer1Confirmed()
    {
        if (PlayerBarMover.p1Index == 1) return;

        PlayerBarMover.P1Locked = !PlayerBarMover.P1Locked;
        p1Confirmed = PlayerBarMover.P1Locked;

        if (p1SpriteRenderer != null) p1SpriteRenderer.color = p1Confirmed ? lockedColor : unlockedColor;

        TryStartGame();
    }

    public void OnPlayer2Confirmed()
    {
        if (PlayerBarMover.p2Index == 1) return;

        PlayerBarMover.P2Locked = !PlayerBarMover.P2Locked;
        p2Confirmed = PlayerBarMover.P2Locked;

        if (p2SpriteRenderer != null) p2SpriteRenderer.color = p2Confirmed ? lockedColor : unlockedColor;

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

        SceneManager.LoadScene(2);
    }
    #endregion
}