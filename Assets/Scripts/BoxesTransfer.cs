using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxesTransfer : AreaTransfer
{
    public int capcityOfBox; //本盒子的容量

    private int quantityOfBox; //本盒子内的成员数量

    public GameObject quantityDisplayer; //实时显示盒子内成员数量的UI，数量改变时会通知其做出修改

    private int presentPage; //当前盒子的页码，从0起计，可以在添加成员时改变，或由胶囊按钮翻页

    private int destPositionNo; //排列整齐的情况下新成员的目标地点编号，从0起计

    private List<GameObject> memberInBox; //每页所存的成员

    private int pageAccess = 0; //当前可访问的页码数，从0起计

    private int completionStart; //删除某成员时，其后的第一个成员位置，用于安排补位，默认不需要补位为-1

    private float completingSpeed = 0.5f;

    //以下为成员盒内的四个成员点位，分布在左上、右上、左下和右下方
    private Vector3[] destPositions = new Vector3[4];

    //以下为成员盒的四个翻页胶囊
    public Button page0;

    public Button page1;

    public Button page2;

    public Button page3;

    private void Awake()
    {
        //在加载成员盒时就确定其容量，兄弟成员盒为15，孩子成员盒为16，同时也初始化所有变量
        if (name == "BrotherBox") memberInBox = new List<GameObject>(15);
        else memberInBox = new List<GameObject>(16);
        quantityOfBox = 0;

        for(int i = 0; i <= 3; i++)
            destPositions[i] = transform.Find($"No{i}Position").position;

        presentPage = 0;
        completionStart = -1;
        pageAccess = 0;

        //为页码胶囊的点击绑定翻页方法
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

    //页码调整方法，传入目标页码即可做出改动。
    //该方法会将当前页面的成员暂时停用，实现隐藏，再把新页面的成员启用，实现打印
    public void pageChangeTo(int pageToGo)
    {
        //如果当前页码就是要求访问的页码，则不需要翻页
        if (presentPage == pageToGo)
        {
            Debug.Log("([页码管理])：" + name + "当前页码已经为" + pageToGo + "，未执行翻页");
            return;
        }
        //如果要求访问的页码超出可访问页码，则直接返回
        if (pageAccess < pageToGo)
        {
            Debug.Log("([页码管理])：" + name + "请求到达的页码" + pageToGo + "越界，未执行翻页");
            return;
        }
        for (int i = 0; i <= 3; i++)
        {
            if (memberInBox.Count > presentPage * 4 + i) LayerChange(memberInBox[presentPage * 4 + i], 6);
            if (memberInBox.Count > pageToGo * 4 + i) LayerChange(memberInBox[pageToGo * 4 + i], 3);
        }
        presentPage = pageToGo;
        Debug.Log("([页码管理])：" + name + "成功翻页至页码" + presentPage);
    }

    public override void getMember(GameObject memberIn, bool attaching = true)
    {
        memberInBox.Add(memberIn);
        if (name == "BrotherBox")
        {
            quantityOfBox = ++memberQuantityOfAreas[4];
            if (attaching)
            {
                Debug.Log("[亲缘关系添加]：成员编号" + memberIn.GetComponent<MemberData>().memberData +
                    "成为了聚焦成员" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "的兄弟");
                TreeOfThisFile.MemberJoinAsBrother(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[亲缘关系打印]：成员编号" + memberIn.GetComponent<MemberData>().memberData + "加入兄弟盒");
        }
        else {
            quantityOfBox = ++memberQuantityOfAreas[5];
            if (attaching)
            {
                Debug.Log("[亲缘关系添加]：成员编号" + memberIn.GetComponent<MemberData>().memberData +
                    "成为了聚焦成员" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "的孩子");
                TreeOfThisFile.MemberJoinAsChild(memberIn.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
            else Debug.Log("[亲缘关系打印]：成员编号" + memberIn.GetComponent<MemberData>().memberData + "加入孩子盒");
        }

        //只有在当前页面满的时候才会翻页
        if (destPositionNo == 0 && quantityOfBox != 1)
        {
            pageAccess++;
            pageChangeTo(presentPage + 1);
        }

        quantityDisplayer.GetComponent<BoxQuantitiesReport>().quantityAdd();
    }

    public override void releaseMember(GameObject memberOut, bool leaving = false)
    {
        //离开时与父亲断绝亲缘关系
        if (name == "BrotherBox")
        {
            quantityOfBox = --memberQuantityOfAreas[4];
            if (leaving)
            {
                Debug.Log("[亲缘关系断绝]：成员编号" + memberOut.GetComponent<MemberData>().memberData +
                         "不再是聚焦成员" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "的兄弟");
                TreeOfThisFile.MemberSeverRelationAsChild(memberOut.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().getData().father);
            }
        }
        else
        {
            quantityOfBox = --memberQuantityOfAreas[5];
            if (leaving)
            {
                Debug.Log("[亲缘关系断绝]：成员编号" + memberOut.GetComponent<MemberData>().memberData +
                         "不再是聚焦成员" + focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData + "的孩子");
                TreeOfThisFile.MemberSeverRelationAsChild(memberOut.GetComponent<MemberData>().memberData,
                    focusCircle.GetComponent<FocusCircle>().focusMember.GetComponent<MemberData>().memberData);
            }
        }


        //删除成员时由于离开的成员位置不定，需要安排该成员以后的成员即时补位，避免成员排列混乱
        //根据离开成员的位置，有三种可能的情况：
        //1.离开的成员在最后一位，对其他成员的位置和页面毫无影响，只需删除之
        //2.离开的成员在一个页面的四个成员之中，三个成员需要做出位置调整，
        //同时若下一个页面有成员，需要将其层级设为成员层再加入本页面，可能涉及到页面的删除
        //以此类推，下下个页面若还有成员，也需要加入下个页面，亦可能涉及页面的删除
        //3.离开的成员是某个页面的唯一一个成员，如果该页面之前有其他页面，需要往回翻页，同时删除本页面

        Debug.Log("([盒子成员补位])：" + name + "启动补位流程...");
        if (memberOut == memberInBox[memberInBox.Count - 1])
        {
            //处理情况1或3
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

    //成员补位方法，处理情况2
    private void memberCompletion(GameObject memberOut)
    {
        for (int i = 0; i < memberInBox.Count - 1; i++)
        {
            //先找到补位的出发位置,再开始逐个将成员往前移动
            if (memberInBox[i] == memberOut) completionStart = i;
            if (completionStart != -1)
            {
                //直接将每个成员复制到其前一个位置
                memberInBox[i] = memberInBox[i + 1];

                //一种情况是，该成员处于下一页，必然在隐藏层，需要视情况改变层级，并考虑页面的删除
                if (i % 4 == 3)
                {
                    if (memberInBox[i].layer == 6) LayerChange(memberInBox[i + 1], 3);
                    StartCoroutine(memberAdsorb(memberInBox[i + 1], destPositions[i % 4]));
                    if (i + 2 == memberInBox.Count) pageAccess--;
                }
                //否则，该成员就在当前正在处理的页面（不一定在活动状态），只需要移动到恰当位置
                else
                {
                    StartCoroutine(memberAdsorb(memberInBox[i + 1], destPositions[i % 4]));
                }
            }
        }

        //补位完成后，将补位出发点设回-1标记无需再补位，并在最后将剩余的成员清除
        completionStart = -1;
        memberInBox.RemoveAt(memberInBox.Count - 1);
    }

    //在执行补位时帮助完成成员移动的方法，传入欲移动的成员和目的地即可
    private IEnumerator memberAdsorb(GameObject memberToMove, Vector3 destPosition)
    {
        Debug.Log("([盒子成员补位])：成员编号" + name + "吸附到目的地编号" + destPosition);
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
            //如果是从本区域内出发，则无条件返回出发点
            if (memberChecking.GetComponent<MemberMovement>().DepArea == gameObject)
            {
                return memberChecking.GetComponent<MemberMovement>().PointOfDeparture;
            }

            //如果出发点不是编辑区，那么认为移动不合法
            if (memberChecking.GetComponent<MemberMovement>().DepArea.name != "EditorArea")
            {
                TransferPrompt = findPrompt("illegal");
                return rejectVector;
            }

            //只有当父亲圈中存在成员时，主成员才能有兄弟
            if (name == "BrotherBox" && memberQuantityOfAreas[2] == 0)
            {
                TransferPrompt = findPrompt("No2to4");
                return rejectVector;
            }

            //若盒子已满，则无法移入新成员
            if (quantityOfBox == capcityOfBox)
            {
                if (name == "BrotherBox") TransferPrompt = findPrompt("4Full");
                else TransferPrompt = findPrompt("5Full");
                return rejectVector;
            }
            if (name == "BrotherBox" && memberQuantityOfAreas[2] != 1) return rejectVector;
        }

        //在成员排布整齐（中间没有空位）的情况下，可以将页面翻至最后的可用页面、直接折算出目标点位并返回
        pageChangeTo(pageAccess);
        destPositionNo = quantityOfBox % 4;
        return destPositions[destPositionNo];
    }

    public void updateMember(List<int> memberUpdating)
    {
        if (memberUpdating.Count == 0) Debug.Log("[聚焦圈引导的成员更新]：" + name + "更新后将为空");
        else Debug.Log("[聚焦圈引导的成员更新]：" + name + "执行更新，新成员共有" + memberUpdating.Count + "个...");
        
        
        /*int oldMemberCount = memberInBox.Count;
        for (int i = 0; i < memberUpdating.Count; i++)
        {
            //对于新加入的成员中超出原有成员数的部分，需要新建成员并令成员盒知悉
            if (i >= oldMemberCount)
            {
                sceneAgent.GetComponent<MemberPool>().PoolPop(gameObject, memberUpdating[i]);
            }
            //否则只需要修改原有成员对应的数据
            else
            {
                memberInBox[i].GetComponent<MemberData>().updateData(memberUpdating[i]);
            }
        }
        oldMemberCount = memberInBox.Count;
        //对于新成员少于原有成员数的部分，则需要将原成员压入对象池，并令盒子释放原有的成员
        for (int i = memberUpdating.Count; i < oldMemberCount; i++)
        {
            sceneAgent.GetComponent<MemberPool>().PoolPush(memberInBox[memberUpdating.Count], false);
            releaseMember(memberInBox[memberUpdating.Count]);
        }*/


        //直接将原有的成员全部清除，再行加入新成员
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
