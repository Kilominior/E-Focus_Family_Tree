using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MemberMovement : BaseClasses
{
    public float adsorbingSpeed = 0.5f; //��Ա�����������ƶ��ٶȣ�������ƶ�����

    public bool isBound { private get; set; } //��Ա�Ŀ��ƶ��ԣ���Ϊfalse��ζ�ų�Ա�ھ۽�Ȧ�У������ƶ�

    private bool firstRun = true; //��¼��Ա�Ƿ��Ǹձ����������ǣ����ʼ����������

    //�����˳��������Ŀ�ĵ�����Ķ������ã����ڴ����¼�
    //������޸���˽�еģ���������ֻ�ܶ�ȡ֮
    public GameObject DepArea { get; private set; }

    public GameObject DestArea { get; private set; }
    //�������������
    //FocusCircle, EditorArea, BrotherBox, ChildenBox, FatherCircle, GrandpaCircle, TrashArea

    public Vector3 PointOfDeparture; //��Ա�ĳ����㣬����Ա��Ҫ������ʹ��֮
    
    private Vector3 destination; //��Ա�ƶ����յ�

    private bool isNotAdsorbing = true; //���ڱ�����ʱ��Ŀ�ĵز��ܱ����ģ�����������;Ŀ�ĵظı�����������

    private bool isPaused = false; //��Ϸ��ͣ״̬

    private IEnumerator OnMouseDown() //�������ʱ�����϶�Э��
    {
        //ֻ���ڳ�Ա���Ա��ƶ���Ҳ�����ھ۽�Ȧ��ʱ����������Ӧ���ĵ��
        if (!isBound)
        {
            //����ʱ�Ŵ��϶�Ŧ
            transform.Find("DragButton").localScale = new(0.5f, 0.5f);
            //��ס�����أ����Ҳ���Ŀ�ĵ�ʱ���ܷ���
            PointOfDeparture = transform.position;
            DestArea = DepArea;
            destination = PointOfDeparture;
            //����걣�ְ���ʱ��ÿfixedUpdate�����屣�������λ����
            while (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = mousePosition;
                yield return new WaitForFixedUpdate();
            }
            //����ʱ�϶�Ŧ��С��ԭ��
            transform.Find("DragButton").localScale = new(0.3f, 0.3f);
            if (!isPaused)
            {
                if (destination == DestArea.GetComponent<AreaTransfer>().rejectVector)
                {
                    //�����Ŀ�ĵ��յ���Ŀ���ΪԼ���ľܾ��㣬������ҷ�����ʾ��Ϣ�������ܾ�ԭ��
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
    //�����˳����������ͣ�¼���һ����Ϸ��ͣ��isPaused����Ϊtrue����ֹĳЩ�ж�

    public void gameContinue() => isPaused = false;
    //�����˳�������ļ����¼�����Ϸ����ʱʹisPaused�ָ�

    private IEnumerator getAdsorbed()
    {
        isNotAdsorbing = false;
        //����ʱ֪ͨ�뿪�ĺͽ���Ĺ�������
        //��Ŀ�ĵؾ��ǳ����أ�����Ҫ֪ͨ
        //��������Ϊ�ֵܺл��ӺУ����յ㲻Ϊ�۽�Ȧ��֪ͨʱ������������������Ե��ϵ�Ͼ�
        Debug.Log("<��Ա�ƶ�>����Ա���" + gameObject.GetComponent<MemberData>().memberData +
            "�ƶ��������� " + DepArea.name + "��Ŀ�ĵ�Ϊ " + DestArea.name);
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
        //�����������ζ��Ŀ�ĵر������һ�εĳ�����
        DepArea = DestArea;
        isNotAdsorbing = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (firstRun)
        {
            //ֻ���ڳ�Ա�ձ�����ʱ�Ż���ô˾�
            firstRun = false;
            DepArea = collision.gameObject;
            return;
        }
        if(isNotAdsorbing)
        {
            if (collision.name == "TrashArea")
            {
                //�������ͣ��ɾ�����ϵ�ʱ�䲻���ͱ��ſ�����ô��Ϊ���ȡ����ɾ������ô�յ㽫�ᱻ��Ϊ���
                DestArea = DepArea;
                destination = PointOfDeparture;
            }
            else
            {
                //��������˺����е�ҳ�뽺�ҵ������壬��ȡ�丸������Ϊ�յ㣬������ָ��Ŀ�ĵ�
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
