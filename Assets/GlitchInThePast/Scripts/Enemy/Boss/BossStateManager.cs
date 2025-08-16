using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/* Planning area
 * Current task: transition from idle to attacking
 *   Reference attack manager and have it start the attack animation
 */

namespace Systems.Enemies.Boss
{
    /// <summary>
    /// Author: JW
    /// Purpose: Handles the state and logic of what the boss should be doing and when. Think of it as the brains of the boss
    /// Use: Place on the base GameObject of the boss and provide it with the Animator the boss uses
    /// </summary>
    public class BossStateManager : MonoBehaviour
    {
        #region State
        /// <summary>
        /// Idle => Boss is idle, protecting levers
        /// AttackingArmRaise => Boss arms are raised to allow players to activate levers and will drop after some time
        /// Attacking => While the boss's arms drop and spawn the shockwaves from animation events. animation will transition from here to Idle or Stunned
        /// Stunned => Boss does nothing for a few seconds. Boss is damageable in this state. 
        /// </summary>
        public enum State
        {
            Idle,
            AttackArmRaise,
            Attacking,
            Stunned,
            Transition,
            Dead,
            SpawningEnemies
        }
        public State currentState = State.Idle;
        public bool IsTransitioning = true;
        #endregion
        
        // Singleton
        public static BossStateManager Instance;
        
        [Header("Animation")]
        public Animator BossAnimator;
        
        // Boss phases
        private int phase = 0;
        
        [Header("Timer")]
        [SerializeField] private float currentTime = 0f;
        [Header("Transition")]
        [SerializeField] private float animationTime = 0f;
        [SerializeField] private float animationLength = 0f;
        public bool IsDead = false;
        [Header("Attacking")]
        public bool CanAttack = true;
        [SerializeField] private float attackInterval = 5f;
        [SerializeField] private float attackVariance = 1f;
        [SerializeField] private float currentInterval;
        [SerializeField] private float armHoldTime = 3f;
        [Header("Stunned")]
        [SerializeField] private float stunnedTime = 5f;
        [SerializeField] private BossHealth bossHealth;

        [Header("Enemy Spawning")] 
        public MeleeSpawner SpawnerMelee;
        public RangedSpawner SpawnerRanged;
        public int MeleeSpawnCount = 5;
        public int RangedSpawnCount = 5;
        public bool IsAllMeleeKilled = false;
        public bool IsAllRangedKilled = false;
        
        [Header("Events")]
        public List<UnityEvent> OnStunStartEvent;
        public List<UnityEvent> OnStunEndEvent;
        public List<UnityEvent> OnArmsRaiseEvent;
        public List<UnityEvent> OnArmsDropEvent;
            
        #region Public Getters And Setters
        public int Phase => phase;
        #endregion

        private void Awake()
        {
            // Singleton set up
            if (Instance == null)
            {
                Instance = this;
                Debug.Log("[BossStateManager] Started up singleton successfully");
            }
            if (Instance != this)
            {
                Destroy(this);
            }
            
            // References and Actions
            BossAnimator = GetComponent<Animator>();
            bossHealth.OnStageChangedAction += NextPhase;
            
            // Wait for boss intro to finish
            SetTransitionTime();
        }

        private void OnDisable()
        {
            bossHealth.OnStageChangedAction -= NextPhase;
        }

