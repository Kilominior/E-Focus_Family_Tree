using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FocusCircle : AreaTransfer
{
    public GameObject focusMember;

    //以下为更新聚焦成员时用以通知其他功能区域做出成员更新的目标区域
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

        //通知信息编辑器现在编辑的是聚焦成员的信息
        canvas.GetComponent<EditingBehaviors>().EditorChange(false);

        //通知其他功能区域做出成员更新
        memberIndexUpdating = focusMember.GetComponent<MemberData>().memberData;
        Debug.Log("[[聚焦圈]]：为成员编号" + memberIndexUpdating + "启动成员更新，开始通知各功能区域...");
        memberDataUpdating = TreeOfThisFile.MembersOfThisTree[memberIndexUpdating];
        fatherCircle.GetComponent<ElderTransfer>().updateMember(memberDataUpdating.father);
        grandpaCircle.GetComponent<ElderTransfer>().updateMember(TreeOfThisFile.GetGrandpa(memberIndexUpdating));
        brotherBox.GetComponent<BoxesTransfer>().updateMember(TreeOfThisFile.GetBrothers(memberIndexUpdating));
        childrenBox.GetComponent<BoxesTransfer>().updateMember(TreeOfThisFile.GetChildren(memberIndexUpdating));

        //通过修改布尔变量限制圈中成员的移动
        focusMember.GetComponent<MemberMovement>().isBound = true;
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        //重新允许成员被移动
        memberOut.GetComponent<MemberMovement>().isBound = false;
        //由于成员被替换，调用对象池的push方法将原有的成员物体删除，但不删除对应信息
        sceneAgent.GetComponent<MemberPool>().PoolPush(memberOut, false);
    }

    //系统显示的现有全部成员的清理方法，也即清屏，在返回主页面或加载新文件前被调用
    public void clearMembers()
    {
        Debug.Log("[[聚焦圈]]：开始执行清屏操作");
        //创建一个新成员取代原有的聚焦成员，由于该成员没有任何亲属，所有区域都将更新为空
        sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject);
        //主动删除新创建的成员，使聚焦圈也为空
        sceneAgent.GetComponent<MemberPool>().PoolPush(focusMember);
        sceneAgent.GetComponent<MemberPool>().PoolClear();
    }

    public override Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false)
    {
        if (!isLoaded)
        {
            //若出发点是在编辑区，认为移动不合法
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name == "EditorArea")
            {
                TransferPrompt = findPrompt("1To0");
                return rejectVector;
            }

            //若编辑区有成员存在，认为移动不合法
            if (memberQuantityOfAreas[1] == 1)
            {
                TransferPrompt = findPrompt("1FullTo0");
                return rejectVector;
            }
        }
        return transform.position;
    }
}





