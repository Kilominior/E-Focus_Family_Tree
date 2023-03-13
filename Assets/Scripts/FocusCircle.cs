using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FocusCircle : AreaTransfer
{
    public GameObject focusMember;

    //����Ϊ���¾۽���Աʱ����֪ͨ������������������Ա���µ�Ŀ������
    public GameObject fatherCircle;

    public GameObject grandpaCircle;

    public GameObject brotherBox;

    public GameObject childrenBox;

    private int memberIndexUpdating;

    private FamilyMember memberDataUpdating;

    public override void getMember(GameObject memberIn, bool attaching = true)
    {
        if (memberQuantityOfAreas[0] == 1) releaseMember(focusMember);
        else memberQuantityOfAreas[0] = 1;
        focusMember = memberIn;

        //֪ͨ��Ϣ�༭�����ڱ༭���Ǿ۽���Ա����Ϣ
        canvas.GetComponent<EditingBehaviors>().EditorChange(false);

        //֪ͨ������������������Ա����
        memberIndexUpdating = focusMember.GetComponent<MemberData>().memberData;
        Debug.Log("[[�۽�Ȧ]]��Ϊ��Ա���" + memberIndexUpdating + "������Ա���£���ʼ֪ͨ����������...");
        memberDataUpdating = TreeOfThisFile.MembersOfThisTree[memberIndexUpdating];
        fatherCircle.GetComponent<ElderTransfer>().updateMember(memberDataUpdating.father);
        grandpaCircle.GetComponent<ElderTransfer>().updateMember(TreeOfThisFile.GetGrandpa(memberIndexUpdating));
        brotherBox.GetComponent<BoxesTransfer>().updateMember(TreeOfThisFile.GetBrothers(memberIndexUpdating));
        childrenBox.GetComponent<BoxesTransfer>().updateMember(TreeOfThisFile.GetChildren(memberIndexUpdating));

        //ͨ���޸Ĳ�����������Ȧ�г�Ա���ƶ�
        focusMember.GetComponent<MemberMovement>().isBound = true;
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        //���������Ա���ƶ�
        memberOut.GetComponent<MemberMovement>().isBound = false;
        //���ڳ�Ա���滻�����ö���ص�push������ԭ�еĳ�Ա����ɾ��������ɾ����Ӧ��Ϣ
        sceneAgent.GetComponent<MemberPool>().PoolPush(memberOut, false);
    }

    //ϵͳ��ʾ������ȫ����Ա����������Ҳ���������ڷ�����ҳ���������ļ�ǰ������
    public void clearMembers()
    {
        Debug.Log("[[�۽�Ȧ]]����ʼִ����������");
        //����һ���³�Աȡ��ԭ�еľ۽���Ա�����ڸó�Աû���κ��������������򶼽�����Ϊ��
        sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject);
        //����ɾ���´����ĳ�Ա��ʹ�۽�ȦҲΪ��
        sceneAgent.GetComponent<MemberPool>().PoolPush(focusMember);
        sceneAgent.GetComponent<MemberPool>().PoolClear();
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //�����������ڱ༭������Ϊ�ƶ����Ϸ�
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name == "EditorArea")
            {
                TransferPrompt = findPrompt("1To0");
                return rejectVector;
            }

            //���༭���г�Ա���ڣ���Ϊ�ƶ����Ϸ�
            if (memberQuantityOfAreas[1] == 1)
            {
                TransferPrompt = findPrompt("1FullTo0");
                return rejectVector;
            }
        }
        return transform.position;
    }
}