        private void Update()
        {
            if (currentState == State.Dead) return; // Only continue if we are alive
            
            if (IsTransitioning) // Check if we are transsitioning or playing an animation
            {
                animationTime += Time.deltaTime;
                if (animationTime >= animationLength)
                {
                    IsTransitioning = false;
                    if (currentState == State.Attacking)
                    {
                        currentState = State.Idle;
                    }
                    else if (currentState == State.Transition)
                    {
                        if (IsDead)
                        {
                            bossHealth.Death();
                            currentState = State.Dead;
                            return;
                        }
                        
                        currentState = State.SpawningEnemies;
                        IsAllMeleeKilled = false;
                        IsAllRangedKilled = false;
                    }
                }
                else
                {
                    return;
                }
            }
            
            if (currentState == State.Idle) // Wait in idle until it is time attack
            {
                if (phase == 0)
                {
                    if (currentTime == 0f) // Decide how long to wait in idle before attacking
                    {
                        currentInterval = attackInterval + Random.Range(-attackVariance, attackVariance);
                    }

                    // Count down to attacking
                    currentTime += Time.deltaTime;
                    if (currentTime >= currentInterval)
                    {
                        // Set the animator to raise arms
                        BossAnimator.SetTrigger("Attack");
                        BossAnimator.SetBool("IsArmsRaised", true);
                        currentTime = 0f;
                        currentInterval = 0f;
                        currentState = State.AttackArmRaise; // Arms are raised, now hold them there for a few seconds
                        OnArmsRaiseEvent[phase]?.Invoke();
                    }
                }
                else if (phase == 1)
                {
                    if (IsAllMeleeKilled && IsAllRangedKilled) // Do attack when all enemies are killed
                    {
                        // Set the animator to raise arms
                        BossAnimator.SetTrigger("Attack");
                        BossAnimator.SetBool("IsArmsRaised", true);
                        currentState = State.AttackArmRaise; // Arms are raised, now hold them there for a few seconds
                        currentTime = 0f;
                        currentInterval = 0f;
                        IsAllMeleeKilled = false;
                        IsAllRangedKilled = false;
                        OnArmsRaiseEvent[phase]?.Invoke();
                    }
                }
            }
            else if (currentState == State.AttackArmRaise) // Hold the arms in the air for a few seconds
            {
                currentTime += Time.deltaTime;
                if (currentTime >= armHoldTime)
                {
                    currentTime = 0f;
                    BossAnimator.SetBool("IsArmsRaised", false); // Play animation to drop arms and spawn shockwaves after it ends
                    currentState = State.Attacking;
                    OnArmsDropEvent[phase]?.Invoke();
                    Debug.Log("[BossStateManager] Switching to Arms Drop");
                }
            }
            else if (currentState == State.Attacking) // Boss is playing arm dropping and shockwave spawning animation so does nothing in this state
            {
            }
            else if (currentState == State.Stunned) // All the levers were activated while the boss's arms were raised. Do nothing while in this state other than wait it out
            {
                // Count down how long we are stunned for
                currentTime += Time.deltaTime;
                if (currentTime >= stunnedTime)
                {
                    StunEnd(); // Exit the stunned state and return to idle
                }
            }
            else if (currentState == State.SpawningEnemies)
            {
                // Check that all enemies are spawned to go to Idle state
                if (SpawnerMelee.CurrentState == PooledEnemySpawner.State.WaitingForKills && SpawnerRanged.CurrentState == PooledEnemySpawner.State.WaitingForKills)
                {
                    currentState = State.Idle;
                }
                
                if (IsAllMeleeKilled && IsAllRangedKilled) // Waits for all enemies to be killed in order to do a big attack
                {
                    BossAnimator.SetTrigger("Attack");
                    BossAnimator.SetBool("IsArmsRaised", true);
                    currentTime = 0f;
                    currentInterval = 0f;
                    currentState = State.AttackArmRaise; // Arms are raised, now hold them there for a few seconds
                    IsAllMeleeKilled = false;
                    IsAllRangedKilled = false;
                    OnArmsRaiseEvent[phase]?.Invoke();
                }
            }
        }

        public void SetTransitionTime()
        {
            IsTransitioning = true;
            animationTime = 0f;
            AnimatorStateInfo stateInfo = BossAnimator.GetCurrentAnimatorStateInfo(0);
            animationLength = stateInfo.length;
        }

        public void NextPhase()
        {
            phase++;
            currentState = State.Transition;
            BossAnimator.SetTrigger("NextStage");
            BossAnimator.SetBool("IsArmsRaised", false);
            BossAnimator.SetBool("IsStunned", false);
            BossAnimator.SetInteger("Phase", phase);
            armHoldTime += armHoldTime;
            if (phase >= bossHealth.MaxPhase)
            {
                IsDead = true;
                SpawnerMelee.IsEnabled = false;
                SpawnerRanged.IsEnabled = false;
                OnDeath();
            }
        }

        public void OnDeath()
        {
            BossAnimator.SetTrigger("IsDead");
            SetTransitionTime();
            currentState = State.Transition;
        }

        public void AttackEnd()
        {
            currentState = State.Idle;
            Debug.Log("[BossStateManager] Switching to Idle");
        }

        public void StunStart()
        {
            if (currentState != State.AttackArmRaise)
            {
                return;
            }
            
            currentState = State.Stunned;
            currentTime = 0f;
            BossAnimator.SetBool("IsStunned", true);
            OnStunStartEvent[phase]?.Invoke();
            Debug.Log("[BossStateManager] Switching to Stunned");
        }

        public void StunEnd()
        {
            currentState = State.Idle;
            currentTime = 0f;
            BossAnimator.SetBool("IsStunned", false);
            OnStunEndEvent[phase]?.Invoke();
            Debug.Log("[BossStateManager] Switching to Idle");
        }

        public void AllMeleeKilled()
        {
            IsAllMeleeKilled = true;
        }

        public void AllRangedKilled()
        {
            IsAllRangedKilled = true;
        }
    }
}