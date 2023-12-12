using com.goldsprite.gstools.CustomRequireEssentials;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class My3DPlayerController : MonoBehaviour
{
#if UNITY_EDITOR
    [RequireEssentials(typeof(Rigidbody), typeof(Animator), typeof(GroundDetection))] public string RequireVal;
#endif

    //引用
    public MyInputSystemManager input => MyInputSystemManager.Instance;
    Rigidbody rb;
    Animator anim;
    GroundDetection gDetection;

    //配置
    public ControlType mode;
    public bool AnimApplyRootMotion = true;

    //玩家属性
    public float speed = 15;
    public float jumpForce = 5;

    //按键触发事件
    //public Action<Vector2> MoveAction;
    public Action<bool> AttackAction;
    public Action<bool> JumpAction;

    //按键映射值
    public Vector2 MoveKeyValue => mode == ControlType.P1 ? input.MoveKeyValue : input.MoveKey2Value;
    public bool MoveBoostKeyValue => mode == ControlType.P1 ? input.MoveBoostKeyValue : input.MoveBoostKey2Value;
    //public bool AttackKeyValue => mode == ControlType.P1 ? input.AttackKeyValue : input.AttackKey2Value;

    //按键映射
    InputAction AttackKeyAction => mode == ControlType.P1 ? input.InputActionInstance.GamePlay.Attack : input.InputActionInstance.GamePlay.Attack2;
    InputAction JumpKeyAction => mode == ControlType.P1 ? input.InputActionInstance.GamePlay.Jump : input.InputActionInstance.GamePlay.Jump2;


    private void Start()
    {
        RegisterAllActionListeners();
        GetAllComponents();
    }


    private void Update()
    {
        MoveTask();

        anim.SetBool("Grounded", gDetection.IsGround);  //更新地面检查动画变量

        //anim.applyRootMotion = AnimApplyRootMotion;
    }


    private void OnDestroy()
    {
        RemoveAllActionListeners();
    }


    Quaternion FaceTurningRotation;
    float TransitionRate = 4.5f;  //动画切换过渡时间速率
    float currentMoveVal;
    float currentMoveValRate;
    float currentMoveKeyRate;
    private void MoveTask()
    {
        var moveVec = MoveKeyValue;
        var dir = UnityUtils.P2To3(moveVec, 0);
        var moveLength = moveVec.magnitude;
        var k = 0;
        if (moveLength != 0)
        {
            k = MoveBoostKeyValue ? 2 : 1;
            //朝向转向
            Facing(dir);
        }

        //动画Blend
        currentMoveKeyRate = Mathf.MoveTowards(currentMoveKeyRate, moveLength, Time.deltaTime * TransitionRate);
        currentMoveValRate = Mathf.MoveTowards(currentMoveValRate, k, Time.deltaTime * TransitionRate);
        currentMoveVal = currentMoveKeyRate * currentMoveValRate;
        anim.SetFloat("MoveValRate", currentMoveVal);
    }


    private void Facing(Vector3 dir)
    {
        FaceTurningRotation = Quaternion.LookRotation(dir, Vector3.up);
        FaceTurningRotation = Quaternion.Lerp(transform.localRotation, FaceTurningRotation, Time.deltaTime * TransitionRate * currentMoveVal);
        var eulers = FaceTurningRotation.eulerAngles;
        eulers.x = eulers.z = 0;
        FaceTurningRotation.eulerAngles = eulers;
        transform.localRotation = FaceTurningRotation;
    }


    private void GetAllComponents()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        gDetection = GetComponent<GroundDetection>();
    }


    private void RegisterAllActionListeners()
    {
        AttackAction = (down) =>
        {
            anim.SetInteger("Attack", down ? 1 : 0);
        };
        input.RegisterActionListener(AttackKeyAction, AttackAction);

        JumpAction = (down) =>
        {
            anim.SetInteger("Jump", down ? 1 : 0);
        };
        input.RegisterActionListener(JumpKeyAction, JumpAction);
    }


    private void RemoveAllActionListeners()
    {
        input.RemoveActionListener(AttackKeyAction, AttackAction);
        input.RemoveActionListener(JumpKeyAction, JumpAction);
    }


    /// <summary>
    /// 起跳
    /// </summary>
    public void ApplyJumpForce()
    {
        var oldVel = rb.velocity;
        var moveVel = GetMoveVel();
        oldVel = UnityUtils.P2To3(moveVel, jumpForce);

        rb.velocity = oldVel;
    }


    /// <summary>
    /// 获取实际移动速度
    /// </summary>
    /// <returns></returns>
    private Vector2 GetMoveVel()
    {
        var dir = MoveKeyValue;
        var velRatio = dir * currentMoveVal;
        var vel = velRatio * speed;
        return vel;

    }


    public enum ControlType
    {
        P1, P2
    }


}