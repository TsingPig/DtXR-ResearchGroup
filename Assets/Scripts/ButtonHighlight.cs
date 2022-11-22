using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonHighlight : Button
{
    public bool isHighlighted = false;
    public Animator animator;
    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        GetComponent<Image>().alphaHitTestMinimumThreshold= 0.9f;
    }
    private void Update()
    {
        animator.SetBool("isHighLight",isHighlighted);   

    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        switch (state)
        {
            case SelectionState.Highlighted:
                isHighlighted = true;
                break;
            default:
                isHighlighted = false;
                break;
        }
    }

   
}
