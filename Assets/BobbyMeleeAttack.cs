using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlitchInThePast.Scripts.Player;
using JW.BeatEmUp.Objects;
using Systems.Enemies;
using UnityEngine;

public class BobbyMeleeAttack : StateMachineBehaviour
{
    public PlayerWeaponSystem WeaponSystem;
    public CustomTriggerer AttackArea;
    public List<EnemyHealth> EnemiesDamaged;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        WeaponSystem = animator.gameObject.GetComponentInParent<PlayerWeaponSystem>();
        AttackArea = animator.gameObject.GetComponent<CustomTriggerer>();
        EnemiesDamaged.Clear(); // Reset everything for each attack
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Get all objects in the attack area
        var triggering = AttackArea.TriggeringObjects;
        
        // Go through each object in the area
        foreach (var trigger in triggering)
        {
            EnemyHealth eh = trigger.GetComponent<EnemyHealth>(); // Get the enemy's health system
            
            // If it is an enemy with a health system and has not been damaged yet
            if (eh != null && !EnemiesDamaged.Contains(eh))
            {
                // Deal the current combo damage to it
                EnemiesDamaged.Add(eh);
                eh.TakeDamage(WeaponSystem.CurrentComboDamage(), PlayerWeaponSystem.WeaponType.Melee);
                Debug.Log($"Dealt {WeaponSystem.CurrentComboDamage()} to {eh.gameObject.name}");
            }
            else // If it does not exist or has already been damaged then skip it
            {
                continue;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EnemiesDamaged.Clear(); // Reset everything for the next attack
    }
}
