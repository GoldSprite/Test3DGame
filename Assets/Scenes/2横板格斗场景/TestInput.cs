using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    public Action<Vector2> act = (v) => { };


    private void Start()
    {
        var input = MyInputSystemManager.Instance;
        input.RegisterActionListener(input.InputActionInstance.GamePlay.Move, act);
        input.RemoveActionListener(input.InputActionInstance.GamePlay.Move, act);
    }


}
