using com.goldsprite.gstools.CustomRequireEssentials;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class My3DPlayerController : MonoBehaviour
{
    [RequireEssentials(typeof(Rigidbody), typeof(Animator))] public string RequireVal;

    //引用
    public MyInputSystemManager input => MyInputSystemManager.Instance;
    Rigidbody rb;
    Animator anim;

    //玩家属性
    public float speed = 1;

    //按键触发事件
    public Action<Vector2> MoveAction;


    private void Start()
    {
        RegisterAllActionListeners();
        GetAllComponents();
    }


    private void Update()
    {
        MoveTask();
    }


    private void OnDestroy()
    {
        RemoveAllActionListeners();
    }


    float FaceTurningRate = 10f;
    Quaternion FaceTurningRotation;
    float velTransitionNodeRate;
    float TransitionRate = 5.5f;  //Idle->Walk动画切换过渡时间速率
    private void MoveTask()
    {
        var moveVec = input.MoveKeyValue;
        var vel = moveVec;
        var velLerp = velTransitionNodeRate = Mathf.MoveTowards(velTransitionNodeRate, vel.magnitude, Time.deltaTime * TransitionRate);
        anim.SetFloat("Speed", velLerp);
        if (moveVec.magnitude != 0)
        {
            var dir = UnityUtils.P2To3(moveVec, rb.velocity.y);
            //移动玩家
            rb.velocity = dir * velTransitionNodeRate * speed;
            Debug.Log($"velTransitionNodeRate: {velTransitionNodeRate}, rb.velocity: {rb.velocity}");

            //朝向转向
            FaceTurningRotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, FaceTurningRotation, Time.deltaTime * TransitionRate);

        }
        //动画Blend
        var keyLength = velTransitionNodeRate;
        var unitLength = Vector2.one.normalized.magnitude;
        var rate = keyLength / unitLength;
        anim.SetFloat("MoveValRate", rate);
    }


    private void GetAllComponents()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }


    private void RegisterAllActionListeners()
    {
        //MoveAction = (v) =>
        //{
        //    //设置动画
        //    if(v.magnitude != 0)
        //    {
        //        anim.SetInteger("Walk", 1);
        //    }
        //    else if(anim.GetInteger("Walk") == 2)
        //    {
        //        anim.SetInteger("Walk", 3);
        //    }
        //    Debug.Log(v+", Walk: "+ anim.GetInteger("Walk"));
        //};
        //input.RegisterActionListener(input.InputActionInstance.GamePlay.Move, MoveAction);
    }


    private void RemoveAllActionListeners()
    {
        input.RemoveActionListener(input.InputActionInstance.GamePlay.Move, MoveAction);
    }


}

//#if UNITY_EDITOR
//[CustomEditor(typeof(My3DPlayerController))]
//public class My3DPlayerControllerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        var target = (My3DPlayerController)this.target;

//        ComponentsCheck(target);
//    }


//    private void ComponentsCheck(My3DPlayerController target)
//    {
//        bool pass = true;
//        if (target.GetComponent<Rigidbody>() == null)
//        {
//            var type = typeof(Rigidbody);
//            EditorGUILayout.HelpBox("必需的组件不存在，点击按钮来添加。", MessageType.Warning);
//            if (GUILayout.Button("添加 " + type.Name))
//                if (target.gameObject.AddComponent(type) == null)
//                    Debug.Log("添加组件失败, 请手动添加.");
//            target.enabled = pass = false;
//        }

//        if(pass) target.enabled = true;
//    }


//}
//#endif