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
            //检查编辑区是否有成员存在，若有，提示用户先行处理之
            if (AreaTransfer.memberQuantityOfAreas[1] == 1)
            {
                TransferPrompt = findPrompt("exitWithEditor");
                prompt?.SetActive(true);
                prompt.GetComponent<PromptManage>().PromptAppear();
                return;
            }
            
            //检查家族是否有族长存在，若无，提示用户指定族长
            if (TreeOfThisFile.chiefIndex == -1 || TreeOfThisFile.redundantIndexes.Contains(TreeOfThisFile.chiefIndex))
            {
                TransferPrompt = findPrompt("exitWithoutChief");
                prompt?.SetActive(true);
                prompt.GetComponent<PromptManage>().PromptAppear();
                return;
            }

            //弹出弹窗提示玩家保存
            exitDialog.SetActive(true);
        });

        exitComfirm.onClick.AddListener(delegate
        {
            exitDialog.SetActive(false);

            //事先清屏
            focusCircle.GetComponent<FocusCircle>().clearMembers();

            StartCoroutine(nameof(LoadSceneAsync), "MainScene");
        });

        exitCencel.onClick.AddListener(delegate
        {
            //玩家取消后关闭弹窗
            exitDialog.SetActive(false);
        });
    }

    //场景加载页面方法，来自网络
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
        {
            Debug.Log("Scene is loaded : " + sceneName);
            yield break;
        }

        //可在此显示Loading界面
        //如果用LoadSceneMode.Single方式加载，需要单独做一个Loading场景，用于切场景过渡
        //这里使用LoadSceneMode.Additive方式，所有的UI界面都在主场景，场景过渡时使用Loading界面即可
        loadPanel.SetActive(true);

        float startTime = Time.time;

        //选择场景加载方式
        //LoadSceneMode.Single会卸载掉之前的原有的场景，只保留新加载场景
        //LoadSceneMode.Additive，会在原有场景的基础上附加新场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        //AsyncOperation.progress范围(0~1)
        //isDone为false时，最大加载到0.9就会暂停，直到isDone为true时才会继续加载剩余的0.9 - 1.0
        //只有allowSceneActivation = true，isDone才会在progress = 1.0后，值为true
        //作用是让场景不会在加载完成后自动跳转到下一个场景
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            //手动延长加载时间，防止加载界面展示时间过短
            if (Time.time - startTime >= loadWaitTime)
            {
                if (operation.progress >= 0.9f && !operation.allowSceneActivation)
                    operation.allowSceneActivation = true;
            }
            yield return null;
        }

        //如果不需要等待，可直接加载后跳转场景
        //operation.allowSceneActivation = true;
        //while (!operation.isDone)
        //{
        //    yield return null;
        //}

        //可在此关闭Loading界面
        loadPanel.SetActive(true);

        //加载完成，可以设置回调
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
