using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealthSystem : MonoBehaviour
{
    public static System.Action<int, PlayerHealthSystem> OnPlayerSpawned;
    private HealthDisplayUI healthUI;

    //Player info
    [Header("Health")]
    public int currentHealth;
    public int maxHealth;

    [Header("Invulnerability")]
    public bool isInvulerable = false;
    public float invulerable = 3f;

    [Header("Damage")]
    [SerializeField] private List<string> damageableTags = new List<string>();
    // Regen
    private bool isRegenerating = false;
    private float timeSinceLastDmg;

    //coroutine
    private Coroutine reganCour;
    private Coroutine invulCour;

    //sprite for flashing
    private SpriteRenderer flashingEffect;




    //for the spawn and checkpoint

    public SpawningManager spawningManager;
    private PlayerInput playerInput; // changed ID to refer to player index instead

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        flashingEffect = GetComponent<SpriteRenderer>();
        OnPlayerSpawned?.Invoke(playerInput.playerIndex, this);

        HealthDisplayUI[] allDisplays = FindObjectsOfType<HealthDisplayUI>();
        foreach (var display in allDisplays)
        {
            if ((int)display.playerID == playerInput.playerIndex)
            {
                healthUI = display;
                break;
            }
        }

        UpdateUI();
    }


    void Update()
    {
        //timer since last dmg
        timeSinceLastDmg += Time.deltaTime;

        //starts rgean hlth after last dmg if not full hlth (duration to be changed)
        if (currentHealth < maxHealth && !isRegenerating && timeSinceLastDmg >= 3f)
        {
            StartCoroutine(Regan());
        }

    }
    private void UpdateUI()
    {
        if (healthUI != null)
            healthUI.UpdateHealth(currentHealth, maxHealth);
    }

    void TakeDamage(int ammount)
    {
        currentHealth -= ammount;
        UpdateUI();
        Debug.Log("palyer is hit");

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        //resets the regan timer , stops and regan 
        timeSinceLastDmg = 0f;

        if (reganCour != null)
        {
            StopCoroutine(reganCour);
            isRegenerating = false;
        }

        if (invulCour != null)
        {
            StopCoroutine(invulCour);
        }

        invulCour = StartCoroutine(Invulerablity());

    }

    void Die()
    {
        // die
        Debug.Log($"Player {playerInput.playerIndex} died.");

        gameObject.SetActive(false);

        if (spawningManager != null)
        {
            // No need for hashset, we're now telling it which player to respawn (by their index)
            spawningManager.HandleRespawning(playerInput);
        }

    }

    void OnTriggerEnter(Collider collision) // 2.5d game regular works fine :D
    {
        if (!isInvulerable && damageableTags.Contains(collision.tag))
        {
            TakeDamage(1);
        }

    }

    IEnumerator Invulerablity()
    {
        isInvulerable = true;

        float flashInterval = 0.2f;
        float timer = 0f;



        while (timer < invulerable)
        {
            if (flashingEffect != null)
            {
                flashingEffect.enabled = false;
                yield return new WaitForSeconds(flashInterval);
                flashingEffect.enabled = true;
                yield return new WaitForSeconds(flashInterval);
            }
            else
            {
                yield return new WaitForSeconds(invulerable);
                break;
            }

            timer += flashInterval * 2;
        }

        isInvulerable = false;

    }

    IEnumerator Regan()
    {
        isRegenerating = true;
        UpdateUI();

        while (currentHealth < maxHealth)
        {
            currentHealth++;
            Debug.Log("health regan start" + currentHealth);

            yield return new WaitForSeconds(11f);
        }

        isRegenerating = false;
        reganCour = null;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();

        isInvulerable = false;
        timeSinceLastDmg = 0f;
        if (invulCour != null) StopCoroutine(invulCour);
        if (reganCour != null) StopCoroutine(reganCour);
        isRegenerating = false;
        reganCour = null;
        invulCour = null;

        // if (flashingEffect != null) flashingEffect.enabled = true;
    }

    public void AssignUI(HealthDisplayUI ui)
    {
        healthUI = ui;
        UpdateUI();
    }

}
