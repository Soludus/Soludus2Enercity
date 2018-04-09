using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareValueObjective : Objective
{
    public Compare compare = Compare.Equals;
    public Object valueA = null;
    public int valueB = 0;

    public enum Compare
    {
        Equals,
        Greater,
        Less,
        Gte,
        Lte
    }

    public override bool IsComplete()
    {
        var valA = SourceProvider<float>.GetValueProvider(valueA, this);
        return CompareValues(valA.GetValue(), valueB, compare);
    }

    public bool CompareValues(float a, float b, Compare c)
    {
        switch (c)
        {
            case Compare.Equals:
                return a == b;
            case Compare.Greater:
                return a > b;
            case Compare.Less:
                return a < b;
            case Compare.Gte:
                return a >= b;
            case Compare.Lte:
                return a <= b;
        }

        return true;
    }
}
