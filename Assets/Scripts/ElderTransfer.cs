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
                Debug.Log("[亲缘关系添加]：成员编号" + memberIn.GetComponent<MemberData>().memberData +
                "成为了聚焦成员" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "的父亲");
                TreeOfThisFile.MemberJoinAsFather(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[亲缘关系打印]：成员编号" + memberIn.GetComponent<MemberData>().memberData + "进入父亲圈");
        }
        else
        {
            memberQuantityOfAreas[3] = 1;
            if (attaching)
            {
                Debug.Log("[亲缘关系添加]：成员编号" + memberIn.GetComponent<MemberData>().memberData +
                     "成为了聚焦成员" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "的爷爷");
                TreeOfThisFile.MemberJoinAsGrandpa(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[亲缘关系打印]：成员编号" + memberIn.GetComponent<MemberData>().memberData + "进入爷爷圈");
        }
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        if (name == "FatherCircle" && memberQuantityOfAreas[2] == 1) Debug.Log("更新前父亲圈原来的成员有孩子数" + theElder.GetComponent<MemberData>().getData().children.Count);
        if (name == "FatherCircle") memberQuantityOfAreas[2] = 0;
        else memberQuantityOfAreas[3] = 0;
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //如果是从本区域内出发，则无条件返回出发点
            if (memberChecking.GetComponent<MemberMovement>().DepArea == gameObject)
                return memberChecking.GetComponent<MemberMovement>().PointOfDeparture;

            //如果出发点不是编辑区，那么认为移动不合法
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name != "EditorArea")
            {
                TransferPrompt = findPrompt("illegal");
                return rejectVector;
            }

            //若爷爷圈已经有成员，也认为不合法
            if (name == "FatherCircle" && memberQuantityOfAreas[2] == 1)
            {
                TransferPrompt = findPrompt("2Full");
                return rejectVector;
            }

            //如果父亲圈没有成员，无法向爷爷圈添加成员
            if (name == "GrandpaCircle" && memberQuantityOfAreas[2] == 0)
            {
                TransferPrompt = findPrompt("No2to3");
                return rejectVector;
            }

            //若爷爷圈已经有成员，也认为不合法
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
        if(memberUpdating == -1) Debug.Log("[聚焦圈引导的成员更新]：" + name + "现在不再有成员");
        else Debug.Log("[聚焦圈引导的成员更新]：" + name + "执行更新，新成员编号为" + memberUpdating);
        //如果圈内已有成员，可以以旧换新
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
        //否则若需要加入成员就由对象池推出
        else
        {
            if (memberUpdating != -1)
            {
                sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject, memberUpdating);
            }
        }
    }
}
