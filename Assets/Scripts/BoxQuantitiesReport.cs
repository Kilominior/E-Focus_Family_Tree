using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoxQuantitiesReport : BaseClasses
{
    private int quantityTheNumber = 0; //对应盒子的容量

    private bool isFull = false; //盒子是否已满，用于改变颜色的判定

    public int boxCapacity; //物体对应的盒子名

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
        
        //如果对应盒子容量已满，将颜色改为警示红色
        if(quantityTheNumber == boxCapacity)
        {
            GetComponent<TextMeshProUGUI>().color = new(227 / 255f, 23 / 255f, 13 / 255f);
            isFull = true;
        }
    }

    public void quantityReduce()
    {
        //如果对应盒子容量已满，但马上将变少，将颜色改回白色
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
