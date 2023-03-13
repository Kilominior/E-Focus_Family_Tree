using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// 该场景的基类，存有允许全局访问的信息.
/// </summary>
public class BaseClasses : MonoBehaviour
{
    public static TreeData DataOfThisTree;

    public static FamilyTree TreeOfThisFile;

    public static GameObject sceneAgent;

    public static GameObject canvas;

    public static GameObject focusCircle;

    public static GameObject editorArea;

    public static GameObject prompt;

    /// <summary>
    /// 存有从json文件中读取的信息.
    /// </summary>
    public static JsonData jsonList;

    /// <summary>
    /// 拒绝成员时弹出的提示信息，由做出拒绝的功能区域从Prompts.json中选取，并将由UI画布输出.
    /// </summary>
    public static string TransferPrompt = string.Empty;

    public static string DeletePrompt = string.Empty;

    /// <summary>
    /// 传入提示的触发条件，传回需要打印出的提示.
    /// </summary>
    public static string findPrompt(string cause)
    {
        return jsonList.PromptsList.Find(c => c.promptCause == cause).promptItself;
    }

    /// <summary>
    /// 传入物体和目标层，将物体所在层改为目标层,
    /// 若物体有孩子存在，同时也更改其孩子的层级.
    /// </summary>
    public static void LayerChange(GameObject targetObject, int layerTo)
    {
        targetObject.layer = layerTo;
        if (targetObject.transform.childCount != 0)
        {
            for (int i = 0; i < targetObject.transform.childCount; i++)
            {
                targetObject.transform.GetChild(i).gameObject.layer = layerTo;
            }
        }
    }

    /// <summary>
    /// 保存和读取文件的文件名校验方法，若合法才返回真
    /// </summary>
    public static bool textCheck(string textToCheck)
    {
        bool opResult = Regex.IsMatch(textToCheck, @"(?!((^(con)$)|^(con)\\..*|(^(prn)$)|^(prn)\\..*|(^(aux)$)|^(aux)\\..*|(^(nul)$)|^(nul)\\..*|(^(com)[1-9]$)|^(com)[1-9]\\..*|(^(lpt)[1-9]$)|^(lpt)[1-9]\\..*)|^\\s+|.*\\s$)(^[^\\\\\\/\\:\\<\\>\\*\\?\\\\\\""\\\\|]{1,255}$)");
        return opResult;
    }
}

/// <summary>
/// 管理json文件存取的类，json文件包含有：提示.
/// </summary>
public class JsonData
{
    public List<Prompts> PromptsList;
}

/// <summary>
/// 可序列化的提示类，用于读取存有提示的json文件.
/// </summary>
[Serializable]
public class Prompts
{
    public string promptCause;
    public string promptItself;
}

/// <summary>
/// 管理各个功能区域的成员转移，
/// 除删除器外，所有功能区的成员管理类都继承自该类，
/// 并将实现其中的方法.
/// </summary>
public abstract class AreaTransfer : BaseClasses
{
    /// <summary>
    /// 获取被加入的成员，attaching默认为true，代表成员需要在该方法中与聚焦成员绑定亲缘关系.
    /// </summary>
    public abstract void getMember(GameObject memberIn, bool attaching = true);

    /// <summary>
    /// 释出被移走的成员，leaving默认为false，代表成员不需要与聚焦成员断绝亲缘关系.
    /// </summary>
    public abstract void releaseMember(GameObject memberOut, bool leaving = false);

    /// <summary>
    /// 为成员指定目的地，或拒绝成员的进入，第二个参数若传入值为真，则不进行判断直接允许加入.
    /// </summary>
    public abstract Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false);

    /// <summary>
    /// 若区域拒绝成员，则返回该向量.
    /// </summary>
    public readonly Vector3 rejectVector = new Vector3(0, 0, 3.14f);

    /// <summary>
    /// 各个区域内的成员数量，索引从0开始分别对应：
    /// 0聚焦圈，1编辑器，2父亲圈，3爷爷圈，4兄弟圈，5孩子圈.
    /// </summary>
    public static int[] memberQuantityOfAreas = new int[6];
}

