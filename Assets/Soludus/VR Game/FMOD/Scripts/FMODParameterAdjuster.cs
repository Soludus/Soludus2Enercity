using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soludus;
#if !DISABLE_FMOD
using FMODUnity;

public class FMODParameterAdjuster : MonoBehaviour
{
    public StudioEventEmitter target = null;
    public string parameterName = "param1";
    [ReadOnly]
    public float currentValue = 0;

    [System.Serializable]
    public class State
    {
        public float speed = 0f;
        public float duration = float.PositiveInfinity;
    }

    public List<State> states = new List<State>();
    public int defaultState = 0;
    public int currentState = 0;

    public float clampMin = 0;
    public float clampMax = 1;

    private float setStateTime = float.NegativeInfinity;

    private void OnEnable()
    {
        if (target == null)
            target = GetComponent<StudioEventEmitter>();
    }

    private void Update()
    {
        if (Time.time > setStateTime + states[currentState].duration)
            currentState = defaultState;

        currentValue = target.GetParameter(parameterName);
        currentValue += states[currentState].speed * Time.deltaTime;

        currentValue = Mathf.Clamp(currentValue, clampMin, clampMax);
        target.SetParameter(parameterName, currentValue);
    }

    public void SetState(int state)
    {
        currentState = state;
        setStateTime = Time.time;
    }
}
#endif
