using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformConditional_UnityEvents : MonoBehaviour
{
    [System.Serializable]
    public class Platforms
    {
        public bool editor = true;
        public bool build = true;
        public bool mobile = true;
        public bool console = true;
        public bool other = true;

        public bool IsValid()
        {
            if (Application.isEditor)
                return editor;

            if (!build)
                return false;

            if (Application.isMobilePlatform)
                return mobile;

            if (Application.isConsolePlatform)
                return console;

            return other;
        }
    }

    [System.Serializable]
    public class UnityEvents
    {
        public UnityEvent onTrue = new UnityEvent();
        public UnityEvent onFalse = new UnityEvent();
    }

    public Platforms platforms = new Platforms();
    public UnityEvents events = new UnityEvents();

    private void Awake()
    {
        if (platforms.IsValid())
            events.onTrue.Invoke();
        else
            events.onFalse.Invoke();
    }
}
