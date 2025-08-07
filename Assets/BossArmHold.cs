using System.Collections;
using System.Collections.Generic;
using Systems.Enemies.Boss;
using UnityEngine;

[Tooltip("Wait for this animation to finish before counting down any other timers")]
public class BossArmHold : StateMachineBehaviour
{
    /* Ideation
     * if our state is AttackArmsRaised then we want to wait for the animation to play before counting down the hold time
     */

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (BossStateManager.Instance.currentState == BossStateManager.State.AttackArmRaise)
        {
            BossStateManager.Instance.SetTransitionTime();
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
