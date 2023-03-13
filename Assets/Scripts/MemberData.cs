using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemberData : BaseClasses
{
    public int memberData; //��Ա��Ӧ����������������Ա�������ϣ���Ϊ-1

    public TMP_Text mainMemberName;

    public TMP_Text spouseName;

    //��Ա���ݻ�ȡ���������ظó�Ա��Ӧ�Ľڵ���Ϣ
    public FamilyMember getData()
    {
        return TreeOfThisFile.MembersOfThisTree[memberData];
    }

    //��Ա���ݸ��·��������޸������Ӧ��index���Ӷ�ʵ�ָ��³�Ա
    public void updateData(int dataNew)
    {
        memberData = dataNew;
        //���µ�ͬʱ����ӡ�³�Ա����Ϣ
        fullnamePrintI();
        fullnamePrintS();
        sexPrint();
    }

    //��Ա���ַ�����ӡ
    public void fullnamePrintI()
    {
        mainMemberName.text = getData().surname + getData().name;
    }

    public void fullnamePrintS()
    {
        spouseName.text = getData().spouseSurname + getData().spouseName;
    }

    //��Ա�Ա��ӡ������ͨ���ı���ͼʵ��
    public void sexPrint()
    {
        if (getData().sex) ManMainPrint();
        else WomanMainPrint();
    }

    public void ManMainPrint()
    {
        transform.Find("MemberDetail").GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("Sprites/manMain");
    }

    public void WomanMainPrint()
    {
        transform.Find("MemberDetail").GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("Sprites/womanMain");
    }
}