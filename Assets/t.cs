using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class t : MonoBehaviour
{
    private void Start()
    {
        SetRenderMode.SetTrans(gameObject);
        gameObject.GetComponent<Renderer>().material.color*=new Color(1f, 1f, 1f,0.1f);
        SetRenderMode.SetOpaque(gameObject);
        gameObject.GetComponent<Renderer>().material.color *= new Color(1f, 1f, 1f, 10f);

    }
}
