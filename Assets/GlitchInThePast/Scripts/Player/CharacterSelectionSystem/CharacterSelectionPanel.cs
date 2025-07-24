using GameData;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private InputConnectionManager inputConnectionManager;
    [SerializeField] private GameObject[] characterPrefabs = new GameObject[2];
    [SerializeField] private Image p1SpriteRenderer;
    [SerializeField] private Image p2SpriteRenderer;

    [Header("What colours should the bars display when the player locks in slot 0 or 2?")]
    [SerializeField] private Color customColourOne;
    [SerializeField] private Color customColourTwo;

    [Header("The hover colour of the bar depending on which slot they are on.")]
    [SerializeField] private Color hoverColourOne;
    [SerializeField] private Color hoverColourTwo;

    private Coroutine p1FlashRoutine;
    private Coroutine p2FlashRoutine;

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

        if (p1Confirmed && p2Confirmed)return;
            
        PlayerBarMover.P1Locked = !PlayerBarMover.P1Locked;
        p1Confirmed = PlayerBarMover.P1Locked;

        if (p1Confirmed)
        {
            if (p1FlashRoutine != null) StopCoroutine(p1FlashRoutine);

            if (PlayerBarMover.p1Index == 0)
            {
                p1FlashRoutine = StartCoroutine(FlashToTargetColor(p1SpriteRenderer, customColourOne));
            }
            else if (PlayerBarMover.p1Index == 2)
            {
                p1FlashRoutine = StartCoroutine(FlashToTargetColor(p1SpriteRenderer, customColourTwo));
            }
        }
        else
        {
            if (p1FlashRoutine != null) StopCoroutine(p1FlashRoutine);
            {
                SetBarAndTextColor(p1SpriteRenderer, Color.white);
            }
        }

        TryStartGame();
    }


    public void OnPlayer2Confirmed()
    {
        if (PlayerBarMover.p2Index == 1) return;

        if (p1Confirmed && p2Confirmed) return;

        PlayerBarMover.P2Locked = !PlayerBarMover.P2Locked;
        p2Confirmed = PlayerBarMover.P2Locked;

        if (p2Confirmed)
        {
            if (p2FlashRoutine != null) StopCoroutine(p2FlashRoutine);

            if (PlayerBarMover.p2Index == 0)
            {
                p2FlashRoutine = StartCoroutine(FlashToTargetColor(p2SpriteRenderer, customColourOne));
            }
            else if (PlayerBarMover.p2Index == 2)
            {
                p2FlashRoutine = StartCoroutine(FlashToTargetColor(p2SpriteRenderer, customColourTwo));
            }
        }
        else
        {
            if (p2FlashRoutine != null) StopCoroutine(p2FlashRoutine);
            {
                SetBarAndTextColor(p2SpriteRenderer, Color.white);
            }
        }

        TryStartGame();
    }
    public void UpdateBarColorFromHover(int playerIndex, int slotIndex)
    {
        if (playerIndex == 1 && !p1Confirmed)
        {
            if (slotIndex == 0)
            {
                SetBarAndTextColor(p1SpriteRenderer, hoverColourOne);
            }
            else if (slotIndex == 2)
            {
                SetBarAndTextColor(p1SpriteRenderer, hoverColourTwo);
            }
            else
            {
                SetBarAndTextColor(p1SpriteRenderer, Color.white);
            }
        }
        else if (playerIndex == 2 && !p2Confirmed)
        {
            if (slotIndex == 0)
            {
                SetBarAndTextColor(p2SpriteRenderer, hoverColourOne);
            }
            else if (slotIndex == 2)
            {
                SetBarAndTextColor(p2SpriteRenderer, hoverColourTwo);
            }
            else
            {
                SetBarAndTextColor(p2SpriteRenderer, Color.white);
            }
        }
    }
    #endregion

    #region Private Functions
    private bool hasStartedFade = false;

    private void TryStartGame()
    {
        if (hasStartedFade) return;

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

        hasStartedFade = true;

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

        StartCoroutine(ConfirmAndFade());
    }

    private IEnumerator ConfirmAndFade()
    {
        yield return new WaitForSeconds(1.0f); // One second grace period to show the locked in state

        if (UI.FadingEffect.ScreenFader.Instance != null)
        {
            UI.FadingEffect.ScreenFader.Instance.FadeTransition("SectionOne", 1.0f, 0.3f);
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    private void SetBarAndTextColor(Image bar, Color color)
    {
        if (bar == null) return;

        bar.color = color;

        var text = bar.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.color = color;
        }
    }

    private IEnumerator FlashToTargetColor(Image bar, Color targetColor, float flashDuration = 0.1f)
    {
        if (bar == null) yield break;

        var text = bar.GetComponentInChildren<TMP_Text>();

        Color flashColor = Color.Lerp(targetColor, Color.black, 0.4f);

        bar.color = flashColor;
        if (text != null) text.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        bar.color = targetColor;
        if (text != null) text.color = targetColor;
    }
    #endregion
}