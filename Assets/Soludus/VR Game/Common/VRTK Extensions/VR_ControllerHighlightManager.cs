using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerHighlight
{
    public VRTK_ControllerTooltips.TooltipButtons tooltipButton;
    public SDK_BaseController.ControllerElements[] highlightElements = null;
    public string tooltipText;
    public Color highlightColor;
}

public class VR_ControllerHighlightManager : MonoBehaviour
{
    public VRTK_ControllerHighlighter highlighter = null;
    public VRTK_ControllerTooltips tooltips = null;

    private List<ControllerHighlight> highlights = new List<ControllerHighlight>();

    private void OnEnable()
    {
        var evnts = GetComponentInParent<VRTK_ControllerEvents>();
        if (highlighter == null)
            highlighter = evnts.GetComponentInChildren<VRTK_ControllerHighlighter>();
        if (tooltips == null)
            tooltips = evnts.GetComponentInChildren<VRTK_ControllerTooltips>();
    }

    public void AddHighlight(ControllerHighlight highlight)
    {
        highlights.Add(highlight);

        if (!string.IsNullOrEmpty(highlight.tooltipText) && highlight.tooltipButton != VRTK_ControllerTooltips.TooltipButtons.None)
        {
            string tooltip = "";

            for (int i = 0; i < highlights.Count; ++i)
            {
                if (highlights[i].tooltipButton == highlight.tooltipButton)
                {
                    if (!string.IsNullOrEmpty(tooltip))
                        tooltip += "\n";
                    tooltip += highlights[i].tooltipText;
                }
            }
            tooltips.UpdateText(highlight.tooltipButton, tooltip);
        }

        if (highlight.highlightElements != null)
        {
            for (int i = 0; i < highlight.highlightElements.Length; ++i)
            {
                highlighter.HighlightElement(highlight.highlightElements[i], highlight.highlightColor);
            }
        }
    }

    public void RemoveHighlight(ControllerHighlight highlight)
    {
        int idx = -1;
        bool[] keepHighlight = highlight.highlightElements != null ? new bool[highlight.highlightElements.Length] : null;
        string tooltip = "";

        for (int i = 0; i < highlights.Count; ++i)
        {
            if (highlights[i] == highlight)
                idx = i;
            else
            {
                if (highlights[i].tooltipButton == highlight.tooltipButton)
                {
                    if (!string.IsNullOrEmpty(tooltip))
                        tooltip += "\n";
                    tooltip += highlights[i].tooltipText;
                }
                if (highlight.highlightElements != null && highlights[i].highlightElements != null)
                {
                    for (int j = 0; j < highlights[i].highlightElements.Length; ++j)
                    {
                        for (int k = 0; k < highlight.highlightElements.Length; ++k)
                        {
                            if (highlights[i].highlightElements[j] == highlight.highlightElements[k])
                                keepHighlight[k] = true;
                        }

                        //if (highlights[i].highlightElement == highlight.highlightElement)
                        //    keepHighlight = true;
                    }
                }
            }
        }

        if (idx != -1)
        {
            tooltips.UpdateText(highlight.tooltipButton, tooltip);
            if (keepHighlight != null)
            {
                for (int i = 0; i < keepHighlight.Length; ++i)
                {
                    if (!keepHighlight[i])
                    {
                        highlighter.UnhighlightElement(highlight.highlightElements[i]);
                    }
                }
            }
            highlights.RemoveAt(idx);
        }
    }
}