using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditingBehaviors : BaseClasses
{
    private GameObject memberInputing;

    //����Ϊ���ӳ�Ա�еĸ��������
    public GameObject surnameI;

    public GameObject nameI;

    public GameObject birthI;

    public GameObject surnameS;

    public GameObject nameS;

    public GameObject birthS;

    public GameObject isChief;

    private bool isEditorArea; //�жϵ�ǰ����������ĳ�Ա���ڱ༭�����Ǿ۽�Ȧ

    //�༭����ó�Աʱ��֪ͨ��������༭���еĳ�Ա��Ϊ�����Ĭ�����֣�����۽�Ȧ�г�Ա��ȡ��֮
    public void EditorChange(bool _isEditorArea)
    {
        isEditorArea = _isEditorArea;
        if (isEditorArea) memberInputing = editorArea.GetComponent<EditorArea>().memberEditing;
        else memberInputing = focusCircle.GetComponent<FocusCircle>().focusMember;
        //�޸����Ա�����չʾ�ֵ��޸�
        showChange();
    }

    //�޸ĺ����������������չʾ���ݣ��Լ��Ƿ�Ϊ�峤
    private void showChange()
    {
        surnameI.GetComponent<TMP_InputField>().text = memberInputing.GetComponent<MemberData>().getData().surname;
        nameI.GetComponent<TMP_InputField>().text = memberInputing.GetComponent<MemberData>().getData().name;
        if(memberInputing.GetComponent<MemberData>().getData().birthYear != -1)
            birthI.GetComponent<TMP_InputField>().text =
                memberInputing.GetComponent<MemberData>().getData().birthYear.ToString();
        else birthI.GetComponent<TMP_InputField>().text = string.Empty;

        surnameS.GetComponent<TMP_InputField>().text = memberInputing.GetComponent<MemberData>().getData().spouseSurname;
        nameS.GetComponent<TMP_InputField>().text = memberInputing.GetComponent<MemberData>().getData().spouseName;
        if (memberInputing.GetComponent<MemberData>().getData().spouseBirthYear != -1)
            birthS.GetComponent<TMP_InputField>().text =
                memberInputing.GetComponent<MemberData>().getData().spouseBirthYear.ToString();
        else birthS.GetComponent<TMP_InputField>().text = string.Empty;

        //ȡ���ڳ�Ա�Ƿ�Ϊ�峤�������峤��ť����ͼ
        if (TreeOfThisFile.chiefIndex != memberInputing.GetComponent<MemberData>().memberData)
            isChief.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/isntChief");
        else isChief.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/isChief");
    }

    public void getSurnameInput(string inputData)
    {
        memberInputing.GetComponent<MemberData>().getData().surname = inputData;
        memberInputing.GetComponent<MemberData>().fullnamePrintI();
    }

    public void getNameInput(string inputData)
    {
        memberInputing.GetComponent<MemberData>().getData().name = inputData;
        memberInputing.GetComponent<MemberData>().fullnamePrintI();
    }

    public void getBirthInput(string inputData)
    {
        int newBrithYear;
        int.TryParse(inputData, out newBrithYear);
        memberInputing.GetComponent<MemberData>().getData().birthYear = newBrithYear;
    }

    public void getSpouseSurnameInput(string inputData)
    {
        memberInputing.GetComponent<MemberData>().getData().spouseSurname = inputData;
        memberInputing.GetComponent<MemberData>().fullnamePrintS();
    }

    public void getSpouseNameInput(string inputData)
    {
        memberInputing.GetComponent<MemberData>().getData().spouseName = inputData;
        memberInputing.GetComponent<MemberData>().fullnamePrintS();
    }

    public void getSpouseBirthInput(string inputData)
    {
        int newBrithYear;
        int.TryParse(inputData, out newBrithYear);
        memberInputing.GetComponent<MemberData>().getData().spouseBirthYear = newBrithYear;
    }

    public void setManMain()
    {
        memberInputing.GetComponent<MemberData>().ManMainPrint();
        memberInputing.GetComponent<MemberData>().getData().sex = true;
    }

    public void setWomanMain()
    {
        memberInputing.GetComponent<MemberData>().WomanMainPrint();
        memberInputing.GetComponent<MemberData>().getData().sex = false;
    }

    public void setChief()
    {
        //ֻ���ڳ�Ա���ڼ������У�Ϊ�۽���Ա��������£������޸����峤״̬
        if (!isEditorArea)
        {
            if (TreeOfThisFile.chiefIndex != memberInputing.GetComponent<MemberData>().memberData)
            {
                TreeOfThisFile.chiefIndex = memberInputing.GetComponent<MemberData>().memberData;
                isChief.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/isChief");
            }
            else
            {
                TreeOfThisFile.chiefIndex = -1;
                isChief.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/isntChief");
            }
        }
    }
}
