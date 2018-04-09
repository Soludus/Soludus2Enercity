using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHazardsEnabled : MonoBehaviour
{
    public MapHandle mapHandle = null;

    public bool hazardsEnabled = true;

    private void OnEnable()
    {
        mapHandle.mapConfiguration.hazards.hazardsEnabled = hazardsEnabled;
    }
}
