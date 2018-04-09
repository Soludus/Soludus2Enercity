using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GrabObjective : Objective
{
    public VRTK_PolicyList targetPolicy = null;

    public bool useRightHand = true;
    public bool useLeftHand = true;

    public float failIfTimeElapsedAfterRelease = 5;
    public bool failIfLastGrabbedIsDestroyed = true;

    private GameObject grabbed = null;
    private GameObject lastGrabbed = null;
    private float releaseTime = 0;

    private GameObject GetGrabbedObject(GameObject hand)
    {
        if (hand != null)
        {
            var grab = hand.GetComponent<VRTK_InteractGrab>();
            if (grab != null)
            {
                var grabbed = grab.GetGrabbedObject();
                if (grabbed != null && targetPolicy.Find(grabbed))
                    return grabbed;
            }
        }
        return null;
    }

    public override bool IsComplete()
    {
        GameObject grabbed = null;
        if (useRightHand)
            grabbed = GetGrabbedObject(VRTK_DeviceFinder.GetControllerRightHand());
        if (useLeftHand && grabbed == null)
            grabbed = GetGrabbedObject(VRTK_DeviceFinder.GetControllerLeftHand());

        if (this.grabbed != null && grabbed == null)
        {
            releaseTime = Time.time;
        }

        this.grabbed = grabbed;
        if (this.grabbed != null)
            lastGrabbed = this.grabbed;

        if (this.grabbed != null)
            return true;

        if (failIfLastGrabbedIsDestroyed && lastGrabbed == null)
            return false;

        return Time.time < releaseTime + failIfTimeElapsedAfterRelease;
    }
}
