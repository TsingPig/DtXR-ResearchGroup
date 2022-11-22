using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public  class HighLight:MonoBehaviour
{
    

    public void EnterHighLight(GameObject gameObject)
    {

        var renderer = gameObject.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.red);
    }
    public void ExitHightLight(GameObject gameObject)
    {
        var renderer = gameObject.GetComponent<Renderer>();
        renderer.material.SetColor("_Color", Color.white);

    }

}
