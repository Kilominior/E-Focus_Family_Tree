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
    /// 传入成员，删除之，第二个参数默认为真，即删除成员的数据，若传入假，将仅删除物体不删除对应数据.
    /// </summary>
    public void PoolPush(GameObject theMember, bool isDelete = true)
    {
        theMember.SetActive(false);
        if (isDelete)
        {
            Debug.Log("<<对象池>>：正在删除的成员编号是：" + LastMember.GetComponent<MemberData>().memberData);
            int memberIndex = theMember.GetComponent<MemberData>().memberData;
            
            //对于来历不明的临时成员，仅删除其本身
            if(theMember.GetComponent<MemberMovement>().DepArea == null)
                TreeOfThisFile.SubTreeRemoveWithCheck(memberIndex);
            else
            {
                //对于来自不同区域的成员，调用不同的删除方法
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
                        //对于临时创建的成员（可能来自编辑区），仅删除其本身
                        TreeOfThisFile.SubTreeRemoveWithCheck(memberIndex);
                        break;
                }
            }
        }
        else Debug.Log("<<对象池>>：进行隐藏的成员编号是：" + theMember.GetComponent<MemberData>().memberData);

        //删除成员时删除其脚本和对应的信息
        Destroy(theMember.GetComponent<MemberMovement>());
        if (poolList.Count < PoolCapacity) poolList.Add(theMember);
        else Destroy(theMember);
    }

    /// <summary>
    /// 传入初始所在的功能区域，在该区域指定位置创建新成员,
    /// 若对象池中对象数量不足，就将创建新预制体.
    /// </summary>
    public void PoolPop(GameObject InitialArea, int memberIndex = -1)
    {
        //对于从编辑区新建的成员，指定其初始位置为特定位置
        initialPosition = InitialArea.GetComponent<AreaTransfer>().destAppoint(InitialArea, true);
        if (poolList.Count > 0)
        {
            Debug.Log("<<对象池>>：从对象池中取出成员...");
            LastMember = poolList[0];
            poolList.RemoveAt(0);
            //创建成员时重新挂载脚本，实现数值的初始化
            LastMember.AddComponent<MemberMovement>();
            LastMember.transform.position = initialPosition;
            //将成员拖动纽的大小设回默认，避免删除器删除留下的大小变化
            LastMember.transform.Find("DragButton").localScale = new(0.3f, 0.3f);
            LastMember.SetActive(true);
        }
        else
        {
            Debug.Log("<<对象池>>：新建成员加入对象池...");
            //从资源中实例化一个成员
            LastMember = Instantiate(Resources.Load<GameObject>("Member"), initialPosition, new Quaternion());

            //为成员的画布屏幕空间决定相机
            LastMember.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.GetComponent<Camera>();

        }
        //提前限制好成员的可移动性
        if (InitialArea.name == "FocusCircle") LastMember.GetComponent<MemberMovement>().isBound = true;
        else LastMember.GetComponent<MemberMovement>().isBound = false;

        //为成员绑定索引，若传入值未给出索引，则请求家族树创建新成员再执行绑定
        if (memberIndex != -1) LastMember.GetComponent<MemberData>().memberData = memberIndex;
        else LastMember.GetComponent<MemberData>().memberData = TreeOfThisFile.MemberCreate();

        //打印成员名字与配偶名字，根据性别给出成员贴图
        LastMember.GetComponent<MemberData>().fullnamePrintI();
        LastMember.GetComponent<MemberData>().fullnamePrintS();
        LastMember.GetComponent<MemberData>().sexPrint();

        //通知接收到新成员的区域，若成员为新建，则需绑定亲缘关系
        if (memberIndex != -1) InitialArea.GetComponent<AreaTransfer>().getMember(LastMember, false);
        else InitialArea.GetComponent<AreaTransfer>().getMember(LastMember);
        Debug.Log("<<对象池>>：当前得到的成员编号是：" + LastMember.GetComponent<MemberData>().memberData);
    }

    public void PoolClear()
    {
        poolList.Clear();
    }
}
