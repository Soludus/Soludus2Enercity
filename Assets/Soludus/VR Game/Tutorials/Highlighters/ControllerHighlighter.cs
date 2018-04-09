using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerHighlighter : Highlighter
{
    public List<SDK_BaseController.ControllerElements> elements = new List<SDK_BaseController.ControllerElements>();
    public Color color = Color.yellow;
    public bool rightHand = true;
    public bool leftHand = true;

    private VRTK_ControllerHighlighter highlighterR = null;
    private VRTK_ControllerHighlighter highlighterL = null;

    private VRTK_ControllerHighlighter FetchHighlighter(GameObject hand)
    {
        if (hand != null)
        {
            return hand.GetComponentInChildren<VRTK_ControllerHighlighter>();
        }
        return null;
    }

    private void Update()
    {
        //if ((rightHand && highlighterR == null) || (leftHand && highlighterL == null))
        //{
        if (activateOnEnable)
            Highlight();
        //}
    }

    public override void Highlight()
    {
        highlighterR = rightHand ? FetchHighlighter(VRTK_DeviceFinder.GetControllerRightHand()) : null;
        highlighterL = leftHand ? FetchHighlighter(VRTK_DeviceFinder.GetControllerLeftHand()) : null;

        if (highlighterR != null)
            Highlight(highlighterR);
        if (highlighterL != null)
            Highlight(highlighterL);
    }

    public override void Unhighlight()
    {
        if (highlighterR != null)
            Unhighlight(highlighterR);
        if (highlighterL != null)
            Unhighlight(highlighterL);
    }

    private void Highlight(VRTK_ControllerHighlighter h)
    {
        for (int i = 0; i < elements.Count; ++i)
        {
            h.HighlightElement(elements[i], color);
        }
    }

    private void Unhighlight(VRTK_ControllerHighlighter h)
    {
        for (int i = 0; i < elements.Count; ++i)
        {
            h.UnhighlightElement(elements[i]);
        }
    }
}
