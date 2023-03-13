using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainFollow : MonoBehaviour
{
    private Vector3 positionUp = new(0, 0.4f);

    private void FixedUpdate()
    {
        transform.position = transform.parent.parent.position + positionUp;
    }
}
