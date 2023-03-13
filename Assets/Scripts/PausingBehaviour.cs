using UnityEngine;


/// <summary>
/// ֧����ͣ�� MonoBehaviour ����.
/// ��Ϸ����Ҫ��ͣ���ܵ��߼���Ӧ�ü̳и��ಢ���߼�д�� OnUpdate �� OnLateUpdate ������, 
/// ����������������Ϊ��ͣʱ���ᱻ����.
/// </summary>
public class PausingBehaviour : MonoBehaviour

{

    /// <summary>
    /// �Ƿ���ͣ�߼�����.
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
    /// ����ͣ���߼����·���.
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
    /// ����ͣ���߼����·���.
    /// </summary>
    protected virtual void OnLateUpdate()
    {

    }

    /// <summary>
    /// ��ͣ��ʼʱ����ø÷���.
    /// </summary>
    protected virtual void OnPauseEnter()
    {

    }

    /// <summary>
    /// ��ͣ����ʱ����ø÷���.
    /// </summary>
    protected virtual void OnPauseExit()
    {

    }
}