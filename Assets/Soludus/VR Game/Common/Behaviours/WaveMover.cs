using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMover : MonoBehaviour
{
    public float speed = 1;
    public float amplitude = 1;

    private float m_position;

    private void Update()
    {
        var newPos = Mathf.Sin(Time.time * speed);
        var deltaPos = newPos - m_position;
        m_position = newPos;

        transform.Translate(0, 0, deltaPos * amplitude);
    }
}
