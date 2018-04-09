using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : Highlighter
{
    public GameObjectEnumerator objects = null;
    public bool updateList = true;

    [Header("Indicator")]
    public GameObject indicatorPrefab = null;
    public float scale = 1;
    public Vector3 offset = new Vector3(0, 10, 0);
    public bool matchRotation = true;
    public Vector3 angleOffset = new Vector3(0, 0, 0);

    private IEnumerable<GameObject> objs = null;
    private List<GameObject> indicators = new List<GameObject>();

    protected override void OnEnable()
    {
        if (!updateList)
            objs = new List<GameObject>(objects.GetObjects());
        base.OnEnable();
    }

    private void LateUpdate()
    {
        if (updateList)
            objs = objects.GetObjects();

        int idx = 0;
        foreach (var item in objs)
        {
            if (indicators.Count <= idx)
                indicators.Add(Instantiate(indicatorPrefab));
            var ind = indicators[idx++];

            Vector3 off = offset;
            if (matchRotation)
            {
                var rot = item.transform.rotation * Quaternion.Euler(angleOffset);
                off = rot * off;
                ind.transform.rotation = rot;
            }
            ind.transform.localScale = scale * Vector3.one;
            ind.transform.position = item.transform.position + off;
        }

        for (int i = indicators.Count - 1; i >= idx; --i)
        {
            Destroy(indicators[i]);
            indicators.RemoveAt(i);
        }
    }

    public override void Highlight()
    {
        //for (int i = 0; i < objects.Count; ++i)
        //{
        //    var ind = Instantiate(indicatorPrefab);
        //    indicators.Add(ind);
        //}
    }

    public override void Unhighlight()
    {
        for (int i = indicators.Count - 1; i >= 0; --i)
        {
            Destroy(indicators[i]);
        }
        indicators.Clear();
    }
}
