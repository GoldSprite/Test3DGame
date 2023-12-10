using com.goldsprite.gstools.CustomRequireEssentials;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class My3DPlayerController : MonoBehaviour
{
    [RequireEssentials(typeof(Rigidbody))] public string RequireVal;

    //引用
    public MyInputSystemManager input => MyInputSystemManager.Instance;
    Rigidbody rb;

    //玩家属性
    public float speed = 5;

    //按键触发事件
    public Action<Vector2> MoveAction;


    private void Start()
    {
        RegisterAllActionListeners();
        GetAllComponents();
    }


    private void Update()
    {
        var moveVec = input.MoveKeyValue;
        if (moveVec.magnitude != 0)
        {
            var dir = UnityUtils.P2To3(moveVec, rb.velocity.y);
            //移动玩家
            rb.velocity = dir * speed;

            //朝向转向
            FaceTurningRotation = Quaternion.LookRotation(dir, Vector3.up);
            rb.rotation = Quaternion.Lerp(rb.rotation, FaceTurningRotation, Time.deltaTime * FaceTurningRate);
        }
    }


    private void OnDestroy()
    {
        RemoveAllActionListeners();
    }


    private void GetAllComponents()
    {
        rb = GetComponent<Rigidbody>();
    }


    float FaceTurningRate = 10f;
    Quaternion FaceTurningRotation;
    private void RegisterAllActionListeners()
    {
    //    MoveAction = (v) =>
    //    {
    //        if (v.magnitude == 0) return;

    //        var dir = UnityUtils.P2To3(v, rb.velocity.y);
    //        移动玩家
    //        rb.velocity = dir * speed;

    //        朝向转向
    //        FaceTurningRotation = Quaternion.LookRotation(dir, Vector3.up);
    //};
    //    input.RegisterActionListener(input.InputActionInstance.GamePlay.Move, MoveAction);
    }


    private IEnumerator FaceTurningTask(Quaternion rotation)
    {
        while (input.MoveKeyValue.magnitude != 0)
        {
            rb.rotation *= Quaternion.RotateTowards(rb.rotation, rotation, Time.deltaTime * FaceTurningRate);
            yield return null;
        }
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