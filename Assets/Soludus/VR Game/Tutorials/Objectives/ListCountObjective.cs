using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListCountObjective : Objective
{
    public GameObjectEnumerator input = null;

    public enum Compare
    {
        Equals,
        Greater,
        Less,
        Gte,
        Lte
    }

    public Compare compare = Compare.Equals;
    public int value = 0;

    public override bool IsComplete()
    {
        int c = 0;
        foreach (var item in input.GetObjects())
        {
            c++;
        }

        switch (compare)
        {
            case Compare.Equals:
                return c == value;
            case Compare.Greater:
                return c > value;
            case Compare.Less:
                return c < value;
            case Compare.Gte:
                return c >= value;
            case Compare.Lte:
                return c <= value;
        }

        return true;
    }
}
