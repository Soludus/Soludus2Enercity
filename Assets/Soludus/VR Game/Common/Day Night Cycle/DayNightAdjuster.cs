using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DayNightAdjuster : MonoBehaviour
{
    [Header("This value is shown for reference. Adjust the transform Directional Light (Sun)")]
    [Range(0, 1)]
    public float m_t = 0;

    [Header("May overwrite values unintentionally")]
    public bool m_runInEditor = false;

    [Header("Remember that changes made in playmode will be lost")]

    [Header("Directional Light (Sun)")]
    public Gradient m_lightColor;
    public AnimationCurve m_lightIntensity = ConstantAnimationCurve(1);
    public AnimationCurve m_lightShadowStrength = ConstantAnimationCurve(1);

    [Header("Procedural Skybox")]
    public AnimationCurve m_sunSize = ConstantAnimationCurve(1);
    public AnimationCurve m_atmosphereThickness = ConstantAnimationCurve(1);
    public Gradient m_skyTint;
    public Gradient m_ground;
    public AnimationCurve m_exposure = ConstantAnimationCurve(1);

    [Header("Lighting")]
    public AnimationCurve m_ambientIntensity = ConstantAnimationCurve(1);
    public AnimationCurve m_reflectionIntensity = ConstantAnimationCurve(1);
    public Gradient m_fogColor;
    public AnimationCurve m_fogDensity = ConstantAnimationCurve(0.01f);

    [Header("Water Material")]
    public Gradient m_baseColor;
    public Gradient m_reflectionColor;
    public Gradient m_specularColor;
    public Gradient m_wavesColor;

    [Header("References")]
    //public Light m_sunLight;
    //public Material m_skyboxMaterial;
    public Material m_waterMaterial;

    [Header("Other")]
    public bool m_setDefault = true;
    public float m_defaultT = 1;

    private static class Uniforms
    {
        public static int _SunSize = Shader.PropertyToID("_SunSize");
        public static int _AtmosphereThickness = Shader.PropertyToID("_AtmosphereThickness");
        public static int _SkyTint = Shader.PropertyToID("_SkyTint");
        public static int _GroundColor = Shader.PropertyToID("_GroundColor");
        public static int _Exposure = Shader.PropertyToID("_Exposure");

        public static int _BaseColor = Shader.PropertyToID("_BaseColor");
        public static int _ReflectionColor = Shader.PropertyToID("_ReflectionColor");
        public static int _SpecularColor = Shader.PropertyToID("_SpecularColor");
        public static int _WavesColor = Shader.PropertyToID("_WavesColor");
    }

    private static Gradient ConstantGradient(Color color)
    {
        return new Gradient
        {
            colorKeys = new GradientColorKey[2]
            {
                new GradientColorKey(color, 0),
                new GradientColorKey(color, 1)
            },
            alphaKeys = new GradientAlphaKey[2]
            {
                new GradientAlphaKey(color.a, 0),
                new GradientAlphaKey(color.a, 1)
            }
        };
    }

    private static AnimationCurve ConstantAnimationCurve(float value)
    {
        return AnimationCurve.Linear(0, value, 1, value);
    }

    private void Reset()
    {
        var sun = RenderSettings.sun;
        var skybox = RenderSettings.skybox;
        var water = m_waterMaterial;

        if (sun != null)
        {
            m_lightColor = ConstantGradient(sun.color);
            m_lightIntensity = ConstantAnimationCurve(sun.intensity);
            m_lightShadowStrength = ConstantAnimationCurve(sun.shadowStrength);
        }

        if (skybox != null)
        {
            m_sunSize = ConstantAnimationCurve(skybox.GetFloat(Uniforms._SunSize));
            m_atmosphereThickness = ConstantAnimationCurve(skybox.GetFloat(Uniforms._AtmosphereThickness));
            m_skyTint = ConstantGradient(skybox.GetColor(Uniforms._SkyTint));
            m_ground = ConstantGradient(skybox.GetColor(Uniforms._GroundColor));
            m_exposure = ConstantAnimationCurve(skybox.GetFloat(Uniforms._Exposure));
        }

        m_reflectionIntensity = ConstantAnimationCurve(RenderSettings.reflectionIntensity);
        m_ambientIntensity = ConstantAnimationCurve(RenderSettings.ambientIntensity);
        m_fogColor = ConstantGradient(RenderSettings.fogColor);
        m_fogDensity = ConstantAnimationCurve(RenderSettings.fogDensity);

        if (water != null)
        {
            m_baseColor = ConstantGradient(water.GetColor(Uniforms._BaseColor));
            m_reflectionColor = ConstantGradient(water.GetColor(Uniforms._ReflectionColor));
            m_specularColor = ConstantGradient(water.GetColor(Uniforms._SpecularColor));
            m_wavesColor = ConstantGradient(water.GetColor(Uniforms._WavesColor));
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying && !m_runInEditor)
            return;

        if (m_setDefault)
            SetDefaults();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying && !m_runInEditor)
            return;

        if (m_setDefault)
            SetDefaults();
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying && !m_runInEditor)
            return;

        var sun = RenderSettings.sun;
        var skybox = RenderSettings.skybox;
        var water = m_waterMaterial;

        m_t = CalculateTFromSun(sun.transform);
        UpdateTargetProperties(m_t, sun, skybox, water);
    }

    private void SetDefaults()
    {
        UpdateTargetProperties(m_defaultT, RenderSettings.sun, RenderSettings.skybox, m_waterMaterial);
    }

    private float CalculateTFromSun(Transform sun)
    {
        float dot = Vector3.Dot(sun.forward, Vector3.up);
        dot = Mathf.Clamp(dot, -1, 1);

        // t moves along sine wave
        //float t = 0.5f - 0.5f * dot; 

        // t moves along triangle wave
        float t = Mathf.Acos(dot) / Mathf.PI;

        return t;
    }

    private void UpdateTargetProperties(float t, Light sun, Material skybox, Material water)
    {
        if (sun != null)
        {
            sun.color = m_lightColor.Evaluate(t);
            sun.intensity = m_lightIntensity.Evaluate(t);
            sun.shadowStrength = Mathf.Clamp01(m_lightShadowStrength.Evaluate(t));
        }

        if (skybox != null)
        {
            skybox.SetFloat(Uniforms._SunSize, m_sunSize.Evaluate(t));
            skybox.SetFloat(Uniforms._AtmosphereThickness, m_atmosphereThickness.Evaluate(t));
            skybox.SetColor(Uniforms._SkyTint, m_skyTint.Evaluate(t));
            skybox.SetColor(Uniforms._GroundColor, m_ground.Evaluate(t));
            skybox.SetFloat(Uniforms._Exposure, m_exposure.Evaluate(t));
        }

        RenderSettings.reflectionIntensity = m_reflectionIntensity.Evaluate(t);
        RenderSettings.ambientIntensity = m_ambientIntensity.Evaluate(t);
        RenderSettings.fogColor = m_fogColor.Evaluate(t);
        RenderSettings.fogDensity = m_fogDensity.Evaluate(t);

        if (water != null)
        {
            water.SetColor(Uniforms._BaseColor, m_baseColor.Evaluate(t));
            water.SetColor(Uniforms._ReflectionColor, m_reflectionColor.Evaluate(t));
            water.SetColor(Uniforms._SpecularColor, m_specularColor.Evaluate(t));
            water.SetColor(Uniforms._WavesColor, m_wavesColor.Evaluate(t));
        }
    }
}
