using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    //Player info
    public int currentHealth;
    public int maxHealth;

    public bool isInvulerable = false;
    public float invulerable = 3f;

    private bool isRegenerating = false;
    private float timeSinceLastDmg;

    //coroutine
    private Coroutine reganCour;
    private Coroutine invulCour;

    //sprite for flashing
    private SpriteRenderer flashingEffect;


    //for the spawn and checkpoint
    public SpawningManager spawningManager;
    public string playerID;
    void Start()
    {
        currentHealth = maxHealth;
        flashingEffect = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //timer since last dmg
        timeSinceLastDmg += Time.deltaTime;

        //starts rgean hlth after last dmg if not full hlth (duration to be changed)
        if(currentHealth < maxHealth && !isRegenerating && timeSinceLastDmg >= 3f)
        {
            StartCoroutine(Regan());
        }

    }

    void TakeDamage(int ammount) 
    {
        currentHealth -= ammount;
        Debug.Log("palyer is hit");
        
        if(currentHealth <= 0 ) 
        {
            Die();
            return;
        }

        //resets the regan timer , stops and regan 
        timeSinceLastDmg = 0f;

        if(reganCour != null ) 
        {
            StopCoroutine(reganCour);
            isRegenerating = false;
        }
        
        if(invulCour != null )
        {
            StopCoroutine (invulCour);
        }
       
        invulCour = StartCoroutine(Invulerablity());

    }

    void Die()
    {
        // die
        Debug.Log(gameObject.tag + " died.");

        gameObject.SetActive(false);

        //adding the players to the hashset when they die
        if (spawningManager != null)
        {
            spawningManager.deadplayers.Add(gameObject.tag);
            spawningManager.HandleRespawning(gameObject.tag);
        }
            
    }

    //test to be changed
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isInvulerable && (collision.CompareTag("test")))
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

        yield break;
    }
    
    IEnumerator Regan()
    {
        isRegenerating = true;

        while (currentHealth < maxHealth) 
        {
            currentHealth++;
            Debug.Log("health regan start" + currentHealth);

            yield return new WaitForSeconds(11f);
        }

        isRegenerating = false;
        reganCour = null;
    }

    
}
