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

        //在文件中获取提示信息
        TextAsset jsonText = Resources.Load("Prompts") as TextAsset;
        string JsonString = jsonText.text;
        jsonList = JsonUtility.FromJson<JsonData>(JsonString);

        //进入系统时初始化静态全局变量
        Debug.Log("<[AgentJobs]>：正在新建项目...创建新树");
        DataOfThisTree = new TreeData();
        TreeOfThisFile = new FamilyTree();
        AreaTransfer.memberQuantityOfAreas = new int[6]{ 0, 0, 0, 0, 0, 0 };

        saveText.text = "Tree0";
        loadText.text = "Tree0";

        loadButton.onClick.AddListener(delegate {
            //弹出弹窗引导用户确认加载
            loadDialog.SetActive(true);
        });

        saveButton.onClick.AddListener(delegate {
            Debug.Log("<[AgentJobs]>：试图将文件保存至：文件名 " + saveText.text + ".efft");
            StartCoroutine(nameof(SaveTreeDataAsync), saveText.text);
        });

        loadConfirm.onClick.AddListener(delegate
        {
            loadDialog.SetActive(false);
            Debug.Log("<[AgentJobs]>：试图加载文件，来自文件 " + loadText.text + ".efft");
            StartCoroutine(nameof(LoadTreeDataAsync), loadText.text);
        });

        loadCencel.onClick.AddListener(delegate
        {
            loadDialog.SetActive(false);
        });
    }

    private void Start()
    {
        //向聚焦圈中填入族长，从而开始各个区域的加载
        GetComponent<MemberPool>().PoolPop(focusCircle, TreeOfThisFile.chiefIndex);
    }

    //使用序列化方式将树的数据存储到文件中
    public IEnumerator SaveTreeDataAsync(string dataTo)
    {
        //先检查家族是否有族长存在，若无，提示用户指定族长
        if (TreeOfThisFile.chiefIndex == -1 || TreeOfThisFile.redundantIndexes.Contains(TreeOfThisFile.chiefIndex))
        {
            TransferPrompt = findPrompt("saveWithoutChief");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //检查编辑区是否有成员存在，若有，提示用户其不会被保存
        if (AreaTransfer.memberQuantityOfAreas[1] == 1)
        {
            TransferPrompt = findPrompt("saveWithEditor");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //检查文件名是否合法
        if (!textCheck(dataTo))
        {
            TransferPrompt = findPrompt("saveTextIlligal");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        savePanel.SetActive(true);
        Debug.Log("<[AgentJobs]>：开始执行打包操作...打包总量为" + TreeOfThisFile.MembersOfThisTree.Count + "个成员");
        //打包文件，对于队列和数组，先初始化为最终需要的容量再载入数值
        DataOfThisTree.surname = TreeOfThisFile.surname;
        DataOfThisTree.genDepth = TreeOfThisFile.genDepth;
        DataOfThisTree.chiefIndex = TreeOfThisFile.chiefIndex;
        DataOfThisTree.memberCount = TreeOfThisFile.memberCount;
        DataOfThisTree.redundantIndexes = new Queue<int>(TreeOfThisFile.redundantIndexes.Count);
        DataOfThisTree.redundantIndexes = TreeOfThisFile.redundantIndexes;

        //为各个成员的信息保存先行申请空间
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

        //循环向表中加入每个成员的对应信息
        foreach (var treeMember in TreeOfThisFile.MembersOfThisTree)
        {
            int i = treeMember.index;
            Debug.Log("[AgentJobs]：正在保存编号为" + i + "的成员，成员名为" + treeMember.surname + treeMember.name);
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

        //使用文件流创建文件，并将封装后的数据序列化存入文件中
        BinaryFormatter BF = new BinaryFormatter();
        FileStream FS = File.Create(Application.dataPath + "/" + dataTo + ".efft");
        BF.Serialize(FS, DataOfThisTree);
        FS.Close();

        //未来的优化空间：主动清理数据类，腾出其临时占用的内存空间

        //提示用户保存成功
        savePanel.SetActive(false);
        Debug.Log("<[AgentJobs]>：保存成功！");
        TransferPrompt = findPrompt("saveSucceed");
        prompt?.SetActive(true);
        prompt.GetComponent<PromptManage>().PromptAppear();
        yield break;
    }

    //对文件中的数据进行反序列化，加载到场景内，传入值为数据的路径，若为空，则直接创建新树
    public IEnumerator LoadTreeDataAsync(string dataFrom)
    {
        //检查编辑区是否有成员存在，若有，提示用户其不会被保存
        if (AreaTransfer.memberQuantityOfAreas[1] == 1)
        {
            TransferPrompt = findPrompt("loadWithEditor");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //检查文件名是否合法
        if (!textCheck(dataFrom))
        {
            TransferPrompt = findPrompt("loadTextIlligal");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //检查指定的文件是否存在
        if (!File.Exists(Application.dataPath + "/" + dataFrom + ".efft"))
        {
            TransferPrompt = findPrompt("loadFileIlligal");
            prompt?.SetActive(true);
            prompt.GetComponent<PromptManage>().PromptAppear();
            yield break;
        }

        //事先清屏
        focusCircle.GetComponent<FocusCircle>().clearMembers();

        loadPanel.SetActive(true);
        //同样使用文件流反序列化文件信息，将其存入专用的数据类
        BinaryFormatter BF = new BinaryFormatter();
        FileStream FS = File.Open(Application.dataPath + "/" + dataFrom + ".efft", FileMode.Open);
        DataOfThisTree = BF.Deserialize(FS) as TreeData;
        FS.Close();

        //从数据类中取出树的信息，交付给家族树类
        Debug.Log("<[AgentJobs]>：开始进行文件加载...一共需加载" + DataOfThisTree.index.Count + "个成员");
        TreeOfThisFile.surname = DataOfThisTree.surname;
        TreeOfThisFile.genDepth = DataOfThisTree.genDepth;
        TreeOfThisFile.chiefIndex = DataOfThisTree.chiefIndex;
        TreeOfThisFile.memberCount = DataOfThisTree.memberCount;
        TreeOfThisFile.redundantIndexes = new Queue<int>(DataOfThisTree.redundantIndexes.Count);
        TreeOfThisFile.redundantIndexes = DataOfThisTree.redundantIndexes;

        //循环从数据类中提取各个成员的信息，交付给家族树中的每个成员
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
            Debug.Log("[AgentJobs]：正在加载编号为" + i + "的成员，成员名为" + treeMember.surname + treeMember.name);
            TreeOfThisFile.MembersOfThisTree.Add(treeMember);
        }

        //未来的优化空间：主动清理数据类，腾出其临时占用的内存空间

        //用对象池的pop更新聚焦圈成员，从而由聚焦圈的getMember方法更新全部区域的成员
        GetComponent<MemberPool>().PoolPop(focusCircle, TreeOfThisFile.chiefIndex);

        //提示用户读取成功
        Debug.Log("<[AgentJobs]>：读取完成！");
        loadPanel.SetActive(false);
        TransferPrompt = findPrompt("loadSucceed");
        prompt?.SetActive(true);
        prompt.GetComponent<PromptManage>().PromptAppear();
        yield break;
    }
}
