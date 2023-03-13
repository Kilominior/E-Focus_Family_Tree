using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeleteManage : BaseClasses
{
    public GameObject MemberToDelete; //将被删除的成员，在碰撞发生时存入

    private bool readyToDelete = false; //用于判定成员是否留在删除器等待，当成员进入删除区达到约定时间时删除成员

    public UnityEvent DeleteDialogComfirm; //负责删除成员，会使游戏暂停，并唤出删除确认弹窗

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MemberToDelete = collision.gameObject;
        readyToDelete = true;
        StartCoroutine(nameof(WaitAndWake));
    }

    //等待约定的时间，若成员在约定时间后仍悬于删除器上，则认为用户确定了需要进行删除
    private IEnumerator WaitAndWake()
    {
        yield return new WaitForSeconds(3f);
        if (readyToDelete)
        {
            //先根据欲删除成员的出发地点决定是否给用户发出额外删除警告
            switch(MemberToDelete.GetComponent<MemberMovement>().DepArea.name)
            {
                case "FatherCircle":
                    DeletePrompt = findPrompt("2Del");
                    break;
                case "GrandpaCircle":
                    DeletePrompt = findPrompt("3Del");
                    break;
                case "BrotherBox":
                    DeletePrompt = findPrompt("haveSonDel");
                    break;
                case "ChildrenBox":
                    DeletePrompt = findPrompt("haveSonDel");
                    break;
                default:
                    //由于警告无论如何都将被设为活动状态，因此若不需要警告，就先将其设置为空，便不可见
                    DeletePrompt = string.Empty;
                    break;
            }
            DeleteDialogComfirm.Invoke();
        }
    }

    //用户确认执行删除时，触发删除事件
    public void DeleteExecute()
    {
        MemberToDelete.GetComponent<MemberMovement>().DepArea.GetComponent<AreaTransfer>().releaseMember(MemberToDelete);
        sceneAgent.GetComponent<MemberPool>().PoolPush(MemberToDelete);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //若成员中途退出，则认为用户取消了删除
        readyToDelete = false;
    }
}
