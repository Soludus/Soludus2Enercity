using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Uses a procedural skybox to create a day night cycle with a visible sun and moon.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    [Header("Light")]
    [SerializeField]
    private Light m_directionalLight;
    [SerializeField]
    private Vector3 m_lightAngles = new Vector3(0, -30, 0);

    [Range(0.5f, 1)]
    public float nightThreshold = 0.54f;
    [Range(0, 2)]
    public float moonAngleOffset = 0.25f;

    public Gradient lightColor = new Gradient
    {
        colorKeys = new[] {
            new GradientColorKey(new Color(1.0f, 1.0f, 0.75f), 0.5f),
            new GradientColorKey(new Color(0.75f, 0.75f, 1.0f), 0.55f),
        }
    };

    [Header("Sky")]
    [SerializeField]
    private Material m_proceduralSkybox;

    public AnimationCurve atmosphereThickness = new AnimationCurve(
        new Keyframe(0.0f, 1.0f),
        new Keyframe(0.5f, 1.0f),
        new Keyframe(0.55f, 0.27f),
        new Keyframe(1.0f, 0.27f)
        );
    public AnimationCurve exposure = new AnimationCurve(
        new Keyframe(0.0f, 1.3f),
        new Keyframe(0.5f, 1.3f),
        new Keyframe(0.55f, 0.42f),
        new Keyframe(1.0f, 0.42f)
        );

    [Header("Time")]
    [SerializeField]
    private float m_time = 0.5f;
    private bool m_night;

    /// <summary>
    /// 1.0 is realtime.
    /// You can override this by setting it to 0 and updating the DayNightCycle.time.
    /// </summary>
    public float timeScale = 1000;

    [Header("Other")]
    [Range(-1, 1)]
    public float evaluationOffset = 0;
    public ReflectionProbe reflectionProbe;

    /// <summary>
    /// Get or set normalized time. 1 unit is 24 hours. 0 is midnight.
    /// </summary>
    public float time
    {
        get { return m_time; }
        set
        {
            m_time = value;
            FindLights();
            UpdateCycle();
            //DynamicGI.UpdateEnvironment();
            if (reflectionProbe != null)
                reflectionProbe.RenderProbe();
        }
    }

    public bool isNight
    {
        get { return m_night; }
    }

    private void OnValidate()
    {
        UpdateCycle();
    }

    private void Update()
    {
        if (timeScale != 0)
            time += Time.deltaTime * timeScale / (24 * 60 * 60);
    }

    private void UpdateCycle()
    {
        if (m_directionalLight != null)
        {
            // wrap (0..1)
            float t = (m_time - 0.5f) % 1.0f;
            if (t < 0)
                t = t + 1;

            var angle = t * 360;

            // peak at 0.5
            if (t > 0.5f)
                t = (1 - t);
            t *= 2;

            // is night
            m_night = t > nightThreshold;

            // light angle (sun/moon position)
            if (m_night)
                angle = (-90 + angle) * (1 + moonAngleOffset) - 90 * moonAngleOffset;
            else
                angle = 90 + angle;

            m_lightAngles.x = angle;
            m_directionalLight.transform.eulerAngles = m_lightAngles;

            t += evaluationOffset;

            // light color
            m_directionalLight.color = lightColor.Evaluate(t);

            // procedural skybox
            var at = atmosphereThickness.Evaluate(t);
            var exp = exposure.Evaluate(t);
            m_proceduralSkybox.SetFloat("_AtmosphereThickness", at);
            m_proceduralSkybox.SetFloat("_Exposure", exp);
        }
    }

    private void FindLights()
    {
        if (m_directionalLight == null)
            m_directionalLight = RenderSettings.sun;
        if (m_directionalLight == null || m_directionalLight.type != LightType.Directional)
            Debug.LogWarning("DayNightCycle needs a directional light.");
    }

}
