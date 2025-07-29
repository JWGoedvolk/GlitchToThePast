using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    [Header("Collision Tags")]
    [SerializeField] private List<string> damageableTags = new List<string>();
    [SerializeField] private List<string> healableTags = new List<string>();

    [SerializeField] private Animator animator;

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

    // Rumble
    private RumbleController rumbleController;
    
    public UnityEvent onDamageTaken;
    public UnityEvent onDeath;

    //sfx
    private SFXManager sfxManager;
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        sfxManager = FindObjectOfType<SFXManager>();
        if (sfxManager == null)
        {
            Debug.LogWarning(" No SFXManager found ");
        }
        else
        {
            Debug.Log(" Found SFXManager");
        }

        rumbleController = RumbleController.Instance;
    }

    void Start()
    {
        currentHealth = maxHealth;
        flashingEffect = GetComponent<SpriteRenderer>();
        OnPlayerSpawned?.Invoke(playerInput.playerIndex, this);

        if (animator is null)
        {
            animator = GetComponent<Animator>();
        }

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

    public void TakeDamage(int ammount)
    {
        Debug.Log($"PlayerHealthSystem: TakeDamage called with amount: {ammount}");
        if (ammount > 0)
        {
            if (sfxManager != null)
            {
                Debug.Log("PlayerHealthSystem: Calling PlayHitSFX()");
                sfxManager.PlayHitSFX();
                
                if (playerInput.currentControlScheme == "Controller") rumbleController.TriggerRumble(rumbleController.rumbleDuration, rumbleController.lowFrequencyIntensity, rumbleController.highFrequencyIntensity);
            }
            else
            {
                Debug.LogWarning("sfxManager reference is nada");
            }
        }

        currentHealth -= ammount;
        UpdateUI();
        onDamageTaken?.Invoke();
        Debug.Log("palyer is hit");

        if (animator != null)
        {
            animator.SetBool("isGettingHit", true);
            StartCoroutine(ResetHitAnimation());
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateUI();

            sfxManager?.PlayDeathSFX();

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

        //play sfx when damged
      

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        // die
        Debug.Log($"Player {playerInput.playerIndex} died.");

        if (playerInput.currentControlScheme == "Controller") rumbleController.TriggerRumble(1f, rumbleController.lowFrequencyIntensity, rumbleController.highFrequencyIntensity);
        gameObject.SetActive(false);
        
        if (spawningManager != null)
        {
            // No need for hashset, we're now telling it which player to respawn (by their index)
            spawningManager.HandleRespawning(playerInput);
        }

       
        Debug.Log("player died");
    }

    void OnTriggerEnter(Collider collision) // 2.5d game regular works fine :D
    {
        Debug.Log($"Triggered by: {collision.tag}");

        if (!isInvulerable && damageableTags.Contains(collision.tag))
        {
            TakeDamage(1);
        }
        else if (healableTags.Contains(collision.tag))
        {
            TakeDamage(-1); // Heal by 1
        }
    }

    public void StartInvulnerability()
    {
        isInvulerable = true;
    }

    public void StopInvulnerability()
    {
        isInvulerable = false;
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
    private IEnumerator ResetHitAnimation()
    {
        yield return new WaitForSeconds(0.3f); // adjust based on animation length
        animator.SetBool("isGettingHit", false);
    }

}