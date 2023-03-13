using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneAgent : MonoBehaviour
{
    //该脚本中包含一些全局管理的方法
    public bool isGamePaused;

    public UnityEvent PauseEvent; //暂停时用于通知各个游戏对象的事件

    public UnityEvent ContinueEvent; //继续时用于通知各个游戏对象的事件

    private void Start()
    {
        isGamePaused = false;
    }

    public void gamePause()
    {
        isGamePaused = true;
        PauseEvent.Invoke();
        Time.timeScale = 0;
    }

    public void gameContinue()
    {
        isGamePaused = false;
        Time.timeScale = 1;
        ContinueEvent.Invoke();
    }
}
