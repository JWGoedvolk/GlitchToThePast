using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RoomExplodingButton : MonoBehaviour
{
    #region Variables
    [Header("Explosion Settings")]
    [Tooltip("How long does it take for the explosion to take place?")]
    public float countdownTime = 5f;
    [Tooltip("When they respawn, ofsset them from the button's position by whatever value is given.")]
    public Vector3 respawnOffset = new Vector3(-2f, 0f, 0f);

    [Header("UI References")]
    [Tooltip("UI Text element to show the ticking countdown")]
    public TMP_Text countdownText;
    [Tooltip("Red Image to cover the screen, acting as an explosion going off")]
    public Image redImage;

    private bool running = false;
    private SpawningManager spawningManager;
    #endregion

    private void Awake()
    {
        spawningManager = FindObjectOfType<SpawningManager>();
        if (redImage) redImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (running) return;

        if (other.tag == "Player1" || other.tag == "Player2")
        {
            Debug.Log(other.name);
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                StartCoroutine(ExplosionSequence());
            }
        }
    }

    #region Private Functions
    private IEnumerator ExplosionSequence()
    {
        running = true;
        float timer = countdownTime;

        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = timer.ToString("F0");
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        if (countdownText != null) countdownText.text = "";
        if (redImage != null)
        {
            redImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            redImage.gameObject.SetActive(false);
        }

        if (spawningManager != null)
        {
            spawningManager.ExplodeRespawnAll(transform.position, respawnOffset);
        }

        running = false;
    }
    #endregion
}