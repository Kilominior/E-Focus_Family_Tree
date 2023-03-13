using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EditorArea : AreaTransfer
{
    public GameObject memberEditing;

    public Vector3 EditorPosition = new(0, -3.5f); //��Ա�ڳ�Ա�༭���е�ʵ��λ��

    //�³�Ա�Ĵ������������½���ť����
    //����༭����û�г�Ա����ô�����ť���Դ���һ���³�Ա
    public void CreateMember()
    {
        if (memberQuantityOfAreas[1] == 0) sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject);
    }

    public override void getMember(GameObject memberIn, bool attaching = true)
    {
        memberQuantityOfAreas[1] = 1;
        memberEditing = memberIn;
        //��ó�Աʱ֪ͨ��������༭���еĳ�Ա��Ϊ�����Ĭ������
        canvas.GetComponent<EditingBehaviors>().EditorChange(true);
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        memberQuantityOfAreas[1] = 0;
        //��Ա�ͳ�ʱ֪ͨ��������۽�Ȧ�еĳ�Ա��Ϊ�����Ĭ������
        canvas.GetComponent<EditingBehaviors>().EditorChange(false);
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //����Ǵӱ������ڳ����������������س�����
            if (memberChecking.GetComponent<MemberMovement>().DepArea == gameObject)
                return memberChecking.GetComponent<MemberMovement>().PointOfDeparture;

            //���༭���Ѿ��г�Ա���ڣ����޷��ټ����Ա
            if (memberQuantityOfAreas[1] == 1)
            {
                TransferPrompt = findPrompt("1Full");
                return rejectVector;
            }

            //����Ȧ��үүȦ�ĳ�Ա���ܽ���༭������Ϊ���ǿ����ھ۽�Ȧ���༭
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name == "FatherCircle" ||
                memberChecking.GetComponent<MemberMovement>().DepArea.name == "GrandpaCircle")
            {
                TransferPrompt = findPrompt("illegal");
                return rejectVector;
            }

            //�ֵ�Ȧ�ͺ���Ȧ��ĳ�Ա����༭�������������Լ�û�к��Ӵ���
            if (memberChecking.GetComponent<MemberData>().getData().children.Count != 0)
            {
                TransferPrompt = findPrompt("haveSonTo1");
                return rejectVector;
            }
        }

        return EditorPosition;
    }
}