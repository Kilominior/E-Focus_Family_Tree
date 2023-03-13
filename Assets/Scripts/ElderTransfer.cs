using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElderTransfer : AreaTransfer
{
    public GameObject theElder;

    public override void getMember(GameObject memberIn, bool attaching = true)
    {
        theElder = memberIn;
        if (name == "FatherCircle")
        {
            memberQuantityOfAreas[2] = 1;
            if (attaching)
            {
                Debug.Log("[��Ե��ϵ���]����Ա���" + memberIn.GetComponent<MemberData>().memberData +
                "��Ϊ�˾۽���Ա" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "�ĸ���");
                TreeOfThisFile.MemberJoinAsFather(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[��Ե��ϵ��ӡ]����Ա���" + memberIn.GetComponent<MemberData>().memberData + "���븸��Ȧ");
        }
        else
        {
            memberQuantityOfAreas[3] = 1;
            if (attaching)
            {
                Debug.Log("[��Ե��ϵ���]����Ա���" + memberIn.GetComponent<MemberData>().memberData +
                     "��Ϊ�˾۽���Ա" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "��үү");
                TreeOfThisFile.MemberJoinAsGrandpa(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[��Ե��ϵ��ӡ]����Ա���" + memberIn.GetComponent<MemberData>().memberData + "����үүȦ");
        }
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        if (name == "FatherCircle" && memberQuantityOfAreas[2] == 1) Debug.Log("����ǰ����Ȧԭ���ĳ�Ա�к�����" + theElder.GetComponent<MemberData>().getData().children.Count);
        if (name == "FatherCircle") memberQuantityOfAreas[2] = 0;
        else memberQuantityOfAreas[3] = 0;
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //����Ǵӱ������ڳ����������������س�����
            if (memberChecking.GetComponent<MemberMovement>().DepArea == gameObject)
                return memberChecking.GetComponent<MemberMovement>().PointOfDeparture;

            //��������㲻�Ǳ༭������ô��Ϊ�ƶ����Ϸ�
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name != "EditorArea")
            {
                TransferPrompt = findPrompt("illegal");
                return rejectVector;
            }

            //��үүȦ�Ѿ��г�Ա��Ҳ��Ϊ���Ϸ�
            if (name == "FatherCircle" && memberQuantityOfAreas[2] == 1)
            {
                TransferPrompt = findPrompt("2Full");
                return rejectVector;
            }

            //�������Ȧû�г�Ա���޷���үүȦ��ӳ�Ա
            if (name == "GrandpaCircle" && memberQuantityOfAreas[2] == 0)
            {
                TransferPrompt = findPrompt("No2to3");
                return rejectVector;
            }

            //��үүȦ�Ѿ��г�Ա��Ҳ��Ϊ���Ϸ�
            if (name == "GrandpaCircle" && memberQuantityOfAreas[3] == 1)
            {
                TransferPrompt = findPrompt("3Full");
                return rejectVector;
            }
        }

        return transform.position;
    }

    public void updateMember(int memberUpdating)
    {
        if(memberUpdating == -1) Debug.Log("[�۽�Ȧ�����ĳ�Ա����]��" + name + "���ڲ����г�Ա");
        else Debug.Log("[�۽�Ȧ�����ĳ�Ա����]��" + name + "ִ�и��£��³�Ա���Ϊ" + memberUpdating);
        //���Ȧ�����г�Ա�������Ծɻ���
        int circleQuantity;
        if (name == "FatherCircle") circleQuantity = memberQuantityOfAreas[2];
        else circleQuantity = memberQuantityOfAreas[3];
        if (circleQuantity == 1)
        {
            if (memberUpdating != -1)
                theElder.GetComponent<MemberData>().updateData(memberUpdating);
            else
            {
                releaseMember(theElder);
                sceneAgent.GetComponent<MemberPool>().PoolPush(theElder, false);
            }
        }
        //��������Ҫ�����Ա���ɶ�����Ƴ�
        else
        {
            if (memberUpdating != -1)
            {
                sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject, memberUpdating);
            }
        }
    }
}
