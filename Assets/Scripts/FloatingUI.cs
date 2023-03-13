using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    //��¼UIԭλ��
    Vector3 trans1;
    //��г�˶��仯��λ�ã�����ó�
    Vector2 trans2;

    //���������
    public float floatHight = 0.1f;
    //������Ƶ��
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
