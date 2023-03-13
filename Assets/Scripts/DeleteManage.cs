using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeleteManage : BaseClasses
{
    public GameObject MemberToDelete; //����ɾ���ĳ�Ա������ײ����ʱ����

    private bool readyToDelete = false; //�����ж���Ա�Ƿ�����ɾ�����ȴ�������Ա����ɾ�����ﵽԼ��ʱ��ʱɾ����Ա

    public UnityEvent DeleteDialogComfirm; //����ɾ����Ա����ʹ��Ϸ��ͣ��������ɾ��ȷ�ϵ���

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MemberToDelete = collision.gameObject;
        readyToDelete = true;
        StartCoroutine(nameof(WaitAndWake));
    }

    //�ȴ�Լ����ʱ�䣬����Ա��Լ��ʱ���������ɾ�����ϣ�����Ϊ�û�ȷ������Ҫ����ɾ��
    private IEnumerator WaitAndWake()
    {
        yield return new WaitForSeconds(3f);
        if (readyToDelete)
        {
            //�ȸ�����ɾ����Ա�ĳ����ص�����Ƿ���û���������ɾ������
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
                    //���ھ���������ζ�������Ϊ�״̬�����������Ҫ���棬���Ƚ�������Ϊ�գ��㲻�ɼ�
                    DeletePrompt = string.Empty;
                    break;
            }
            DeleteDialogComfirm.Invoke();
        }
    }

    //�û�ȷ��ִ��ɾ��ʱ������ɾ���¼�
    public void DeleteExecute()
    {
        MemberToDelete.GetComponent<MemberMovement>().DepArea.GetComponent<AreaTransfer>().releaseMember(MemberToDelete);
        sceneAgent.GetComponent<MemberPool>().PoolPush(MemberToDelete);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //����Ա��;�˳�������Ϊ�û�ȡ����ɾ��
        readyToDelete = false;
    }
}
