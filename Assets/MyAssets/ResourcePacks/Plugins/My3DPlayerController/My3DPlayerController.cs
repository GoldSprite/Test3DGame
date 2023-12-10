using com.goldsprite.gstools.CustomRequireEssentials;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class My3DPlayerController : MonoBehaviour
{
    [RequireEssentials(typeof(Rigidbody))] public string RequireVal;

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