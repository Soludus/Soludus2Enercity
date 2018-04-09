using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoopData : MonoBehaviour
{
    public List<GameObject> splatPrefabs = new List<GameObject>();
    public float dropHeight;
    public float minInterval, maxInterval;
    public int maxCount;
    public int requiredWaterHitsToCleanSplat;
    public int requiredCleansToStop;

    internal int totalCount;
    internal int currentCount;
    internal int cleanedCount;
}
