using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class AgentJobs : BaseClasses
{
    public TMP_InputField saveText;

    public TMP_InputField loadText;

    public Button saveButton;

    public Button loadButton;

    public GameObject loadDialog;

    public Button loadConfirm;

    public Button loadCencel;

    public GameObject savePanel;

    public GameObject loadPanel;

    public float SLWaitTime = 0.5f;

    private void Awake()
    {
        sceneAgent = gameObject;
        focusCircle = GameObject.Find("/FocusCircle");
        canvas = GameObject.Find("/Canvas");
        editorArea = GameObject.Find("/EditorArea");
        prompt = canvas.transform.Find("Prompt").gameObject;

        //���ļ��л�ȡ��ʾ��Ϣ
        TextAsset jsonText = Resources.Load("Prompts") as TextAsset;
        string JsonString = jsonText.text;
        jsonList = JsonUtility.FromJson<JsonData>(JsonString);

        //����ϵͳʱ��ʼ����̬ȫ�ֱ���
        Debug.Log("<[AgentJobs]>�������½���Ŀ...��������");
        DataOfThisTree = new TreeData();
        TreeOfThisFile = new FamilyTree();
        AreaTransfer.memberQuantityOfAreas = new int[6]{ 0, 0, 0, 0, 0, 0 };

        saveText.text = "Tree0";
        loadText.text = "Tree0";

        loadButton.onClick.AddListener(delegate {
            //�������������û�ȷ�ϼ���
            loadDialog.SetActive(true);
        });

        saveButton.onClick.AddListener(delegate {
            Debug.Log("<[AgentJobs]>����ͼ���ļ����������ļ��� " + saveText.text + ".efft");
            StartCoroutine(nameof(SaveTreeDataAsync), saveText.text);
        });

        loadConfirm.onClick.AddListener(delegate
        {
            loadDialog.SetActive(false);
            Debug.Log("<[AgentJobs]>����ͼ�����ļ��������ļ� " + loadText.text + ".efft");
            StartCoroutine(nameof(LoadTreeDataAsync), loadText.text);
        });

        loadCencel.onClick.AddListener(delegate
        {
            loadDialog.SetActive(false);
        });
    }

    private void Start()
    {
        //��۽�Ȧ�������峤���Ӷ���ʼ��������ļ���
        GetComponent<MemberPool>().PoolPop(focusCircle, TreeOfThisFile.chiefIndex);
    }

    //ʹ�����л���ʽ���������ݴ洢���ļ���
    public IEnumerator SaveTreeDataAsync(string dataTo)
    {
        //�ȼ������Ƿ����峤���ڣ����ޣ���ʾ�û�ָ���峤
        if (TreeOfThisFile.chiefIndex == -1 || TreeOfThisFile.redundantIndexes.Contains(TreeOfThisFile.chiefIndex))
        {
            TransferPrompt = findPrompt("saveWithoutChief");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //���༭���Ƿ��г�Ա���ڣ����У���ʾ�û��䲻�ᱻ����
        if (AreaTransfer.memberQuantityOfAreas[1] == 1)
        {
            TransferPrompt = findPrompt("saveWithEditor");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //����ļ����Ƿ�Ϸ�
        if (!textCheck(dataTo))
        {
            TransferPrompt = findPrompt("saveTextIlligal");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        savePanel.SetActive(true);
        Debug.Log("<[AgentJobs]>����ʼִ�д������...�������Ϊ" + TreeOfThisFile.MembersOfThisTree.Count + "����Ա");
        //����ļ������ڶ��к����飬�ȳ�ʼ��Ϊ������Ҫ��������������ֵ
        DataOfThisTree.surname = TreeOfThisFile.surname;
        DataOfThisTree.genDepth = TreeOfThisFile.genDepth;
        DataOfThisTree.chiefIndex = TreeOfThisFile.chiefIndex;
        DataOfThisTree.memberCount = TreeOfThisFile.memberCount;
        DataOfThisTree.redundantIndexes = new Queue<int>(TreeOfThisFile.redundantIndexes.Count);
        DataOfThisTree.redundantIndexes = TreeOfThisFile.redundantIndexes;

        //Ϊ������Ա����Ϣ������������ռ�
        DataOfThisTree.index = new List<int>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.name = new List<string>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.memberSurname = new List<string>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.sex = new List<bool>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.birthYear = new List<int>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.spouseName = new List<string>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.spouseSurname = new List<string>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.spouseBirthYear = new List<int>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.father = new List<int>(TreeOfThisFile.MembersOfThisTree.Count);
        DataOfThisTree.children = new List<List<int>>(TreeOfThisFile.MembersOfThisTree.Count);

        //ѭ������м���ÿ����Ա�Ķ�Ӧ��Ϣ
        foreach (var treeMember in TreeOfThisFile.MembersOfThisTree)
        {
            int i = treeMember.index;
            Debug.Log("[AgentJobs]�����ڱ�����Ϊ" + i + "�ĳ�Ա����Ա��Ϊ" + treeMember.surname + treeMember.name);
            DataOfThisTree.index.Add(i);
            DataOfThisTree.name.Add(treeMember.name);
            DataOfThisTree.memberSurname.Add(treeMember.surname);
            DataOfThisTree.sex.Add(treeMember.sex);
            DataOfThisTree.birthYear.Add(treeMember.birthYear);
            DataOfThisTree.spouseName.Add(treeMember.spouseName);
            DataOfThisTree.spouseSurname.Add(treeMember.spouseSurname);
            DataOfThisTree.spouseBirthYear.Add(treeMember.spouseBirthYear);
            DataOfThisTree.father.Add(treeMember.father);
            DataOfThisTree.children.Add(treeMember.children);
        }

        //ʹ���ļ��������ļ���������װ����������л������ļ���
        BinaryFormatter BF = new BinaryFormatter();
        FileStream FS = File.Create(Application.dataPath + "/" + dataTo + ".efft");
        BF.Serialize(FS, DataOfThisTree);
        FS.Close();

        //δ�����Ż��ռ䣺�������������࣬�ڳ�����ʱռ�õ��ڴ�ռ�

        //��ʾ�û�����ɹ�
        savePanel.SetActive(false);
        Debug.Log("<[AgentJobs]>������ɹ���");
        TransferPrompt = findPrompt("saveSucceed");
        prompt?.SetActive(true);
        prompt.GetComponent<PromptManage>().PromptAppear();
        yield break;
    }

    //���ļ��е����ݽ��з����л������ص������ڣ�����ֵΪ���ݵ�·������Ϊ�գ���ֱ�Ӵ�������
    public IEnumerator LoadTreeDataAsync(string dataFrom)
    {
        //���༭���Ƿ��г�Ա���ڣ����У���ʾ�û��䲻�ᱻ����
        if (AreaTransfer.memberQuantityOfAreas[1] == 1)
        {
            TransferPrompt = findPrompt("loadWithEditor");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //����ļ����Ƿ�Ϸ�
        if (!textCheck(dataFrom))
        {
            TransferPrompt = findPrompt("loadTextIlligal");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //���ָ�����ļ��Ƿ����
        if (!File.Exists(Application.dataPath + "/" + dataFrom + ".efft"))
        {
            TransferPrompt = findPrompt("loadFileIlligal");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //��������
        focusCircle.GetComponent<FocusCircle>().clearMembers();

        loadPanel.SetActive(true);
        //ͬ��ʹ���ļ��������л��ļ���Ϣ���������ר�õ�������
        BinaryFormatter BF = new BinaryFormatter();
        FileStream FS = File.Open(Application.dataPath + "/" + dataFrom + ".efft", FileMode.Open);
        DataOfThisTree = BF.Deserialize(FS) as TreeData;
        FS.Close();

        //����������ȡ��������Ϣ����������������
        Debug.Log("<[AgentJobs]>����ʼ�����ļ�����...һ�������" + DataOfThisTree.index.Count + "����Ա");
        TreeOfThisFile.surname = DataOfThisTree.surname;
        TreeOfThisFile.genDepth = DataOfThisTree.genDepth;
        TreeOfThisFile.chiefIndex = DataOfThisTree.chiefIndex;
        TreeOfThisFile.memberCount = DataOfThisTree.memberCount;
        TreeOfThisFile.redundantIndexes = new Queue<int>(DataOfThisTree.redundantIndexes.Count);
        TreeOfThisFile.redundantIndexes = DataOfThisTree.redundantIndexes;

        //ѭ��������������ȡ������Ա����Ϣ���������������е�ÿ����Ա
        TreeOfThisFile.MembersOfThisTree = new List<FamilyMember>(DataOfThisTree.index.Count);
        for (int i = 0; i < DataOfThisTree.index.Count; i++)
        {
            FamilyMember treeMember = new FamilyMember(DataOfThisTree.memberSurname[i], i);
            treeMember.name = DataOfThisTree.name[i];
            treeMember.sex = DataOfThisTree.sex[i];
            treeMember.birthYear = DataOfThisTree.birthYear[i];
            treeMember.spouseName = DataOfThisTree.spouseName[i];
            treeMember.spouseSurname = DataOfThisTree.spouseSurname[i];
            treeMember.spouseBirthYear = DataOfThisTree.spouseBirthYear[i];
            treeMember.father = DataOfThisTree.father[i];
            treeMember.children = DataOfThisTree.children[i];
            Debug.Log("[AgentJobs]�����ڼ��ر��Ϊ" + i + "�ĳ�Ա����Ա��Ϊ" + treeMember.surname + treeMember.name);
            TreeOfThisFile.MembersOfThisTree.Add(treeMember);
        }

        //δ�����Ż��ռ䣺�������������࣬�ڳ�����ʱռ�õ��ڴ�ռ�

        //�ö���ص�pop���¾۽�Ȧ��Ա���Ӷ��ɾ۽�Ȧ��getMember��������ȫ������ĳ�Ա
        GetComponent<MemberPool>().PoolPop(focusCircle, TreeOfThisFile.chiefIndex);

        //��ʾ�û���ȡ�ɹ�
        Debug.Log("<[AgentJobs]>����ȡ��ɣ�");
        loadPanel.SetActive(false);
        TransferPrompt = findPrompt("loadSucceed");
        prompt?.SetActive(true);
        prompt.GetComponent<PromptManage>().PromptAppear();
        yield break;
    }
}
