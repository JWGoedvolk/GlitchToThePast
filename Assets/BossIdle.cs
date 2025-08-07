using System.Collections;
using System.Collections.Generic;
using Systems.Enemies.Boss;
using UnityEngine;

/// <summary>
/// This is the behaviour for what the boss does when idle
/// </summary>
public class BossIdle : StateMachineBehaviour
{
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Are we in phase 2?
        if (BossStateManager.Instance.Phase == 1)
        {
            
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    // override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     
    // }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //     
    // }
}
