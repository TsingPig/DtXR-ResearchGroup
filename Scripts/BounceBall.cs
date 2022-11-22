using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBall : MonoBehaviour
{
    Vector3 originalTransform;
    public float distanceToReset;
    private void Start()
    {
        originalTransform = transform.position;
    }
    private void Update()
    {
        if(Vector3.Distance(transform.position
            ,originalTransform) > distanceToReset)
        {
            transform.position = originalTransform;
        }

    }
}
