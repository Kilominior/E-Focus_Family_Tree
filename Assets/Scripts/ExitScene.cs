using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitScene : BaseClasses
{
    public Button exitButton;

    public GameObject loadPanel;

    public GameObject exitDialog;

    public Button exitComfirm;

    public Button exitCencel;

    public float loadWaitTime = 0.5f;

    void Start()
    {
        exitButton.onClick.AddListener(delegate
        {
            //���༭���Ƿ��г�Ա���ڣ����У���ʾ�û����д���֮
            if (AreaTransfer.memberQuantityOfAreas[1] == 1)
            {
                TransferPrompt = findPrompt("exitWithEditor");
                prompt?.SetActive(true);
                prompt.GetComponent<PromptManage>().PromptAppear();
                return;
            }
            
            //�������Ƿ����峤���ڣ����ޣ���ʾ�û�ָ���峤
            if (TreeOfThisFile.chiefIndex == -1 || TreeOfThisFile.redundantIndexes.Contains(TreeOfThisFile.chiefIndex))
            {
                TransferPrompt = findPrompt("exitWithoutChief");
                prompt?.SetActive(true);
                prompt.GetComponent<PromptManage>().PromptAppear();
                return;
            }

            //����������ʾ��ұ���
            exitDialog.SetActive(true);
        });

        exitComfirm.onClick.AddListener(delegate
        {
            exitDialog.SetActive(false);

            //��������
            focusCircle.GetComponent<FocusCircle>().clearMembers();

            StartCoroutine(nameof(LoadSceneAsync), "MainScene");
        });

        exitCencel.onClick.AddListener(delegate
        {
            //���ȡ����رյ���
            exitDialog.SetActive(false);
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
