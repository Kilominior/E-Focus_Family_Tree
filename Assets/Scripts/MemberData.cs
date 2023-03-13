using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MemberData : BaseClasses
{
    public int memberData; //成员对应的数据索引，若成员不在树上，则为-1

    public TMP_Text mainMemberName;

    public TMP_Text spouseName;

    //成员数据获取方法，返回该成员对应的节点信息
    public FamilyMember getData()
    {
        return TreeOfThisFile.MembersOfThisTree[memberData];
    }

    //成员数据更新方法，会修改物体对应的index，从而实现更新成员
    public void updateData(int dataNew)
    {
        memberData = dataNew;
        //更新的同时将打印新成员的信息
        fullnamePrintI();
        fullnamePrintS();
        sexPrint();
    }

    //成员名字方法打印
    public void fullnamePrintI()
    {
        mainMemberName.text = getData().surname + getData().name;
    }

    public void fullnamePrintS()
    {
        spouseName.text = getData().spouseSurname + getData().spouseName;
    }

    //成员性别打印方法，通过改变贴图实现
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