/// <summary>
/// 家族成员类，存有家族成员的全部信息细节，可以通过index访问到具体成员的信息.
/// </summary>
public class FamilyMember
{
    public int index { get; set; } //成员的编号，与其在线性表中的位置相同，若为-1则代表未被分配编号

    // 以下为成员本人的基本信息
    public string name { get; set; } //成员的名字

    public string surname { get; set; } //成员的姓氏

    public bool sex { get; set; } //主成员性别，false女，true男，配偶性别由成员性别推算出

    public int birthYear { get; set; } //成员出生年份

    //以下为成员配偶spouse（若有）的基本信息
    public string spouseName { get; set; } //配偶的名字

    public string spouseSurname { get; set; } //配偶的姓氏

    public int spouseBirthYear { get; set; } //配偶出生年份

    //以下为成员的涉及的亲子关系
    public int father { get; set; } //成员的父亲，若为空，则为-1

    public List<int> children { get; set; } //成员的孩子，存储为数组

    public FamilyMember(string _surname)
    {
        //初始化方法将创建一个除姓氏外全默认的标准成员
        index = -1;
        name = string.Empty;
        surname = _surname;
        birthYear = -1;
        sex = true;
        spouseName = string.Empty;
        spouseSurname = string.Empty;
        spouseBirthYear = -1;
        father = -1;
        children = new List<int>(16);
    }

    public FamilyMember(string _surname, int _index)
    {
        //若初始化的同时也传入index，则直接设置好index
        index = _index;
        name = string.Empty;
        surname = _surname;
        birthYear = -1;
        sex = true;
        spouseName = string.Empty;
        spouseSurname = string.Empty;
        spouseBirthYear = -1;
        father = -1;
        children = new List<int>(16);
    }

    ~FamilyMember()
    {
        children.Clear();
    }
}

/// <summary>
/// 家族树类，存有家族树整体的信息，可以用祖先的编号直接获取祖先成员.
/// </summary>
public class FamilyTree
{
    public string surname { get; set; } //家族的姓氏

    public int genDepth { get; set; } //树的高度

    public int chiefIndex { get; set; } //家族的族长编号，为-1时代表没有族长存在

    public int memberCount { get; set; } //家族的成员总数

    public List<FamilyMember> MembersOfThisTree; //树上的全部家族成员，由线性表存储

    //由于删除的发生而不再被使用的索引，新建成员将优先使用其中弹出的索引位置
    public Queue<int> redundantIndexes { get; set; }


    public FamilyTree()
    {
        //初始化将创建一个只有1名成员的家族树，默认族长为唯一的成员，并初始化成员列表和冗余索引队列
        surname = "族姓";
        genDepth = 1;
        chiefIndex = 0;
        memberCount = 1;
        MembersOfThisTree = new List<FamilyMember>(256);
        MembersOfThisTree.Add(new FamilyMember(surname, 0));
        redundantIndexes = new();
    }

    public FamilyTree(string _surname)
    {
        //若初始化的同时也传入姓氏，则直接将家族的姓氏设定好
        surname = _surname;
        genDepth = 1;
        chiefIndex = 0;
        memberCount = 1;
        MembersOfThisTree = new List<FamilyMember>(256);
        MembersOfThisTree.Add(new FamilyMember(surname, 0));
        redundantIndexes = new();
    }

    ~FamilyTree()
    {
        MembersOfThisTree.Clear();
        redundantIndexes.Clear();
    }

    //为树中选定成员获取兄弟的编号表，若没有则返回空表，在创建返回所用临时表时会避开成员自己
    public List<int> GetBrothers(int indexFrom)
    {
        if (MembersOfThisTree[indexFrom].father == -1) return new List<int>(0);
        List<int> tmpList = new(MembersOfThisTree[MembersOfThisTree[indexFrom].father].children.Count);
        foreach (var c in MembersOfThisTree[MembersOfThisTree[indexFrom].father].children)
            if (c != indexFrom) tmpList.Add(c);
        return tmpList;
    }

    //为树中选定成员获取孩子的编号表，若没有则返回空表
    public List<int> GetChildren(int indexFrom)
    {
        if (MembersOfThisTree[indexFrom].children.Count == 0) return new List<int>(0);
        List<int> tmpList = new(MembersOfThisTree[indexFrom].children.Count);
        foreach (var c in MembersOfThisTree[indexFrom].children)
            tmpList.Add(c);
        return tmpList;
    }

