using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 euler = new Vector3(0, 10, 0);
    public Space space = Space.Self;

    private void Update()
    {
        Quaternion rot = Quaternion.Euler(euler * Time.deltaTime);

        if (space == Space.Self)
            transform.rotation = transform.rotation * rot;
        else
            transform.rotation = rot * transform.rotation;
    }
}
