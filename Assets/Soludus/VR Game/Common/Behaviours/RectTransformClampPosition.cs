using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class RectTransformClampPosition : MonoBehaviour
{
    [Tooltip("Which side to clamp vertically.")]
    public VerticalSide vertical;
    [Tooltip("Which side to clamp horizontally.")]
    public HorizontalSide horizontal;
    [Tooltip("Clamp relative to this RectTransform. If null, parent is used.")]
    public RectTransform clampTo = null;

    public enum VerticalSide
    {
        None,
        Top,
        Bottom
    }

    public enum HorizontalSide
    {
        None,
        Left,
        Right
    }

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Clamp();
    }

    Rect _myRect;
    Rect _clampRect;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_myRect.center, _myRect.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_clampRect.center, _clampRect.size);
    }

    private void Clamp()
    {
        if (clampTo == null)
            clampTo = rectTransform.parent ? rectTransform.parent.GetComponent<RectTransform>() : null;

        if (clampTo != null)
        {
            var clampRect = clampTo.rect;
            var myRect = rectTransform.rect;

            _myRect = myRect;
            _clampRect = clampRect;

            Vector2 delta = Vector2.zero;

            switch (vertical)
            {
                case VerticalSide.Top:
                    if (clampRect.yMax < myRect.yMax)
                        delta.y += clampRect.yMax - myRect.yMax;
                    break;
                case VerticalSide.Bottom:
                    if (clampRect.yMin > myRect.yMin)
                        delta.y += clampRect.yMin - myRect.yMin;
                    break;
            }

            switch (horizontal)
            {
                case HorizontalSide.Left:
                    if (clampRect.xMax < myRect.xMax)
                        delta.x += clampRect.xMax - myRect.xMax;
                    break;
                case HorizontalSide.Right:
                    if (clampRect.xMin > myRect.xMin)
                        delta.x += clampRect.xMin - myRect.xMin;
                    break;
            }

            if (delta != Vector2.zero)
            {
                rectTransform.anchoredPosition += delta;
            }
        }
    }
}
