using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    //记录UI原位置
    Vector3 trans1;
    //简谐运动变化的位置，计算得出
    Vector2 trans2;

    //浮动的振幅
    public float floatHight = 0.1f;
    //浮动的频率
    public float floatRate = 1f;

    private void Start()
    {
        trans1 = transform.position;
    }

    private void Update()
    {
        trans2 = trans1;
        trans2.y = Mathf.Sin(Time.fixedTime * Mathf.PI * floatRate) * floatHight + trans1.y;

        transform.position = trans2;
    }
}
