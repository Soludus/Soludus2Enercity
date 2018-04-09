using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float lookSensitivity = 1;

    private Vector2 targetRot;

    private void Update()
    {
        var deltaTime = Time.deltaTime;

        Vector2 rotation = new Vector2(
            Input.GetAxis("Mouse X") * lookSensitivity,
            Input.GetAxis("Mouse Y") * lookSensitivity);
        targetRot += rotation;

        var rot = targetRot;
        transform.localRotation = Quaternion.AngleAxis(rot.x, Vector3.up) * Quaternion.AngleAxis(rot.y, Vector3.left);
    }
}
