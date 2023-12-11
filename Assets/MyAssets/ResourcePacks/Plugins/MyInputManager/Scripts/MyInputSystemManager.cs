//#define JoyStick

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static System.Collections.Specialized.BitVector32;

public class MyInputSystemManager : MonoBehaviour
{
    //配置
    public InputActions InputActionInstance { get; private set; }
    public static MyInputSystemManager Instance;
#if JoyStick
    public Joystick joystick;
#endif
    //实时
    public List<MyKVPair<InputActionMap, bool>> InputEnables;
    [Header("KeyListenerValue")]
    [SerializeField] private Vector2 moveKeyValue;
    public Vector2 MoveKeyValue { get => moveKeyValue; private set => moveKeyValue = value; }
    [SerializeField] private bool moveBoostKeyValue;
    public bool MoveBoostKeyValue { get => moveBoostKeyValue; private set => moveBoostKeyValue = value; }
    [SerializeField] private bool returnKeyValue;
    public bool ReturnKeyValue { get => returnKeyValue; private set => returnKeyValue = value; }
    [SerializeField] private bool attackKeyValue;
    public bool AttackKeyValue { get => attackKeyValue; private set => attackKeyValue = value; }

    //2P
    [SerializeField] private Vector2 moveKey2Value;
    public Vector2 MoveKey2Value { get => moveKey2Value; private set => moveKey2Value = value; }
    [SerializeField] private bool moveBoostKey2Value;
    public bool MoveBoostKey2Value { get => moveBoostKey2Value; private set => moveBoostKey2Value = value; }
    [SerializeField] private bool attackKey2Value;
    public bool AttackKey2Value { get => attackKey2Value; private set => attackKey2Value = value; }

    Vector2 oldJoystickMoveActionValue;  //判断是否有新输入变化值


    private void Awake()
    {
        if (Instance == null) Instance = this;
        initInputActions();
        RegisterAllActionListener();
#if JoyStick
        GetJoystick();
#endif
    }

    private void OnDisable()
    {
        InputActionInstance.Disable();
    }

    private void Update()
    {
        UpdateValues();
    }


    /// <summary>
    /// 更新同步所有Action值
    /// </summary>
    private void UpdateValues()
    {
        //输入禁用
        //if (InputEnabled)
        //{
        //    MoveKeyValue = default;
        //    AttackKeyValue = false;
        //}
#if JoyStick
        if(joystick != null)
            UpdateJoystickValue();
#endif
    }


#if JoyStick
    /// <summary>
    /// 更改时<para/>
    /// 刷新Joystick轮盘输入
    /// </summary>
    private void UpdateJoystickValue()
    {
        var move = MoveActionValue;
        move.x = joystick.Horizontal;
        move.y = joystick.Vertical;
        if (oldJoystickMoveActionValue != move)
        {
            MoveActionValue = oldJoystickMoveActionValue = move;
            //Debug.Log("刷新摇杆值");
        }
    }
#endif


    /// <summary>
    /// 全局唯一<para/>
    /// 初始化输入核心类
    /// </summary>
    private void initInputActions()
    {
        if (InputActionInstance != null) return;

        DontDestroyOnLoad(gameObject);
        InputActionInstance = new InputActions();
        InputActionInstance.Enable();

        InitializeInputEnables();
    }


    /// <summary>
    /// 初始化输入开关表
    /// </summary>
    private void InitializeInputEnables()
    {
        InputEnables.Clear();
        var pair = new MyKVPair<InputActionMap, bool>();
        pair.Key = InputActionInstance.GamePlay.Get();
        pair.Value = true;
        InputEnables.Add(pair);
        pair = new MyKVPair<InputActionMap, bool>();
        pair.Key = InputActionInstance.UIPlay.Get();
        pair.Value = true;
        InputEnables.Add(pair);
    }


