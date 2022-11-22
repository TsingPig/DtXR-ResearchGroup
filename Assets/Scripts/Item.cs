using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Color originalColor;
    private void Start()
    {
        originalColor=GetComponent<Renderer>().material.color;
        itemObject = gameObject;

        itemName = itemObject.name;
    }
    [HideInInspector] public string itemName;
    [HideInInspector] public GameObject itemObject;
    
    

}
