using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// �ó����Ļ��࣬��������ȫ�ַ��ʵ���Ϣ.
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
    /// ���д�json�ļ��ж�ȡ����Ϣ.
    /// </summary>
    public static JsonData jsonList;

    /// <summary>
    /// �ܾ���Աʱ��������ʾ��Ϣ���������ܾ��Ĺ��������Prompts.json��ѡȡ��������UI�������.
    /// </summary>
    public static string TransferPrompt = string.Empty;

    public static string DeletePrompt = string.Empty;

    /// <summary>
    /// ������ʾ�Ĵ���������������Ҫ��ӡ������ʾ.
    /// </summary>
    public static string findPrompt(string cause)
    {
        return jsonList.PromptsList.Find(c => c.promptCause == cause).promptItself;
    }

    /// <summary>
    /// ���������Ŀ��㣬���������ڲ��ΪĿ���,
    /// �������к��Ӵ��ڣ�ͬʱҲ�����亢�ӵĲ㼶.
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
    /// ����Ͷ�ȡ�ļ����ļ���У�鷽�������Ϸ��ŷ�����
    /// </summary>
    public static bool textCheck(string textToCheck)
    {
        bool opResult = Regex.IsMatch(textToCheck, @"(?!((^(con)$)|^(con)\\..*|(^(prn)$)|^(prn)\\..*|(^(aux)$)|^(aux)\\..*|(^(nul)$)|^(nul)\\..*|(^(com)[1-9]$)|^(com)[1-9]\\..*|(^(lpt)[1-9]$)|^(lpt)[1-9]\\..*)|^\\s+|.*\\s$)(^[^\\\\\\/\\:\\<\\>\\*\\?\\\\\\""\\\\|]{1,255}$)");
        return opResult;
    }
}

/// <summary>
/// ����json�ļ���ȡ���࣬json�ļ������У���ʾ.
/// </summary>
public class JsonData
{
    public List<Prompts> PromptsList;
}

/// <summary>
/// �����л�����ʾ�࣬���ڶ�ȡ������ʾ��json�ļ�.
/// </summary>
[Serializable]
public class Prompts
{
    public string promptCause;
    public string promptItself;
}

/// <summary>
/// ���������������ĳ�Աת�ƣ�
/// ��ɾ�����⣬���й������ĳ�Ա�����඼�̳��Ը��࣬
/// ����ʵ�����еķ���.
/// </summary>
public abstract class AreaTransfer : BaseClasses
{
    /// <summary>
    /// ��ȡ������ĳ�Ա��attachingĬ��Ϊtrue�������Ա��Ҫ�ڸ÷�������۽���Ա����Ե��ϵ.
    /// </summary>
    public abstract void getMember(GameObject memberIn, bool attaching = true);

    /// <summary>
    /// �ͳ������ߵĳ�Ա��leavingĬ��Ϊfalse�������Ա����Ҫ��۽���Ա�Ͼ���Ե��ϵ.
    /// </summary>
    public abstract void releaseMember(GameObject memberOut, bool leaving = false);

    /// <summary>
    /// Ϊ��Աָ��Ŀ�ĵأ���ܾ���Ա�Ľ��룬�ڶ�������������ֵΪ�棬�򲻽����ж�ֱ���������.
    /// </summary>
    public abstract Vector3 destAppoint(GameObject memberChecking, bool isLoaded = false);

    /// <summary>
    /// ������ܾ���Ա���򷵻ظ�����.
    /// </summary>
    public readonly Vector3 rejectVector = new Vector3(0, 0, 3.14f);

    /// <summary>
    /// ���������ڵĳ�Ա������������0��ʼ�ֱ��Ӧ��
    /// 0�۽�Ȧ��1�༭����2����Ȧ��3үүȦ��4�ֵ�Ȧ��5����Ȧ.
    /// </summary>
    public static int[] memberQuantityOfAreas = new int[6];
}

/// <summary>
/// �����Ա�࣬���м����Ա��ȫ����Ϣϸ�ڣ�����ͨ��index���ʵ������Ա����Ϣ.
/// </summary>
public class FamilyMember
{
    public int index { get; set; } //��Ա�ı�ţ����������Ա��е�λ����ͬ����Ϊ-1�����δ��������

