using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditingBehaviors : BaseClasses
{
    private GameObject memberInputing;

    //以下为孩子成员中的各个输入框
    public GameObject surnameI;

    public GameObject nameI;

    public GameObject birthI;

    public GameObject surnameS;

    public GameObject nameS;

    public GameObject birthS;

    public GameObject isChief;

    private bool isEditorArea; //判断当前的输入框管理的成员属于编辑区还是聚焦圈

    //编辑区获得成员时将通知画布，令编辑区中的成员成为输入框默认文字，否则聚焦圈中成员将取代之
    public void EditorChange(bool _isEditorArea)
    {
        isEditorArea = _isEditorArea;
        if (isEditorArea) memberInputing = editorArea.GetComponent<EditorArea>().memberEditing;
        else memberInputing = focusCircle.GetComponent<FocusCircle>().focusMember;
        //修改完成员后进行展示字的修改
        showChange();
    }

    //修改孩子物体各个输入框的展示内容，以及是否为族长
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

        //取决于成员是否为族长，决定族长按钮的贴图
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
        //只有在成员处在家族树中（为聚焦成员）的情况下，才能修改其族长状态
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