    /// <summary>
    /// 在初始化时<para/>
    /// 注册所有Action的监听器<para/>
    /// PS: performed-更改时, started-仅按下, canceled-取消时
    /// </summary>
    private void RegisterAllActionListener()
    {
        var gamePlay = InputActionInstance.GamePlay;
        var uiPlay = InputActionInstance.UIPlay;

        RegisterActionListener<Vector2>(gamePlay.Move, (val) => { MoveKeyValue = val; });
        RegisterActionListener<bool>(gamePlay.MoveBoost, (val) => { MoveBoostKeyValue = val; });
        RegisterActionListener<bool>(uiPlay.Return, (val) => { AttackKeyValue = val; });
        RegisterActionListener<bool>(gamePlay.Attack, (val) => { AttackKeyValue = val; });

        RegisterActionListener<Vector2>(gamePlay.Move2, (val) => { MoveKey2Value = val; });
        RegisterActionListener<bool>(gamePlay.MoveBoost2, (val) => { MoveBoostKey2Value = val; });
        RegisterActionListener<bool>(gamePlay.Attack2, (val) => { AttackKey2Value = val; });

    }


    /// <summary>
    /// 注册单个Action监听器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="move"></param>
    /// <param name="value"></param>
    public void RegisterActionListener<T>(InputAction keyAction, Action<T> act, bool log = false)
    {
        Action<InputAction.CallbackContext> action = (c) => {
            var valObj = keyAction.ReadValueAsObject();
            if (valObj == null) 
                valObj = default(T);
            T t = (T)Convert.ChangeType(valObj, typeof(T));

            var disable = !IsInputEnable(keyAction.actionMap);
            if (disable) return;  //输入禁用时返回
            act?.Invoke(t);

            //debug log
            if (!log) return;
            Debug.Log($"[InputSystemManager]: keyAction: {keyAction.name}, keyValue: {t}");
        };
        keyAction.performed += action;
        keyAction.canceled += action;

        AddListenersDic<T>(keyAction, action, act);
    }


    Dictionary<InputAction, Dictionary<Delegate, Action<InputAction.CallbackContext>>> listeners = new Dictionary<InputAction, Dictionary<Delegate, Action<InputAction.CallbackContext>>>();
    public bool RemoveActionListener<T>(InputAction keyAction, Action<T> act)
    {
        if(RemoveListenersDic(keyAction, act, out Action<InputAction.CallbackContext> action))
        {
            keyAction.performed -= action;
            keyAction.canceled -= action;
            return true;
        }

        return false;
    }


    private void AddListenersDic<T>(InputAction keyAction, Action<InputAction.CallbackContext> action, Action<T> act)
    {
        if (!listeners.ContainsKey(keyAction))
        {
            Dictionary<Delegate, Action<InputAction.CallbackContext>> pair = new() { { act, action } };
            listeners.Add(keyAction, pair);
        }
        else if (!listeners[keyAction].ContainsKey(act))
        {
            listeners[keyAction].Add(act, action);
        }
    }


    private bool RemoveListenersDic<T>(InputAction keyAction, Action<T> act, out Action<InputAction.CallbackContext> action)
    {
        if (listeners.ContainsKey(keyAction) && listeners[keyAction].TryGetValue(act, out Action<InputAction.CallbackContext> oAction))
        {
            action = oAction;
            listeners[keyAction].Remove(act);
            return true;
        }
        action = null;
        return false;
    }


    /// <summary>
    /// 是否启用了该输入行动表
    /// </summary>
    /// <param name="actionMap"></param>
    /// <returns></returns>
    private bool IsInputEnable(InputActionMap actionMap)
    {
        var map = InputEnables.FirstOrDefault(p=>p.Key==actionMap);
        if(map == null) return false;

        return map.Value;
    }


    /// <summary>
    /// 设置输入启用/禁用
    /// </summary>
    /// <param name="actionMap">对应行为表</param>
    /// <param name="boo">启用或禁用</param>
    public void SetInputEnable(InputActionMap actionMap, bool boo)
    {
        var map = InputEnables.FirstOrDefault(p => p.Key == actionMap);
        if (map == null) return;

        map.Value = boo;
    }


#if JoyStick
    private void GetJoystick()
    {
        joystick = GameObject.FindObjectOfType<Joystick>().GetComponent<Joystick>();
    }
#endif


}
