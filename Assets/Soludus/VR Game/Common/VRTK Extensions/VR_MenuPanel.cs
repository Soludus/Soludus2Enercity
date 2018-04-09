using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class VR_MenuPanel : MonoBehaviour
{
    public float distance = 10;
    public float yOffset = -2;
    public GameObject panel = null;

    private void OnEnable()
    {
        var headset = VRTK_DeviceFinder.HeadsetTransform();

        // pos and orientation
        var headPos = headset.position;
        var pos = headPos;
        pos.y += yOffset;
        var lookVector = Vector3.ProjectOnPlane(headset.forward, Vector3.up).normalized;
        pos += lookVector * distance;
        panel.transform.position = pos;
        panel.transform.forward = pos - headPos;

        panel.SetActive(true);
    }

    private void OnDisable()
    {
        panel.SetActive(false);
    }
}