    //为树中选定成员获取爷爷的编号，若没有则返回-1
    public int GetGrandpa(int indexFrom)
    {
        if (MembersOfThisTree[indexFrom].father == -1)
            return -1;
        return MembersOfThisTree[MembersOfThisTree[indexFrom].father].father;
    }

    //为树中选定成员获取家族的祖宗的编号，如果自己就是，则返回自己
    public int GetRootMember(int indexFrom)
    {
        int indexNow = indexFrom;
        while(MembersOfThisTree[indexNow].father != -1)
        {
            indexNow = MembersOfThisTree[indexNow].father;
        }
        return indexNow;
    }

    //查询选定成员是否有兄弟存在
    public bool HaveBrother(int indexAsking)
    {
        if (MembersOfThisTree[indexAsking].father == -1) return false;
        return MembersOfThisTree[MembersOfThisTree[indexAsking].father].children.Count != 0;
    }

    /// <summary>
    /// 新成员创建并加入线性表的方法，该方法会返回新成员的索引。
    /// 会优先从冗余索引队列中获取索引，若未取得，则将成员加入线性表末尾.
    /// </summary>
    public int MemberCreate()
    {
        FamilyMember newMember;
        //新成员的姓氏默认使用聚焦圈内成员的姓氏，尽可能减少麻烦
        string sname = BaseClasses.focusCircle.GetComponent<FocusCircle>().focusMember.
            GetComponent<MemberData>().getData().surname;
        //增加成员总数计数器
        memberCount++;
        if (redundantIndexes.Count == 0)
        {
            newMember = new FamilyMember(sname, MembersOfThisTree.Count);
            MembersOfThisTree.Add(newMember);
            Debug.Log("->成员表管理<-：新成员以编号" + MembersOfThisTree.Count + "创建于列表末尾");
        }
        else
        {
            int indexTo = redundantIndexes.Dequeue();
            newMember = new FamilyMember(sname, indexTo);
            MembersOfThisTree[indexTo] = newMember;
            Debug.Log("->成员表管理<-：新成员以编号" + indexTo + "继承了冗余空间队列中的索引");
        }
        return newMember.index;
    }

    //以下方法为树中的选定成员添加亲属，将其索引绑到恰当的位置

    public void MemberJoinAsFather(int memberComing, int focusMember)
    {
        Debug.Log(">成员关系绑定<：" + "父亲：" + memberComing + "；孩子：" + focusMember);
        //聚焦成员将绑定父亲
        MembersOfThisTree[focusMember].father = memberComing;

        //父亲将得到成员作为孩子
        MembersOfThisTree[memberComing].children.Add(focusMember);
    }

    public void MemberJoinAsGrandpa(int memberComing, int focusMember)
    {
        Debug.Log(">成员关系绑定<：" + "父亲：" + memberComing + "；孩子：" + MembersOfThisTree[focusMember].father);
        //成员的父亲将绑定爷爷作为父亲
        MembersOfThisTree[MembersOfThisTree[focusMember].father].father = memberComing;

        //爷爷将得到成员的父亲作为孩子
        MembersOfThisTree[memberComing].children.Add(MembersOfThisTree[focusMember].father);
    }

    public void MemberJoinAsChild(int memberComing, int focusMember)
    {
        Debug.Log(">成员关系绑定<：" + "父亲：" + focusMember + "；孩子：" + memberComing);
        //孩子将绑定聚焦成员作为父亲
        MembersOfThisTree[memberComing].father = focusMember;

        //聚焦成员将增加孩子
        MembersOfThisTree[focusMember].children.Add(memberComing);
    }

    public void MemberJoinAsBrother(int memberComing, int focusMember)
    {
        Debug.Log(">成员关系绑定<：" + "父亲：" + MembersOfThisTree[focusMember].father + "；孩子：" + memberComing);
        //兄弟将绑定聚焦成员的父亲
        MembersOfThisTree[memberComing].father = MembersOfThisTree[focusMember].father;

        //聚焦成员的父亲将增加孩子
        MembersOfThisTree[MembersOfThisTree[focusMember].father].children.Add(memberComing);
    }

