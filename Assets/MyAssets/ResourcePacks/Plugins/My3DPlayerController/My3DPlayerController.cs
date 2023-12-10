using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class My3DPlayerController : MonoBehaviour
{
    public MyInputSystemManager input => MyInputSystemManager.Instance;
    Rigidbody rb;

    public Action<Vector2> MoveAction = (v)=>
    {
        //Debug.Log($"{(v.magnitude==0?"抬起":"按下")}移动");
    };


    private void Start()
    {
        RegisterAllActionListeners();
        GetAllComponents();
    }


    private void OnDestroy()
    {
        RemoveAllActionListeners();
    }


    private void GetAllComponents()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void RegisterAllActionListeners()
    {
        input.RegisterActionListener(input.InputActionInstance.GamePlay.Move, MoveAction);
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
//        if (target.GetComponent<Rigidbody>() == null)
//        {
//            GUILayout.Label("组件不全");
//            target.enabled = false;
//        }
//    }


//}
//#endif