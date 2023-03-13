using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxQuantitiesReport : BaseClasses
{
    private int quantityTheNumber = 0; //��Ӧ���ӵ�����

    private bool isFull = false; //�����Ƿ����������ڸı���ɫ���ж�

    public int boxCapacity; //�����Ӧ�ĺ�����

    private void Start()
    {
        if (name == "BrotherQuantityText") boxCapacity = 15;
        else boxCapacity = 16;
    }

    public void quantityAdd()
    {
        int.TryParse(GetComponent<TextMeshProUGUI>().text, out quantityTheNumber);
        quantityTheNumber++;
        GetComponent<TextMeshProUGUI>().text = quantityTheNumber.ToString();
        
        //�����Ӧ������������������ɫ��Ϊ��ʾ��ɫ
        if(quantityTheNumber == boxCapacity)
        {
            GetComponent<TextMeshProUGUI>().color = new(227 / 255f, 23 / 255f, 13 / 255f);
            isFull = true;
        }
    }

    public void quantityReduce()
    {
        //�����Ӧ�������������������Ͻ����٣�����ɫ�Ļذ�ɫ
        if (isFull)
        {
            isFull = false;
            GetComponent<TextMeshProUGUI>().color = new(255 / 255f, 255 / 255f, 255 / 255f);
        }

        int.TryParse(GetComponent<TextMeshProUGUI>().text, out quantityTheNumber);
        quantityTheNumber--;
        GetComponent<TextMeshProUGUI>().text = quantityTheNumber.ToString();
    }
}
