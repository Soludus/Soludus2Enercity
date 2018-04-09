using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSequence : Objective
{
    public List<Objective> objectives = new List<Objective>();
    public bool useChildren = true;
    public bool activateCurrentObjective = true;

    public float startDelay = 0;

    private int currentObjective = 0;

    private void Awake()
    {
        if (activateCurrentObjective)
        {
            for (int i = 0; i < objectives.Count; ++i)
            {
                objectives[i].gameObject.SetActive(false);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        currentObjective = 0;
        if (useChildren)
        {
            objectives.Clear();
            for (int i = 0; i < transform.childCount; ++i)
            {
                var obj = transform.GetChild(i).GetComponent<Objective>();
                if (obj != null && obj.enabled)
                    objectives.Add(obj);
            }
        }
        if (activateCurrentObjective)
        {
            for (int i = 0; i < objectives.Count; ++i)
            {
                objectives[i].gameObject.SetActive(false);
            }
        }
        StartCoroutine(UpdateObjectives());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
        if (activateCurrentObjective)
        {
            for (int i = 0; i < objectives.Count; ++i)
            {
                objectives[i].gameObject.SetActive(false);
            }
        }
    }

    public override bool IsComplete()
    {
        return currentObjective >= objectives.Count && objectives[objectives.Count - 1].IsComplete();
    }

    private IEnumerator UpdateObjectives()
    {
        yield return new WaitForSeconds(startDelay);
        while(true)
        {
            if (activateCurrentObjective)
            {
                for (int i = 0; i < objectives.Count; ++i)
                {
                    objectives[i].gameObject.SetActive(i == currentObjective);
                }
            }

            if (currentObjective < objectives.Count && objectives[currentObjective].IsComplete())
            {
                ++currentObjective;
            }
            else if (currentObjective > 0)
            {
                if (currentObjective >= objectives.Count)
                {
                    if (objectives[objectives.Count - 1].returnIfNotComplete && !objectives[objectives.Count - 1].IsComplete())
                        --currentObjective;
                }
                else
                {
                    if (objectives[currentObjective].returnIfNotComplete && !objectives[currentObjective - 1].IsComplete())
                        --currentObjective;
                }
            }

            yield return null;
        }
    }
}
