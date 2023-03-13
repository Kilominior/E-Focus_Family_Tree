using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTracer : MonoBehaviour
{
    public GameObject BBox;

    public GameObject CBox;

    private void Awake()
    {

        //令画布中的一些UI控件根据屏幕大小做出位置调整，实现适配
        transform.Find("BrotherReporter").position = BBox.transform.position;
        transform.Find("ChildrenReporter").position = CBox.transform.position;
    }
}
