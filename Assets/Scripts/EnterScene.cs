using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EnterScene : MonoBehaviour
{
    public Button enterButton;

    public Button exitButton;

    public GameObject loadPanel;

    public TMP_Text enterButtonText;

    public TMP_Text enterPrompt;

    public float loadWaitTime = 0.5f;

    private void Start()
    {
        if (BaseClasses.TreeOfThisFile != null)
        {
            enterButtonText.text = "�ص�ϵͳ";
            enterPrompt.text = "����������Ŀ";
        }

        enterButton.onClick.AddListener(delegate
        {
            StartCoroutine(nameof(LoadSceneAsync), "SampleScene");
        });

        exitButton.onClick.AddListener(delegate
        {
            //�������unity��������
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            //�����ڴ���ļ���
#else
        Application.Quit();
#endif
        });
    }
    
    //��������ҳ�淽������������
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
        {
            Debug.Log("Scene is loaded : " + sceneName);
            yield break;
        }

        //���ڴ���ʾLoading����
        //�����LoadSceneMode.Single��ʽ���أ���Ҫ������һ��Loading�����������г�������
        //����ʹ��LoadSceneMode.Additive��ʽ�����е�UI���涼������������������ʱʹ��Loading���漴��
        loadPanel.SetActive(true);

        float startTime = Time.time;

        //ѡ�񳡾����ط�ʽ
        //LoadSceneMode.Single��ж�ص�֮ǰ��ԭ�еĳ�����ֻ�����¼��س���
        //LoadSceneMode.Additive������ԭ�г����Ļ����ϸ����³���
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        //AsyncOperation.progress��Χ(0~1)
        //isDoneΪfalseʱ�������ص�0.9�ͻ���ͣ��ֱ��isDoneΪtrueʱ�Ż��������ʣ���0.9 - 1.0
        //ֻ��allowSceneActivation = true��isDone�Ż���progress = 1.0��ֵΪtrue
        //�������ó��������ڼ�����ɺ��Զ���ת����һ������
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            //�ֶ��ӳ�����ʱ�䣬��ֹ���ؽ���չʾʱ�����
            if (Time.time - startTime >= loadWaitTime)
            {
                if (operation.progress >= 0.9f && !operation.allowSceneActivation)
                    operation.allowSceneActivation = true;
            }
            yield return null;
        }

        //�������Ҫ�ȴ�����ֱ�Ӽ��غ���ת����
        //operation.allowSceneActivation = true;
        //while (!operation.isDone)
        //{
        //    yield return null;
        //}

        //���ڴ˹ر�Loading����
        loadPanel.SetActive(true);

        //������ɣ��������ûص�
        Debug.Log("LoadSceneAsync Success : " + sceneName);
    }

    public IEnumerator UnloadSceneAsync(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.isLoaded)
        {
            Debug.Log("Scene is not loaded : " + sceneName);
            yield break;
        }

        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }
}
