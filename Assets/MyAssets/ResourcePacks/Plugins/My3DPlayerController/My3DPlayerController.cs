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
    [RequireEssentials(typeof(Rigidbody), typeof(Animator))] public string RequireVal;
#endif

    //引用
    public MyInputSystemManager input => MyInputSystemManager.Instance;
    Rigidbody rb;
    Animator anim;

    //配置
    public ControlType mode;

    //玩家属性
    //float speed = 1;

    //按键触发事件
    //public Action<Vector2> MoveAction;

    //按键映射
    public Vector2 MoveKeyValue => mode == ControlType.P1 ? input.MoveKeyValue : input.MoveKey2Value;
    public bool MoveBoostKeyValue => mode == ControlType.P1 ? input.MoveBoostKeyValue : input.MoveBoostKey2Value;


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
        //input.RemoveActionListener(input.InputActionInstance.GamePlay.Move, MoveAction);
    }


    public enum ControlType
    {
        P1, P2
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