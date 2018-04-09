using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerTooltip : MonoBehaviour
{
    public VRTK_ControllerTooltips.TooltipButtons tooltipButton = VRTK_ControllerTooltips.TooltipButtons.None;
    public string text = "";
    public bool rightHand = true;
    public bool leftHand = true;

    private VRTK_ControllerTooltips tooltipsR = null;
    private VRTK_ControllerTooltips tooltipsL = null;

    private VRTK_ControllerTooltips FetchHighlighter(GameObject hand)
    {
        if (hand != null)
        {
            return hand.GetComponentInChildren<VRTK_ControllerTooltips>();
        }
        return null;
    }

    private void OnEnable()
    {
        Highlight();
    }

    private void OnDisable()
    {
        Unhighlight();
    }

    private void Update()
    {
        if ((rightHand && tooltipsR == null) || (leftHand && tooltipsL == null))
        {
            Highlight();
        }
    }

    public void Highlight()
    {
        tooltipsR = rightHand ? FetchHighlighter(VRTK_DeviceFinder.GetControllerRightHand()) : null;
        tooltipsL = leftHand ? FetchHighlighter(VRTK_DeviceFinder.GetControllerLeftHand()) : null;

        if (tooltipsR != null)
            Highlight(tooltipsR);
        if (tooltipsL != null)
            Highlight(tooltipsL);
    }

    public void Unhighlight()
    {
        if (tooltipsR != null)
            Unhighlight(tooltipsR);
        if (tooltipsL != null)
            Unhighlight(tooltipsL);
    }

    private void Highlight(VRTK_ControllerTooltips h)
    {
        h.UpdateText(tooltipButton, text);
    }

    private void Unhighlight(VRTK_ControllerTooltips h)
    {
        h.UpdateText(tooltipButton, "");
    }
}