    //以下方法负责成员与其父亲的关系移除，从成员自身以及对应亲属处移除成员与之的联系
    public void MemberSeverRelationAsChild(int childMemberToGo, int fatherMember)
    {
        //检测成员是否有父子亲缘关系，若否，直接退出
        if (MembersOfThisTree[childMemberToGo].father == -1) return;
        if (MembersOfThisTree[childMemberToGo].father != fatherMember) return;

        Debug.Log(">-成员关系解除-<：原父亲：" + MembersOfThisTree[childMemberToGo].father + "；原孩子：" + childMemberToGo);
        //成员解除其父亲的索引
        MembersOfThisTree[childMemberToGo].father = -1;

        //父亲删去该孩子的索引
        MembersOfThisTree[fatherMember].children.Remove(childMemberToGo);
    }

    //成员的删除方法，确定了删除目标后调用该方法，后续可以完善优化避免空间浪费
    private void MemberDel(int IndexToDel)
    {
        //删除成员的亲缘关系，防止误用
        MembersOfThisTree[IndexToDel].father = -1;
        MembersOfThisTree[IndexToDel].children.Clear();

        //将成员的索引加入冗余空间队列
        redundantIndexes.Enqueue(IndexToDel);
        Debug.Log(">->成员删除<-<：成员编号" + IndexToDel + "被删除，当前冗余空间队列中共有" + redundantIndexes.Count + "个索引");

        memberCount--;
    }

    //以下方法为树中选定成员删除亲属，并且做出连带删除，每次删除都会减少成员总数计数器

    public void MemberRemoveFatherWithAncestor(int IndexOfChild)
    {
        SubTreeRemoveWithCheck(GetRootMember(IndexOfChild), IndexOfChild);
    }

    /// <summary>
    /// 该方法从给定的根节点开始，从上往下逐个删除成员，不删除被阻止节点的支系.
    /// </summary>
    /// <param name="IndexOfDel">删除开始的根节点.</param>
    /// <param name="IndexAvoid">不允许删除的树系根节点，若没有可不传值.</param>
    public void SubTreeRemoveWithCheck(int IndexOfRoot, int IndexAvoid = -1)
    {
        if (IndexAvoid != -1) Debug.Log(">->成员递归删除<-<：根成员编号" + IndexOfRoot + "，避开成员编号" + IndexAvoid);
        else Debug.Log(">->成员递归删除<-<：根成员编号" + IndexOfRoot + "，不避开成员");
        //首先检查给定的根节点是否有父亲，若有，删除其在父亲处的索引
        MemberSeverRelationAsChild(IndexOfRoot, MembersOfThisTree[IndexOfRoot].father);

        //检查阻止的节点是否有父亲，若有，删除其在父亲处的索引，从而使其不可能被找到
        if(IndexAvoid != -1)
            MemberSeverRelationAsChild(IndexAvoid, MembersOfThisTree[IndexAvoid].father);

        //接着进入递归，开始逐个向下删除
        MemberRemoveWithCheck(IndexOfRoot);
    }

    //成员的递归删除方法，可以逐个向下删除成员
    private void MemberRemoveWithCheck(int IndexOfNow)
    {
        if(MembersOfThisTree[IndexOfNow].children.Count != 0)
        {
            foreach (var child in MembersOfThisTree[IndexOfNow].children)
            {
                //只要孩子的索引不为阻止的索引，都向下递归删除
                MemberRemoveWithCheck(child);
            }
        }
        //以后序遍历方式对对成员自己进行删除
        MemberDel(IndexOfNow);
    }
}

/// <summary>
/// 树数据类，负责与文件交互以存取数据，提供裸数据形式.
/// </summary>
[System.Serializable]
public class TreeData
{
    //树的信息
    public string surname;

    public int genDepth;

    public int chiefIndex;

    public int memberCount;

    public Queue<int> redundantIndexes;

    //成员的信息
    public List<int> index;

    public List<string> name;

    public List<string> memberSurname;

    public List<bool> sex;

    public List<int> birthYear;

    public List<string> spouseName;

    public List<string> spouseSurname;

    public List<int> spouseBirthYear;

    public List<int> father;

    public List<List<int>> children;
}
