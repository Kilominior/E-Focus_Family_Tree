using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTracer : MonoBehaviour
{
    public GameObject BBox;

    public GameObject CBox;

    private void Awake()
    {

        //����е�һЩUI�ؼ�������Ļ��С����λ�õ�����ʵ������
        transform.Find("BrotherReporter").position = BBox.transform.position;
        transform.Find("ChildrenReporter").position = CBox.transform.position;
    }
}
