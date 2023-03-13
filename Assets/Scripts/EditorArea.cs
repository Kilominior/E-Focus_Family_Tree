using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EditorArea : AreaTransfer
{
    public GameObject memberEditing;

    public Vector3 EditorPosition = new(0, -3.5f); //成员在成员编辑器中的实际位置

    //新成员的创建方法，被新建按钮订阅
    //如果编辑区里没有成员，那么点击按钮可以创建一个新成员
    public void CreateMember()
    {
        if (memberQuantityOfAreas[1] == 0) sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject);
    }

    public override void getMember(GameObject memberIn, bool attaching = true)
    {
        memberQuantityOfAreas[1] = 1;
        memberEditing = memberIn;
        //获得成员时通知画布，令编辑区中的成员成为输入框默认文字
        canvas.GetComponent<EditingBehaviors>().EditorChange(true);
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        memberQuantityOfAreas[1] = 0;
        //成员释出时通知画布，令聚焦圈中的成员成为输入框默认文字
        canvas.GetComponent<EditingBehaviors>().EditorChange(false);
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //如果是从本区域内出发，则无条件返回出发点
            if (memberChecking.GetComponent<MemberMovement>().DepArea == gameObject)
                return memberChecking.GetComponent<MemberMovement>().PointOfDeparture;

            //若编辑区已经有成员存在，则无法再加入成员
            if (memberQuantityOfAreas[1] == 1)
            {
                TransferPrompt = findPrompt("1Full");
                return rejectVector;
            }

            //父亲圈和爷爷圈的成员不能进入编辑区，因为他们可以在聚焦圈被编辑
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name == "FatherCircle" ||
                memberChecking.GetComponent<MemberMovement>().DepArea.name == "GrandpaCircle")
            {
                TransferPrompt = findPrompt("illegal");
                return rejectVector;
            }

            //兄弟圈和孩子圈里的成员进入编辑区的条件，是自己没有孩子存在
            if (memberChecking.GetComponent<MemberData>().getData().children.Count != 0)
            {
                TransferPrompt = findPrompt("haveSonTo1");
                return rejectVector;
            }
        }

        return EditorPosition;
    }
}