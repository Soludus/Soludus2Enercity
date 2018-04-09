using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputKey_UnityEvents : MonoBehaviour
{
    public KeyCode key = KeyCode.None;
    public UnityEvent onKeyDown = new UnityEvent();
    public UnityEvent onKeyUp = new UnityEvent();

    private void Update()
    {
        if (Input.GetKeyDown(key))
            onKeyDown.Invoke();
        if (Input.GetKeyUp(key))
            onKeyUp.Invoke();
    }
}
