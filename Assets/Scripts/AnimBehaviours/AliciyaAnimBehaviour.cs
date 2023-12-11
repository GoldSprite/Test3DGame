using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AliciyaAnimBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //进入Attack完成, 转为播放ing
        if (IsAnimByFullName(stateInfo, "Base Layer.Lucy_Kick02_Root")) animator.SetInteger("Attack", 2);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Hurt播放完毕退出
        if (IsAnimByFullName(stateInfo, "Base Layer.HurtBlend Tree")) animator.SetBool("Hurt", false);
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


    /// <summary>
    /// 当前时候为目标Name动画
    /// </summary>
    /// <param name="stateInfo"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool IsAnimByFullName(AnimatorStateInfo stateInfo, string name)
    {
        int currentAnimationHash = stateInfo.fullPathHash;
        return currentAnimationHash == Animator.StringToHash(name);
    }


}
