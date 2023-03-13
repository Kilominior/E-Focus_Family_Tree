using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberPool : BaseClasses
{
    private static List<GameObject> poolList = new();

    private readonly int PoolCapacity = 60;

    private GameObject LastMember;

    public GameObject Camera; 

    private Vector3 initialPosition;

    /// <summary>
    /// �����Ա��ɾ��֮���ڶ�������Ĭ��Ϊ�棬��ɾ����Ա�����ݣ�������٣�����ɾ�����岻ɾ����Ӧ����.
    /// </summary>
    public void PoolPush(GameObject theMember, bool isDelete = true)
    {
        theMember.SetActive(false);
        if (isDelete)
        {
            Debug.Log("<<�����>>������ɾ���ĳ�Ա����ǣ�" + LastMember.GetComponent<MemberData>().memberData);
            int memberIndex = theMember.GetComponent<MemberData>().memberData;
            
            //����������������ʱ��Ա����ɾ���䱾��
            if(theMember.GetComponent<MemberMovement>().DepArea == null)
                TreeOfThisFile.SubTreeRemoveWithCheck(memberIndex);
            else
            {
                //�������Բ�ͬ����ĳ�Ա�����ò�ͬ��ɾ������
                switch (theMember.GetComponent<MemberMovement>().DepArea.name)
                {
                    case "FatherCircle":
                        TreeOfThisFile.MemberRemoveFatherWithAncestor(focusCircle.GetComponent<FocusCircle>().
                            focusMember.GetComponent<MemberData>().memberData);
                        break;
                    case "GrandpaCircle":
                        TreeOfThisFile.MemberRemoveFatherWithAncestor(TreeOfThisFile.MembersOfThisTree[focusCircle.
                            GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData].father);
                        break;
                    case "BrotherBox":
                        TreeOfThisFile.SubTreeRemoveWithCheck(memberIndex);
                        break;
                    case "ChildrenBox":
                        TreeOfThisFile.SubTreeRemoveWithCheck(memberIndex);
                        break;
                    default:
                        //������ʱ�����ĳ�Ա���������Ա༭��������ɾ���䱾��
                        TreeOfThisFile.SubTreeRemoveWithCheck(memberIndex);
                        break;
                }
            }
        }
        else Debug.Log("<<�����>>���������صĳ�Ա����ǣ�" + theMember.GetComponent<MemberData>().memberData);

        //ɾ����Աʱɾ����ű��Ͷ�Ӧ����Ϣ
        Destroy(theMember.GetComponent<MemberMovement>());
        if (poolList.Count < PoolCapacity) poolList.Add(theMember);
        else Destroy(theMember);
    }

    /// <summary>
    /// �����ʼ���ڵĹ��������ڸ�����ָ��λ�ô����³�Ա,
    /// ��������ж����������㣬�ͽ�������Ԥ����.
    /// </summary>
    public void PoolPop(GameObject InitialArea, int memberIndex = -1)
    {
        //���ڴӱ༭���½��ĳ�Ա��ָ�����ʼλ��Ϊ�ض�λ��
        initialPosition = InitialArea.GetComponent<AreaTransfer>().destAppoint(InitialArea, true);
        if (poolList.Count > 0)
        {
            Debug.Log("<<�����>>���Ӷ������ȡ����Ա...");
            LastMember = poolList[0];
            poolList.RemoveAt(0);
            //������Աʱ���¹��ؽű���ʵ����ֵ�ĳ�ʼ��
            LastMember.AddComponent<MemberMovement>();
            LastMember.transform.position = initialPosition;
            //����Ա�϶�Ŧ�Ĵ�С���Ĭ�ϣ�����ɾ����ɾ�����µĴ�С�仯
            LastMember.transform.Find("DragButton").localScale = new(0.3f, 0.3f);
            LastMember.SetActive(true);
        }
        else
        {
            Debug.Log("<<�����>>���½���Ա��������...");
            //����Դ��ʵ����һ����Ա
            LastMember = Instantiate(Resources.Load<GameObject>("Member"), initialPosition, new Quaternion());

            //Ϊ��Ա�Ļ�����Ļ�ռ�������
            LastMember.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.GetComponent<Camera>();

        }
        //��ǰ���ƺó�Ա�Ŀ��ƶ���
        if (InitialArea.name == "FocusCircle") LastMember.GetComponent<MemberMovement>().isBound = true;
        else LastMember.GetComponent<MemberMovement>().isBound = false;

        //Ϊ��Ա��������������ֵδ��������������������������³�Ա��ִ�а�
        if (memberIndex != -1) LastMember.GetComponent<MemberData>().memberData = memberIndex;
        else LastMember.GetComponent<MemberData>().memberData = TreeOfThisFile.MemberCreate();

        //��ӡ��Ա��������ż���֣������Ա������Ա��ͼ
        LastMember.GetComponent<MemberData>().fullnamePrintI();
        LastMember.GetComponent<MemberData>().fullnamePrintS();
        LastMember.GetComponent<MemberData>().sexPrint();

        //֪ͨ���յ��³�Ա����������ԱΪ�½����������Ե��ϵ
        if (memberIndex != -1) InitialArea.GetComponent<AreaTransfer>().getMember(LastMember, false);
        else InitialArea.GetComponent<AreaTransfer>().getMember(LastMember);
        Debug.Log("<<�����>>����ǰ�õ��ĳ�Ա����ǣ�" + LastMember.GetComponent<MemberData>().memberData);
    }

    public void PoolClear()
    {
        poolList.Clear();
    }
}
