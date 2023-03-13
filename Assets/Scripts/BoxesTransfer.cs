using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxesTransfer : AreaTransfer
{
    public int capcityOfBox; //�����ӵ�����

    private int quantityOfBox; //�������ڵĳ�Ա����

    public GameObject quantityDisplayer; //ʵʱ��ʾ�����ڳ�Ա������UI�������ı�ʱ��֪ͨ�������޸�

    private int presentPage; //��ǰ���ӵ�ҳ�룬��0��ƣ���������ӳ�Աʱ�ı䣬���ɽ��Ұ�ť��ҳ

    private int destPositionNo; //���������������³�Ա��Ŀ��ص��ţ���0���

    private List<GameObject> memberInBox; //ÿҳ����ĳ�Ա

    private int pageAccess = 0; //��ǰ�ɷ��ʵ�ҳ��������0���

    private int completionStart; //ɾ��ĳ��Աʱ�����ĵ�һ����Աλ�ã����ڰ��Ų�λ��Ĭ�ϲ���Ҫ��λΪ-1

    private float completingSpeed = 0.5f;

    //����Ϊ��Ա���ڵ��ĸ���Ա��λ���ֲ������ϡ����ϡ����º����·�
    private Vector3[] destPositions = new Vector3[4];

    //����Ϊ��Ա�е��ĸ���ҳ����
    public Button page0;

    public Button page1;

    public Button page2;

    public Button page3;

    private void Awake()
    {
        //�ڼ��س�Ա��ʱ��ȷ�����������ֵܳ�Ա��Ϊ15�����ӳ�Ա��Ϊ16��ͬʱҲ��ʼ�����б���
        if (name == "BrotherBox") memberInBox = new List<GameObject>(15);
        else memberInBox = new List<GameObject>(16);
        quantityOfBox = 0;

        for(int i = 0; i <= 3; i++)
            destPositions[i] = transform.Find($"No{i}Position").position;

        presentPage = 0;
        completionStart = -1;
        pageAccess = 0;

        //Ϊҳ�뽺�ҵĵ���󶨷�ҳ����
        page0.onClick.AddListener(delegate
        {
            pageChangeTo(0);
        });
        page1.onClick.AddListener(delegate
        {
            pageChangeTo(1);
        });
        page2.onClick.AddListener(delegate
        {
            pageChangeTo(2);
        });
        page3.onClick.AddListener(delegate
        {
            pageChangeTo(3);
        });
    }

    //ҳ���������������Ŀ��ҳ�뼴�������Ķ���
    //�÷����Ὣ��ǰҳ��ĳ�Ա��ʱͣ�ã�ʵ�����أ��ٰ���ҳ��ĳ�Ա���ã�ʵ�ִ�ӡ
    public void pageChangeTo(int pageToGo)
    {
        //�����ǰҳ�����Ҫ����ʵ�ҳ�룬����Ҫ��ҳ
        if (presentPage == pageToGo)
        {
            Debug.Log("([ҳ�����])��" + name + "��ǰҳ���Ѿ�Ϊ" + pageToGo + "��δִ�з�ҳ");
            return;
        }
        //���Ҫ����ʵ�ҳ�볬���ɷ���ҳ�룬��ֱ�ӷ���
        if (pageAccess < pageToGo)
        {
            Debug.Log("([ҳ�����])��" + name + "���󵽴��ҳ��" + pageToGo + "Խ�磬δִ�з�ҳ");
            return;
        }
        for (int i = 0; i <= 3; i++)
        {
            if (memberInBox.Count > presentPage * 4 + i) LayerChange(memberInBox[presentPage * 4 + i], 6);
            if (memberInBox.Count > pageToGo * 4 + i) LayerChange(memberInBox[pageToGo * 4 + i], 3);
        }
        presentPage = pageToGo;
        Debug.Log("([ҳ�����])��" + name + "�ɹ���ҳ��ҳ��" + presentPage);
    }

    public override void getMember(GameObject memberIn, bool attaching = true)
    {
        memberInBox.Add(memberIn);
        if (name == "BrotherBox")
        {
            quantityOfBox = ++memberQuantityOfAreas[4];
            if (attaching)
            {
                Debug.Log("[��Ե��ϵ���]����Ա���" + memberIn.GetComponent<MemberData>().memberData +
                    "��Ϊ�˾۽���Ա" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "���ֵ�");
                TreeOfThisFile.MemberJoinAsBrother(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[��Ե��ϵ��ӡ]����Ա���" + memberIn.GetComponent<MemberData>().memberData + "�����ֵܺ�");
        }
        else {
            quantityOfBox = ++memberQuantityOfAreas[5];
            if (attaching)
            {
                Debug.Log("[��Ե��ϵ���]����Ա���" + memberIn.GetComponent<MemberData>().memberData +
                    "��Ϊ�˾۽���Ա" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "�ĺ���");
                TreeOfThisFile.MemberJoinAsChild(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[��Ե��ϵ��ӡ]����Ա���" + memberIn.GetComponent<MemberData>().memberData + "���뺢�Ӻ�");
        }

        //ֻ���ڵ�ǰҳ������ʱ��Żᷭҳ
        if (destPositionNo == 0 && quantityOfBox != 1)
        {
            pageAccess++;
            pageChangeTo(presentPage + 1);
        }

        quantityDisplayer.GetComponent<BoxQuantitiesReport>().quantityAdd();
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        //�뿪ʱ�븸�׶Ͼ���Ե��ϵ
        if (name == "BrotherBox")
        {
            quantityOfBox = --memberQuantityOfAreas[4];
            if (leaving)
            {
                Debug.Log("[��Ե��ϵ�Ͼ�]����Ա���" + memberOut.GetComponent<MemberData>().memberData +
                         "�����Ǿ۽���Ա" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "���ֵ�");
                TreeOfThisFile.MemberSeverRelationAsChild(memberOut.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().getData().father);
            }
        }
        else
        {
            quantityOfBox = --memberQuantityOfAreas[5];
            if (leaving)
            {
                Debug.Log("[��Ե��ϵ�Ͼ�]����Ա���" + memberOut.GetComponent<MemberData>().memberData +
                         "�����Ǿ۽���Ա" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "�ĺ���");
                TreeOfThisFile.MemberSeverRelationAsChild(memberOut.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
        }


        //ɾ����Աʱ�����뿪�ĳ�Աλ�ò�������Ҫ���Ÿó�Ա�Ժ�ĳ�Ա��ʱ��λ�������Ա���л���
        //�����뿪��Ա��λ�ã������ֿ��ܵ������
        //1.�뿪�ĳ�Ա�����һλ����������Ա��λ�ú�ҳ�����Ӱ�죬ֻ��ɾ��֮
        //2.�뿪�ĳ�Ա��һ��ҳ����ĸ���Ա֮�У�������Ա��Ҫ����λ�õ�����
        //ͬʱ����һ��ҳ���г�Ա����Ҫ����㼶��Ϊ��Ա���ټ��뱾ҳ�棬�����漰��ҳ���ɾ��
        //�Դ����ƣ����¸�ҳ�������г�Ա��Ҳ��Ҫ�����¸�ҳ�棬������漰ҳ���ɾ��
        //3.�뿪�ĳ�Ա��ĳ��ҳ���Ψһһ����Ա�������ҳ��֮ǰ������ҳ�棬��Ҫ���ط�ҳ��ͬʱɾ����ҳ��

        Debug.Log("([���ӳ�Ա��λ])��" + name + "������λ����...");
        if (memberOut == memberInBox[memberInBox.Count - 1])
        {
            //�������1��3
            memberInBox.RemoveAt(memberInBox.Count - 1);
            if (memberInBox.Count % 4 == 0 && presentPage != 0)
            {
                pageAccess--;
                pageChangeTo(presentPage - 1);
            }
        }
        else memberCompletion(memberOut);

        quantityDisplayer.GetComponent<BoxQuantitiesReport>().quantityReduce();
    }

    //��Ա��λ�������������2
    private void memberCompletion(GameObject memberOut)
    {
        for (int i = 0; i < memberInBox.Count - 1; i++)
        {
            //���ҵ���λ�ĳ���λ��,�ٿ�ʼ�������Ա��ǰ�ƶ�
            if (memberInBox[i] == memberOut) completionStart = i;
            if (completionStart != -1)
            {
                //ֱ�ӽ�ÿ����Ա���Ƶ���ǰһ��λ��
                memberInBox[i] = memberInBox[i + 1];

                //һ������ǣ��ó�Ա������һҳ����Ȼ�����ز㣬��Ҫ������ı�㼶��������ҳ���ɾ��
                if (i % 4 == 3)
                {
                    if (memberInBox[i].layer == 6) LayerChange(memberInBox[i + 1], 3);
                    StartCoroutine(memberAdsorb(memberInBox[i + 1], destPositions[i % 4]));
                    if (i + 2 == memberInBox.Count) pageAccess--;
                }
                //���򣬸ó�Ա���ڵ�ǰ���ڴ����ҳ�棨��һ���ڻ״̬����ֻ��Ҫ�ƶ���ǡ��λ��
                else
                {
                    StartCoroutine(memberAdsorb(memberInBox[i + 1], destPositions[i % 4]));
                }
            }
        }

        //��λ��ɺ󣬽���λ���������-1��������ٲ�λ���������ʣ��ĳ�Ա���
        completionStart = -1;
        memberInBox.RemoveAt(memberInBox.Count - 1);
    }

    //��ִ�в�λʱ������ɳ�Ա�ƶ��ķ������������ƶ��ĳ�Ա��Ŀ�ĵؼ���
    private IEnumerator memberAdsorb(GameObject memberToMove, Vector3 destPosition)
    {
        Debug.Log("([���ӳ�Ա��λ])����Ա���" + name + "������Ŀ�ĵر��" + destPosition);
        while (memberToMove.transform.position != destPosition)
        {
            Vector3 dir = destPosition - memberToMove.transform.position;
            memberToMove.transform.position += dir * completingSpeed;
            yield return new WaitForFixedUpdate();
        }
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //����Ǵӱ������ڳ����������������س�����
            if (memberChecking.GetComponent<MemberMovement>().DepArea == gameObject)
            {
                return memberChecking.GetComponent<MemberMovement>().PointOfDeparture;
            }

            //��������㲻�Ǳ༭������ô��Ϊ�ƶ����Ϸ�
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name != "EditorArea")
            {
                TransferPrompt = findPrompt("illegal");
                return rejectVector;
            }

            //ֻ�е�����Ȧ�д��ڳ�Աʱ������Ա�������ֵ�
            if (name == "BrotherBox" && memberQuantityOfAreas[2] == 0)
            {
                TransferPrompt = findPrompt("No2to4");
                return rejectVector;
            }

            //���������������޷������³�Ա
            if (quantityOfBox == capcityOfBox)
            {
                if (name == "BrotherBox") TransferPrompt = findPrompt("4Full");
                else TransferPrompt = findPrompt("5Full");
                return rejectVector;
            }
            if (name == "BrotherBox" && memberQuantityOfAreas[2] != 1) return rejectVector;
        }

        //�ڳ�Ա�Ų����루�м�û�п�λ��������£����Խ�ҳ�淭�����Ŀ���ҳ�桢ֱ�������Ŀ���λ������
        pageChangeTo(pageAccess);
        destPositionNo = quantityOfBox % 4;
        return destPositions[destPositionNo];
    }

    public void updateMember(List<int> memberUpdating)
    {
        if (memberUpdating.Count == 0) Debug.Log("[�۽�Ȧ�����ĳ�Ա����]��" + name + "���º�Ϊ��");
        else Debug.Log("[�۽�Ȧ�����ĳ�Ա����]��" + name + "ִ�и��£��³�Ա����" + memberUpdating.Count + "��...");
        
        
        /*int oldMemberCount = memberInBox.Count;
        for (int i = 0; i < memberUpdating.Count; i++)
        {
            //�����¼���ĳ�Ա�г���ԭ�г�Ա���Ĳ��֣���Ҫ�½���Ա�����Ա��֪Ϥ
            if (i >= oldMemberCount)
            {
                sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject, memberUpdating[i]);
            }
            //����ֻ��Ҫ�޸�ԭ�г�Ա��Ӧ������
            else
            {
                memberInBox[i].GetComponent<MemberData>().updateData(memberUpdating[i]);
            }
        }
        oldMemberCount = memberInBox.Count;
        //�����³�Ա����ԭ�г�Ա���Ĳ��֣�����Ҫ��ԭ��Աѹ�����أ���������ͷ�ԭ�еĳ�Ա
        for (int i = memberUpdating.Count; i < oldMemberCount; i++)
        {
            sceneAgent.GetComponent<MemberPool>().PoolPush(memberInBox[memberUpdating.Count], false);
            releaseMember(memberInBox[memberUpdating.Count]);
        }*/


        //ֱ�ӽ�ԭ�еĳ�Աȫ����������м����³�Ա
        int oldMemberCount = memberInBox.Count;
        for (int i = oldMemberCount - 1; i >= 0; i--)
        {
            sceneAgent.GetComponent<MemberPool>().PoolPush(memberInBox[i], false);
            releaseMember(memberInBox[i]);
        }
        for(int i = 0; i < memberUpdating.Count; i++)
            sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject, memberUpdating[i]);
    }
}
