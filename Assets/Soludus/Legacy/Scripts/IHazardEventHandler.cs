using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHazardEventHandler
{
    void OnHazardReceived(Hazard hazard);
    void OnHazardRemoved(Hazard hazard);
}
