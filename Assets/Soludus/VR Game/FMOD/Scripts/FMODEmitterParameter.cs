using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Soludus;
#if !DISABLE_FMOD
using FMODUnity;

public sealed class FMODEmitterParameter : MonoBehaviour
{
    public StudioEventEmitter target = null;
    public string parameterName = "param1";
    [ReadOnly]
    public float currentValue = 0;
    [ReadOnly]
    public float rawValue = 0;

    public Object valueSource = null;

    [System.Serializable]
    public class ValueParams
    {
        public float scale = 1;
        public float bias = 0;

        [Header("Clamp")]
        public float clampMin = 0;
        public float clampMax = 1;

        [Header("Other")]
        public float smoothing = 0;
        public bool useCurve = false;
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    }

    public ValueParams adjust = new ValueParams();

    public bool playOnlyWhenNonZero = false;

    public enum UpdateType
    {
        Update,
        LateUpdate,
        FixedUpdate
    }

    public UpdateType update = UpdateType.Update;
    public int startDelay = 3;

    private int updateCount = 0;

    private void OnEnable()
    {
        if (target == null)
            target = GetComponentInParent<StudioEventEmitter>();

        updateCount = 0;
        currentValue = 0;
    }

    private void Update()
    {
        if (update == UpdateType.Update)
        {
            InternalUpdate();
        }
    }

    private void LateUpdate()
    {
        if (update == UpdateType.LateUpdate)
        {
            InternalUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (update == UpdateType.FixedUpdate)
        {
            InternalUpdate();
        }
    }

    private void InternalUpdate()
    {
        ++updateCount;
        if (updateCount < startDelay)
            return;

        var source = SourceProvider<float>.GetValueProvider(valueSource, this);
        if (source == null)
            return;

        if (updateCount == startDelay)
            source.InitValue();

        rawValue = source.GetValue();
        float targetValue = rawValue * adjust.scale + adjust.bias;
        targetValue = Mathf.Clamp(targetValue, adjust.clampMin, adjust.clampMax);

        if (adjust.useCurve)
        {
            targetValue = adjust.curve.Evaluate(targetValue);
        }

        if (!string.IsNullOrEmpty(parameterName))
            currentValue = target.GetParameter(parameterName);
        currentValue = adjust.smoothing > 0 ? Mathf.MoveTowards(currentValue, targetValue, Time.deltaTime / adjust.smoothing) : targetValue;

        if (target != null)
        {
            if (!string.IsNullOrEmpty(parameterName))
                target.SetParameter(parameterName, currentValue);

            if (playOnlyWhenNonZero)
            {
                if (target.IsPlaying())
                {
                    if (currentValue == 0)
                        target.Stop();
                }
                else
                {
                    if (currentValue != 0)
                        target.Play();
                }
            }
        }
    }

    public void SetBias(float bias)
    {
        adjust.bias = bias;
    }

    public void SetClampMin(float min)
    {
        adjust.clampMin = min;
    }

    public void SetClampMax(float max)
    {
        adjust.clampMax = max;
    }
}
#endif