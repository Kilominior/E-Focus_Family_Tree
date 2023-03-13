using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptManage : BaseClasses
{
    private float stayTime = 2f; //��ʾ��ʾ��ͣ����ʱ��

    public void PromptAppear()
    {
        GetComponent<TextMeshProUGUI>().text = TransferPrompt;
        StartCoroutine(nameof(PromptStay));
    }

    public void PromptKeep()
    {
        GetComponent<TextMeshProUGUI>().text = DeletePrompt;
    }

    private IEnumerator PromptStay()
    {
        yield return new WaitForSecondsRealtime(stayTime);
        gameObject.SetActive(false);
    }
}
