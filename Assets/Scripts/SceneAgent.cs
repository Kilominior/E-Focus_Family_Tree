using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneAgent : MonoBehaviour
{
    //�ýű��а���һЩȫ�ֹ���ķ���
    public bool isGamePaused;

    public UnityEvent PauseEvent; //��ͣʱ����֪ͨ������Ϸ������¼�

    public UnityEvent ContinueEvent; //����ʱ����֪ͨ������Ϸ������¼�

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
