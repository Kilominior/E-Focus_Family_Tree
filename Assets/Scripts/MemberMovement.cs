using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MemberMovement : BaseClasses
{
    public float adsorbingSpeed = 0.5f; //成员进行吸附的移动速度（相对于移动距离

    public bool isBound { private get; set; } //成员的可移动性，若为false意味着成员在聚焦圈中，不可移动

    private bool firstRun = true; //记录成员是否是刚被创建，若是，则初始化出发区域

    //保存了出发区域和目的地区域的对象引用，用于触发事件
    //对其的修改是私有的，其他对象只能读取之
    public GameObject DepArea { get; private set; }

    public GameObject DestArea { get; private set; }
    //功能区域包括：
    //FocusCircle, EditorArea, BrotherBox, ChildenBox, FatherCircle, GrandpaCircle, TrashArea

    public Vector3 PointOfDeparture; //成员的出发点，若成员需要返回则使用之
    
    private Vector3 destination; //成员移动的终点

    private bool isNotAdsorbing = true; //不在被吸附时，目的地才能被更改，以免吸附中途目的地改变引发的问题

    private bool isPaused = false; //游戏暂停状态

    private IEnumerator OnMouseDown() //当鼠标点击时触发拖动协程
    {
        //只有在成员可以被移动，也即不在聚焦圈中时，它才能响应鼠标的点击
        if (!isBound)
        {
            //拿起时放大拖动纽
            transform.Find("DragButton").localScale = new(0.5f, 0.5f);
            //记住出发地，当找不到目的地时就能返回
            PointOfDeparture = transform.position;
            DestArea = DepArea;
            destination = PointOfDeparture;
            //当鼠标保持按下时，每fixedUpdate让物体保持在鼠标位置上
            while (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = mousePosition;
                yield return new WaitForFixedUpdate();
            }
            //放下时拖动纽缩小回原样
            transform.Find("DragButton").localScale = new(0.3f, 0.3f);
            if (!isPaused)
            {
                if (destination == DestArea.GetComponent<AreaTransfer>().rejectVector)
                {
                    //如果从目的地收到的目标点为约定的拒绝点，则向玩家发出提示信息，表明拒绝原因
                    prompt?.SetActive(true);
                    prompt.GetComponent<PromptManage>().PromptAppear();
                    DestArea = DepArea;
                    destination = PointOfDeparture;
                }
                StartCoroutine(nameof(getAdsorbed));
            }
        }
    }

    public void gamePause() => isPaused = true;
    //订阅了场景代理的暂停事件，一旦游戏暂停，isPaused将会为true，阻止某些行动

    public void gameContinue() => isPaused = false;
    //订阅了场景代理的继续事件，游戏继续时使isPaused恢复

    private IEnumerator getAdsorbed()
    {
        isNotAdsorbing = false;
        //吸附时通知离开的和进入的功能区域
        //若目的地就是出发地，则不需要通知
        //若出发地为兄弟盒或孩子盒，而终点不为聚焦圈，通知时将附加条件，进行亲缘关系断绝
        Debug.Log("<成员移动>：成员编号" + gameObject.GetComponent<MemberData>().memberData +
            "移动，出发自 " + DepArea.name + "；目的地为 " + DestArea.name);
        if(DepArea != DestArea)
        {
            if((DepArea.name == "BrotherBox" || DepArea.name == "ChildrenBox") && DestArea != focusCircle)
                DepArea.GetComponent<AreaTransfer>().releaseMember(gameObject, true);
            else DepArea.GetComponent<AreaTransfer>().releaseMember(gameObject);
            DestArea.GetComponent<AreaTransfer>().getMember(gameObject);
        }
        while (transform.position != destination)
        {
            Vector3 dir = destination - transform.position;
            transform.position += dir * adsorbingSpeed;
            yield return new WaitForFixedUpdate();
        }
        //完成吸附就意味着目的地变成了下一次的出发地
        DepArea = DestArea;
        isNotAdsorbing = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (firstRun)
        {
            //只有在成员刚被创建时才会调用此句
            firstRun = false;
            DepArea = collision.gameObject;
            return;
        }
        if(isNotAdsorbing)
        {
            if (collision.name == "TrashArea")
            {
                //若鼠标悬停在删除器上的时间不够就被放开，那么认为玩家取消了删除，那么终点将会被设为起点
                DestArea = DepArea;
                destination = PointOfDeparture;
            }
            else
            {
                //如果遇到了盒子中的页码胶囊等子物体，则取其父物体作为终点，并由其指定目的地
                if (collision.TryGetComponent<AreaTransfer>(out _))
                {
                    DestArea = collision.gameObject;
                    destination = collision.GetComponent<AreaTransfer>().destAppoint(gameObject);
                }
                else
                {
                    DestArea = collision.transform.parent.gameObject;
                    destination = collision.transform.parent.GetComponent<AreaTransfer>().destAppoint(gameObject);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isNotAdsorbing && collision.gameObject == DestArea) destination = PointOfDeparture;
    }
}
