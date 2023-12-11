using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// 屏幕管理器: 
/// 管理屏幕转换, 转场效果
/// </summary>
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance;

    [Header("引用")]
    //public MyGameStateManager _GameStateManager;
    public MyInputSystemManager _InputSystemManager;
    public Animator StartAnim;
    public GameObject GameClearToast;
    public TextMeshProUGUI DefeatCoinText;  //死亡界面玩家金币数

    [Header("配置")]
    [SerializeField] private ScreenType currentScreen = ScreenType.StartScreen;
    public ScreenType CurrentScreen { get=>currentScreen; set {
            LastScreen = currentScreen;
            currentScreen = value;
            //Debug.Log($"CurrentScreen变更: {currentScreen}");
        } 
    }
    public TransitionAnimType TransitionStatu;
    public List<MyKVPair<ScreenType, GameObject>> Screens = new();
    public Animator TransitionAnim;
    public List<MyKVPair<TransitionAnimType, AnimationClip>> TransitionAnimClips;

    //实时
    bool ApplicationStart;
    ScreenType LastScreen;


    private void Awake()
    {
        Instance = this;
        CGScreen(CurrentScreen);

        //程序启动动画
        if (!ApplicationStart)
        {
            ApplicationStart = true;
            StartAnim?.Play("ApplicationStart");
        }
    }


    private void Start()
    {
        RegisterListenShortcutKeys();  //注册快捷键(Esc

        ////注册游戏通关事件监听
        //LevelManager.GameClearedEvent += () =>
        //{
        //    GameClearToast.SetActive(true);
        //};
    }


    private void Update()
    {
        UpdateGameState();

        //UpdateDefeatCoin();
    }


    ///// <summary>
    ///// 刷新显示死亡界面玩家金币数
    ///// </summary>
    //private void UpdateDefeatCoin()
    //{
    //    if (CurrentScreen == ScreenType.DefeatScreen)
    //    {
    //        var datas = GlobalDatasManager.PlayerDatas;
    //        DefeatCoinText.text = datas.Coins+"";

    //        var trans = DefeatCoinText.GetComponent<RectTransform>();
    //        LayoutRebuilder.ForceRebuildLayoutImmediate(trans.parent.GetComponent<RectTransform>());
    //    }
    //}


    /// <summary>
    /// 
    /// </summary>
    private void UpdateGameState()
    {
        var pauseAll = TransitionStatu != TransitionAnimType.None;
        var pauseGame = CurrentScreen == ScreenType.PauseScreen || CurrentScreen == ScreenType.DefeatScreen;
        _InputSystemManager.SetInputEnable(_InputSystemManager.InputActionInstance.UIPlay, !pauseAll);  //UI输入
        //_GameStateManager.ChangeGameState(pauseAll||pauseGame?GameStateType.Pause:GameStateType.Running);  //游戏运行

        ////卸载场景
        //var unload = LevelManager.GameLoaded
        //    && CurrentScreen != ScreenType.GamePlayScreen 
        //    && CurrentScreen != ScreenType.PauseScreen
        //    && CurrentScreen != ScreenType.DefeatScreen
        //    ;
        //if (unload)
        //{
        //    LevelManager.UnLoadCurrentMap();
        //}
    }


    /// <summary>
    /// 切换屏幕
    /// </summary>
    /// <param name="target"></param>
    public void CGScreen(ScreenType target, bool convertState = true)
    {
        foreach (var kvp in Screens)
            kvp.Value.SetActive(false);

        var targetCanvas = Tools.GetScreenGameObjectByType(Screens, target);
        targetCanvas.SetActive(true);

        if(convertState) CurrentScreen = target;
    }


    /// <summary>
    /// 有过渡效果的切换屏幕
    /// </summary>
    /// <param name="target"></param>
    public void CGScreenTransition(ScreenType target, Action act = null)
    {
        PlayTransitionAnim(TransitionAnimType.Close, (animName) => {
            CGScreen(target);
            PlayTransitionAnim(TransitionAnimType.Open, (n) => { act?.Invoke(); });
        });
    }


    ///// <summary>
    ///// 有过渡效果的切换屏幕: 实际协程逻辑
    ///// </summary>
    ///// <param name="target"></param>
    ///// <returns></returns>
    //private IEnumerator CGScreenTransitionTask(ScreenType target)
    //{
    //    var p = TransitionAnimClips.FirstOrDefault(p => p.Key == TransitionAnimType.Close);
    //    if (p == null)
    //    {
    //        Debug.LogWarning("动画错误");
    //        yield return 0;
    //    }
    //    TransitionAnim.PlayInFixedTime(p.Value.name);
    //    while (true)
    //    {
    //        if (Tools.IsAnimPlayEnd(TransitionAnim, p.Value.name))
    //            break;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    CGScreen(target);

    //    p = TransitionAnimClips.FirstOrDefault(p => p.Key == TransitionAnimType.Open);
    //    if (p == null)
    //    {
    //        Debug.LogWarning("动画错误");
    //        yield return 0;
    //    }
    //    TransitionAnim.PlayInFixedTime(p.Value.name);
    //    //while (true)
    //    //{
    //    //    if (Tools.IsAnimPlayEnd(TransitionAnim, p.Value.name))
    //    //        break;
    //    //    yield return new WaitForFixedUpdate();
    //    //}

    //}


    /// <summary>
    /// 播放指定转换动画并异步执行回调
    /// </summary>
    /// <param name="type"></param>
    /// <param name="act"></param>
    public void PlayTransitionAnim(TransitionAnimType type, Action<string> act = null)
    {
        //过渡时暂停
        TransitionStatu = type;
        //暂停事件系统以防止多次点击
        var eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.enabled = false;

        //播放动画
        var p = TransitionAnimClips.FirstOrDefault(p => p.Key == type);
        if (p == null)
        {
            Debug.LogWarning("动画错误");
            return;
        }
        TransitionAnim.Play(p.Value.name, 0, 0);

        //等待动画结束
        StartCoroutine(WaitAnimFinish(p.Value.name, () =>
        {
            //执行回调
            act?.Invoke(p.Value.name);
            TransitionStatu = TransitionAnimType.None;
            eventSystem.enabled = true;
        }));
    }


    /// <summary>
    /// 协程方法: 等待动画播放完成执行回调
    /// </summary>
    /// <param name="animName"></param>
    /// <param name="act"></param>
    /// <returns></returns>
    public IEnumerator WaitAnimFinish(string animName, Action act)
    {
        while (true)
        {
            if (Tools.IsAnimPlayEnd(TransitionAnim, animName))
                break;
            yield return null;
        }
        act?.Invoke();
    }


    class Tools
    {
        public static bool IsAnimPlayEnd(Animator animator, string animName, int Indexlayer = 0)
        {
            var clip = animator.GetCurrentAnimatorClipInfo(Indexlayer);
            return
                (clip.Length != 0 && clip[0].clip.name == animName) &&
                animator.GetCurrentAnimatorStateInfo(Indexlayer).normalizedTime >= 1;
        }


        public static GameObject GetScreenGameObjectByType(List<MyKVPair<ScreenType, GameObject>> screens, ScreenType type)
        {
            var screen = screens.FirstOrDefault(pair => pair.Key == type);

            if (screen.Equals(default(KeyValuePair<ScreenType, GameObject>)))
            {
                // 没有找到匹配的 ScreenType，可以抛出异常或返回 null，具体取决于您的需求
                Debug.LogWarning("No GameObject found for the specified ScreenType.");
            }

            return screen.Value;
        }


    }


    private void RegisterListenShortcutKeys()
    {
        OnKeyDown_Return();
    }

    bool returnKeyDown;
    private void OnKeyDown_Return()
    {
        _InputSystemManager.RegisterActionListener<bool>(_InputSystemManager.InputActionInstance.UIPlay.Return, (b) =>
        {

            //if (!LevelManager.GameLoaded) return;  //禁止游戏外使用
            if (!b) return;

            if (CurrentScreen == ScreenType.PauseScreen)
            {
                CGScreen(LastScreen);
                //Debug.Log("CGLast");
            }
            else
            {
                CGScreen(ScreenType.PauseScreen);
                //Debug.Log("CGPause");
            }
        });
        //var keyDown = _InputSystemManager.ReturnKeyValue;
        //if (keyDown)
        //{
        //    if(CurrentScreen == ScreenType.PauseScreen)
        //    {
        //        _InputSystemManager.ReturnKeyValue = false;
        //        return;
        //    }
        //    else
        //    {
        //        CGScreen(ScreenType.PauseScreen);
        //    }
        //}
    }


    //public static IEnumerator RegisterPlayerDeathEventTask()
    //{
    //    var Player = FindObjectOfType<PlayerComponent>();
    //    var events = Player.Event;

    //    while(events == null)
    //    {
    //        events = Player.Event;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    //已死亡并显示死亡界面
    //    Action DeathEnd = () =>
    //    {
    //        Instance.CGScreen(ScreenType.DefeatScreen);
    //    };
    //    events.RegisterEvent(PlayerEventManager.Event.DeathEnd, DeathEnd);
    //}


    public enum ScreenType
    {
        StartScreen, LevelSelectionScreen, SettingsScreen, GamePlayScreen, PauseScreen, DefeatScreen
    }


    public enum TransitionAnimType
    {
        None, Open, Close
    }


    #region Btn_Onclick

    public void StartCanvas_StartBtn()
    {
        //CGScreenTransition(ScreenType.LevelSelectionScreen);
        CGScreenTransition(ScreenType.GamePlayScreen);
    }

    public void StartCanvas_OptionsBtn()
    {
        CGScreenTransition(ScreenType.SettingsScreen);
    }

    public void StartCanvas_ExitBtn()
    {
        PlayTransitionAnim(TransitionAnimType.Close, (p) => {
            Application.Quit();
        });
    }


    //public void LevelSelectionCanvas_EnterLevelBtn(LevelName levelName)
    //{
    //    //if (!_CustomSelectLevelData.IsUnLock(levelName)) return;

    //    var target = ScreenType.GamePlayScreen;

    //    PlayTransitionAnim(TransitionAnimType.Close, (p) =>
    //    {
    //        _LevelManager.LoadLevelAsync(levelName, () =>
    //        {
    //            CGScreen(target, false);
    //            PlayTransitionAnim(TransitionAnimType.Open, (n) => { CurrentScreen = target; });
    //        });
    //    });
    //}


    public void GOONBtn()
    {
        CGScreen(LastScreen);
    }


    public void ReturnSettingsBtn()
    {
        CGScreenTransition(ScreenType.SettingsScreen);
    }


    public void ReturnLevelSelectionBtn()
    {
        CGScreenTransition(ScreenType.LevelSelectionScreen);
    }


    public void ReturnStartScreenBtn()
    {
        CGScreenTransition(ScreenType.StartScreen);
    }


    ///// <summary>
    ///// 玩家重生按钮
    ///// </summary>
    //public void PlayerReSpawnBtn()
    //{
    //    var respawnPrice = 30;
    //    if (GlobalDatasManager.PlayerDatas.Coins < respawnPrice) return;

    //    GlobalDatasManager.PlayerDatas.Coins -= 30;
    //    var player = FindObjectOfType<PlayerComponent>();
    //    CGScreen(ScreenType.GamePlayScreen);
    //    player.Events.RaiseEvent(PlayerEventManager.Event.Respawn, this);
    //}

    #endregion


    //public void EnterLevel(LevelName levelName)
    //{
    //    if (!GlobalDatasManager.LevelDatas.IsUnLockLevel(levelName)) return;

    //    PlayTransitionAnim(TransitionAnimType.Close, (n) => {
    //        LevelManager.LoadLevelAsync(levelName, () =>
    //        {
    //            PlayTransitionAnim(TransitionAnimType.Open, (n) =>
    //            {
    //                Debug.Log($"关卡: {Enum.GetName(levelName.GetType(), levelName)} 加载完成.");
    //            });
    //            CGScreen(ScreenType.GamePlayScreen);
    //        });
    //    });
    //}


}
