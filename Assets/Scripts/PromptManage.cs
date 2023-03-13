using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptManage : BaseClasses
{
    private float stayTime = 2f; //提示显示后停留的时间

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
