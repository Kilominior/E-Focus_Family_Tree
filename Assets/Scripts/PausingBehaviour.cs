using UnityEngine;


/// <summary>
/// 支持暂停的 MonoBehaviour 基类.
/// 游戏中需要暂停功能的逻辑都应该继承该类并将逻辑写到 OnUpdate 及 OnLateUpdate 方法中, 
/// 这两个方法在设置为暂停时不会被调用.
/// </summary>
public class PausingBehaviour : MonoBehaviour

{

    /// <summary>
    /// 是否暂停逻辑处理.
    /// </summary>
    public static bool pause { get; set; }

    static PausingBehaviour()
    {
        pause = false;
    }

    private bool _isPaused;

    protected virtual void OnEnable()
    {
        _isPaused = pause;
    }

    protected virtual void Update()
    {
        if (!pause)
        {
            if (_isPaused)
            {
                _isPaused = false;
                this.OnPauseExit();
            }
            this.OnUpdate();
        }
        else
        {
            if (!_isPaused)
            {
                _isPaused = true;
                this.OnPauseEnter();
            }
        }
    }

    /// <summary>
    /// 可暂停的逻辑更新方法.
    /// </summary>
    protected virtual void OnUpdate()
    {

    }

    protected virtual void LateUpdate()
    {
        if (!pause)
        {
            this.OnLateUpdate();
        }
    }

    /// <summary>
    /// 可暂停的逻辑更新方法.
    /// </summary>
    protected virtual void OnLateUpdate()
    {

    }

    /// <summary>
    /// 暂停开始时会调用该方法.
    /// </summary>
    protected virtual void OnPauseEnter()
    {

    }

    /// <summary>
    /// 暂停结束时会调用该方法.
    /// </summary>
    protected virtual void OnPauseExit()
    {

    }
}