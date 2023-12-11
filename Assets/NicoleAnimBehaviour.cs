using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NicoleAnimBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Walk: " + animator.GetInteger("Walk"));
        int currentAnimationHash = stateInfo.fullPathHash;
        if (currentAnimationHash == Animator.StringToHash("Base Layer.Lucy_Walk_F_Start_Inplace"))
        {
            animator.SetInteger("Walk", 2);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Walk: " + animator.GetInteger("Walk"));
        if (animator.GetInteger("Walk") == 3)
            animator.SetInteger("Walk", 0);
        //int currentAnimationHash = stateInfo.fullPathHash;
        //if (currentAnimationHash == Animator.StringToHash("Base Layer.Lucy_Walk_F_Inplace"))
        //{
        //    if(animator.GetInteger("Walk") == 3)
        //        animator.SetInteger("Walk", 0);
        //}
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //int currentAnimationHash = stateInfo.fullPathHash;
        //if (currentAnimationHash == Animator.StringToHash("Base Layer.Walk"))
        //{
        //    if (animator.GetInteger("Walk") == 3)
        //        animator.SetInteger("Walk", 0);
        //}
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
