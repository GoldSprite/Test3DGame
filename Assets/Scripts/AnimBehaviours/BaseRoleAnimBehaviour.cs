using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRoleAnimBehaviour : StateMachineBehaviour
{

    public List<MyKVPair<string, string>> FullAnimNames;


    private void Awake()
    {
        InitFullAnimNames();
    }


    protected virtual void InitFullAnimNames()
    {
        FullAnimNames = new List<MyKVPair<string, string>>()
        {
            new MyKVPair<string, string>(){ Key="Attack", Value="Base Layer.Lucy_Kick02_Root" },
            new MyKVPair<string, string>(){ Key="Hurt", Value="Base Layer.HurtBlend Tree" },
        };
    }



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //进入Attack完成, 转为播放ing
        if (IsCurrentAnimByFullName(stateInfo, "Attack")) animator.SetInteger("Attack", 2);
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
        if (IsCurrentAnimByFullName(stateInfo, "Hurt")) animator.SetBool("Hurt", false);
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
    /// 当前动画是否匹配目标名字
    /// </summary>
    /// <param name="stateInfo"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private bool IsCurrentAnimByFullName(AnimatorStateInfo stateInfo, string toggleName)
    {
        var animName = MyKVPair<string, string>.TryGetVal(FullAnimNames, toggleName);
        if(animName == null) return false;

        int currentAnimationHash = stateInfo.fullPathHash;
        return currentAnimationHash == Animator.StringToHash(animName);
    }


}
