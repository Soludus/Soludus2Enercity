using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VRTK;

public class VR_Splash : MonoBehaviour
{
    [System.Serializable]
    public class SplashLogo
    {
        public Sprite sprite = null;
        [Range(0, 60)]
        public float duration = 2.0f;
        public float fadeIn = 0.5f;
        public float fadeOut = 0.5f;
        public Color color = Color.white;
        public float scale = 1.0f;
    }

    [Header("References")]
    public SpriteRenderer spriteRend = null;

    [Header("Event")]
    public UnityEvent onFinished = new UnityEvent();

    [Header("Logos")]
    public float distance = 10;
    public bool includeFadesInDuration = true;
    public List<SplashLogo> logos = new List<SplashLogo>();

    public void Begin()
    {
        StartCoroutine(Overlay());
    }

    public void Stop()
    {
        StopAllCoroutines();
        spriteRend.sprite = null;
    }

    private IEnumerator Overlay()
    {
        yield return null;

#if VRTK_DEFINE_SDK_STEAMVR
        if (VRTK_SDK_Bridge.GetSystemSDK() is SDK_SteamVRSystem)
        {
            // Wait until we have tracking.
            var hmd = SteamVR_Controller.Input((int)Valve.VR.OpenVR.k_unTrackedDeviceIndex_Hmd);
            while (!hmd.hasTracking)
                yield return null;
        }
#endif

        yield return new WaitForSecondsRealtime(0.5f);

        Transform headset = null;
        do
        {
            yield return null;
            headset = VRTK_DeviceFinder.HeadsetTransform();
        }
        while (headset == null);

        // pos and orientation
        var headPos = headset.position;
        var pos = headPos;
        var lookVector = Vector3.ProjectOnPlane(headset.forward, Vector3.up).normalized;
        pos += lookVector * distance;
        transform.position = pos;
        transform.forward = pos - headPos;

        // logos
        for (int i = 0; i < logos.Count; ++i)
        {
            spriteRend.sprite = logos[i].sprite;
            Vector3 rendScale = spriteRend.transform.localScale;
            spriteRend.transform.localScale = logos[i].scale * rendScale;

            float alpha = 0;
            float fadeStart = Time.time;
            while (alpha < 1)
            {
                alpha = 1 - GetFadeAlpha(logos[i].fadeIn, fadeStart, Time.time);
                spriteRend.color = logos[i].color * new Color(1, 1, 1, alpha);
                yield return null;
            }

            float dur = logos[i].duration;
            if (includeFadesInDuration)
                dur -= logos[i].fadeIn + logos[i].fadeOut;
            dur = Mathf.Max(0, dur);
            yield return new WaitForSeconds(dur);

            fadeStart = Time.time;
            while (alpha > 0)
            {
                alpha = GetFadeAlpha(logos[i].fadeOut, fadeStart, Time.time);
                spriteRend.color = logos[i].color * new Color(1, 1, 1, alpha);
                yield return null;
            }

            spriteRend.transform.localScale = rendScale;
        }

        onFinished.Invoke();
    }

    private float GetFadeAlpha(float fadeDuration, float fadeStartT, float currentT)
    {
        return Mathf.Clamp01(fadeDuration > 0 ? (fadeStartT + fadeDuration - currentT) / fadeDuration : 0);
    }

}