    // ����Ϊ��Ա���˵Ļ�����Ϣ
    public string name { get; set; } //��Ա������

    public string surname { get; set; } //��Ա������

    public bool sex { get; set; } //����Ա�Ա�falseŮ��true�У���ż�Ա��ɳ�Ա�Ա������

    public int birthYear { get; set; } //��Ա�������

    //����Ϊ��Ա��żspouse�����У��Ļ�����Ϣ
    public string spouseName { get; set; } //��ż������

    public string spouseSurname { get; set; } //��ż������

    public int spouseBirthYear { get; set; } //��ż�������

    //����Ϊ��Ա���漰�����ӹ�ϵ
    public int father { get; set; } //��Ա�ĸ��ף���Ϊ�գ���Ϊ-1

    public List<int> children { get; set; } //��Ա�ĺ��ӣ��洢Ϊ����

    public FamilyMember(string _surname)
    {
        //��ʼ������������һ����������ȫĬ�ϵı�׼��Ա
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
        //����ʼ����ͬʱҲ����index����ֱ�����ú�index
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
/// �������࣬���м������������Ϣ�����������ȵı��ֱ�ӻ�ȡ���ȳ�Ա.
/// </summary>
public class FamilyTree
{
    public string surname { get; set; } //���������

    public int genDepth { get; set; } //���ĸ߶�

    public int chiefIndex { get; set; } //������峤��ţ�Ϊ-1ʱ����û���峤����

    public int memberCount { get; set; } //����ĳ�Ա����

    public List<FamilyMember> MembersOfThisTree; //���ϵ�ȫ�������Ա�������Ա�洢

    //����ɾ���ķ��������ٱ�ʹ�õ��������½���Ա������ʹ�����е���������λ��
    public Queue<int> redundantIndexes { get; set; }


    public FamilyTree()
    {
        //��ʼ��������һ��ֻ��1����Ա�ļ�������Ĭ���峤ΪΨһ�ĳ�Ա������ʼ����Ա�б��������������
        surname = "����";
        genDepth = 1;
        chiefIndex = 0;
        memberCount = 1;
        MembersOfThisTree = new List<FamilyMember>(256);
        MembersOfThisTree.Add(new FamilyMember(surname, 0));
        redundantIndexes = new();
    }

    public FamilyTree(string _surname)
    {
        //����ʼ����ͬʱҲ�������ϣ���ֱ�ӽ�����������趨��
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

    //Ϊ����ѡ����Ա��ȡ�ֵܵı�ű���û���򷵻ؿձ��ڴ�������������ʱ��ʱ��ܿ���Ա�Լ�
    public List<int> GetBrothers(int indexFrom)
    {
        if (MembersOfThisTree[indexFrom].father == -1) return new List<int>(0);
        List<int> tmpList = new(MembersOfThisTree[MembersOfThisTree[indexFrom].father].children.Count);
        foreach (var c in MembersOfThisTree[MembersOfThisTree[indexFrom].father].children)
            if (c != indexFrom) tmpList.Add(c);
        return tmpList;
    }

    //Ϊ����ѡ����Ա��ȡ���ӵı�ű���û���򷵻ؿձ�
    public List<int> GetChildren(int indexFrom)
    {
        if (MembersOfThisTree[indexFrom].children.Count == 0) return new List<int>(0);
        List<int> tmpList = new(MembersOfThisTree[indexFrom].children.Count);
        foreach (var c in MembersOfThisTree[indexFrom].children)
            tmpList.Add(c);
        return tmpList;
    }

    //Ϊ����ѡ����Ա��ȡүү�ı�ţ���û���򷵻�-1
    public int GetGrandpa(int indexFrom)
    {
        if (MembersOfThisTree[indexFrom].father == -1)
            return -1;
        return MembersOfThisTree[MembersOfThisTree[indexFrom].father].father;
    }

    //Ϊ����ѡ����Ա��ȡ��������ڵı�ţ�����Լ����ǣ��򷵻��Լ�
    public int GetRootMember(int indexFrom)
    {
        int indexNow = indexFrom;
        while(MembersOfThisTree[indexNow].father != -1)
        {
            indexNow = MembersOfThisTree[indexNow].father;
        }
        return indexNow;
    }

    //��ѯѡ����Ա�Ƿ����ֵܴ���
    public bool HaveBrother(int indexAsking)
    {
        if (MembersOfThisTree[indexAsking].father == -1) return false;
        return MembersOfThisTree[MembersOfThisTree[indexAsking].father].children.Count != 0;
    }

    /// <summary>
    /// �³�Ա�������������Ա�ķ������÷����᷵���³�Ա��������
    /// �����ȴ��������������л�ȡ��������δȡ�ã��򽫳�Ա�������Ա�ĩβ.
    /// </summary>
    public int MemberCreate()
    {
        FamilyMember newMember;
        //�³�Ա������Ĭ��ʹ�þ۽�Ȧ�ڳ�Ա�����ϣ������ܼ����鷳
        string sname = BaseClasses.focusCircle.GetComponent<FocusCircle>().focusMember.
            GetComponent<MemberData>().getData().surname;
        //���ӳ�Ա����������
        memberCount++;
        if (redundantIndexes.Count == 0)
        {
            newMember = new FamilyMember(sname, MembersOfThisTree.Count);
            MembersOfThisTree.Add(newMember);
            Debug.Log("->��Ա�����<-���³�Ա�Ա��" + MembersOfThisTree.Count + "�������б�ĩβ");
        }
        else
        {
            int indexTo = redundantIndexes.Dequeue();
            newMember = new FamilyMember(sname, indexTo);
            MembersOfThisTree[indexTo] = newMember;
            Debug.Log("->��Ա�����<-���³�Ա�Ա��" + indexTo + "�̳�������ռ�����е�����");
        }
        return newMember.index;
    }

    //���·���Ϊ���е�ѡ����Ա�������������������ǡ����λ��

    public void MemberJoinAsFather(int memberComing, int focusMember)
    {
        Debug.Log(">��Ա��ϵ��<��" + "���ף�" + memberComing + "�����ӣ�" + focusMember);
        //�۽���Ա���󶨸���
        MembersOfThisTree[focusMember].father = memberComing;

        //���׽��õ���Ա��Ϊ����
        MembersOfThisTree[memberComing].children.Add(focusMember);
    }

    public void MemberJoinAsGrandpa(int memberComing, int focusMember)
    {
        Debug.Log(">��Ա��ϵ��<��" + "���ף�" + memberComing + "�����ӣ�" + MembersOfThisTree[focusMember].father);
        //��Ա�ĸ��׽���үү��Ϊ����
        MembersOfThisTree[MembersOfThisTree[focusMember].father].father = memberComing;

        //үү���õ���Ա�ĸ�����Ϊ����
        MembersOfThisTree[memberComing].children.Add(MembersOfThisTree[focusMember].father);
    }

    public void MemberJoinAsChild(int memberComing, int focusMember)
    {
        Debug.Log(">��Ա��ϵ��<��" + "���ף�" + focusMember + "�����ӣ�" + memberComing);
        //���ӽ��󶨾۽���Ա��Ϊ����
        MembersOfThisTree[memberComing].father = focusMember;

        //�۽���Ա�����Ӻ���
        MembersOfThisTree[focusMember].children.Add(memberComing);
    }

    public void MemberJoinAsBrother(int memberComing, int focusMember)
    {
        Debug.Log(">��Ա��ϵ��<��" + "���ף�" + MembersOfThisTree[focusMember].father + "�����ӣ�" + memberComing);
        //�ֵܽ��󶨾۽���Ա�ĸ���
        MembersOfThisTree[memberComing].father = MembersOfThisTree[focusMember].father;

        //�۽���Ա�ĸ��׽����Ӻ���
        MembersOfThisTree[MembersOfThisTree[focusMember].father].children.Add(memberComing);
    }

    //���·��������Ա���丸�׵Ĺ�ϵ�Ƴ����ӳ�Ա�����Լ���Ӧ�������Ƴ���Ա��֮����ϵ
    public void MemberSeverRelationAsChild(int childMemberToGo, int fatherMember)
    {
        //����Ա�Ƿ��и�����Ե��ϵ������ֱ���˳�
        if (MembersOfThisTree[childMemberToGo].father == -1) return;
        if (MembersOfThisTree[childMemberToGo].father != fatherMember) return;

        Debug.Log(">-��Ա��ϵ���-<��ԭ���ף�" + MembersOfThisTree[childMemberToGo].father + "��ԭ���ӣ�" + childMemberToGo);
        //��Ա����丸�׵�����
        MembersOfThisTree[childMemberToGo].father = -1;

        //����ɾȥ�ú��ӵ�����
        MembersOfThisTree[fatherMember].children.Remove(childMemberToGo);
    }

    //��Ա��ɾ��������ȷ����ɾ��Ŀ�����ø÷������������������Ż�����ռ��˷�
    private void MemberDel(int IndexToDel)
    {
        //ɾ����Ա����Ե��ϵ����ֹ����
        MembersOfThisTree[IndexToDel].father = -1;
        MembersOfThisTree[IndexToDel].children.Clear();

        //����Ա��������������ռ����
        redundantIndexes.Enqueue(IndexToDel);
        Debug.Log(">->��Աɾ��<-<����Ա���" + IndexToDel + "��ɾ������ǰ����ռ�����й���" + redundantIndexes.Count + "������");

        memberCount--;
    }

    //���·���Ϊ����ѡ����Աɾ��������������������ɾ����ÿ��ɾ��������ٳ�Ա����������

    public void MemberRemoveFatherWithAncestor(int IndexOfChild)
    {
        SubTreeRemoveWithCheck(GetRootMember(IndexOfChild), IndexOfChild);
    }

    /// <summary>
    /// �÷����Ӹ����ĸ��ڵ㿪ʼ�������������ɾ����Ա����ɾ������ֹ�ڵ��֧ϵ.
    /// </summary>
    /// <param name="IndexOfDel">ɾ����ʼ�ĸ��ڵ�.</param>
    /// <param name="IndexAvoid">������ɾ������ϵ���ڵ㣬��û�пɲ���ֵ.</param>
    public void SubTreeRemoveWithCheck(int IndexOfRoot, int IndexAvoid = -1)
    {
        if (IndexAvoid != -1) Debug.Log(">->��Ա�ݹ�ɾ��<-<������Ա���" + IndexOfRoot + "���ܿ���Ա���" + IndexAvoid);
        else Debug.Log(">->��Ա�ݹ�ɾ��<-<������Ա���" + IndexOfRoot + "�����ܿ���Ա");
        //���ȼ������ĸ��ڵ��Ƿ��и��ף����У�ɾ�����ڸ��״�������
        MemberSeverRelationAsChild(IndexOfRoot, MembersOfThisTree[IndexOfRoot].father);

        //�����ֹ�Ľڵ��Ƿ��и��ף����У�ɾ�����ڸ��״����������Ӷ�ʹ�䲻���ܱ��ҵ�
        if(IndexAvoid != -1)
            MemberSeverRelationAsChild(IndexAvoid, MembersOfThisTree[IndexAvoid].father);

        //���Ž���ݹ飬��ʼ�������ɾ��
        MemberRemoveWithCheck(IndexOfRoot);
    }

    //��Ա�ĵݹ�ɾ�������������������ɾ����Ա
    private void MemberRemoveWithCheck(int IndexOfNow)
    {
        if(MembersOfThisTree[IndexOfNow].children.Count != 0)
        {
            foreach (var child in MembersOfThisTree[IndexOfNow].children)
            {
                //ֻҪ���ӵ�������Ϊ��ֹ�������������µݹ�ɾ��
                MemberRemoveWithCheck(child);
            }
        }
        //�Ժ��������ʽ�ԶԳ�Ա�Լ�����ɾ��
        MemberDel(IndexOfNow);
    }
}

/// <summary>
/// �������࣬�������ļ������Դ�ȡ���ݣ��ṩ��������ʽ.
/// </summary>
[System.Serializable]
public class TreeData
{
    //������Ϣ
    public string surname;

    public int genDepth;

    public int chiefIndex;

    public int memberCount;

    public Queue<int> redundantIndexes;

    //��Ա����Ϣ
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
