using Soludus.Energy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigGUI : MonoBehaviour
{
    public SettingsManager settings = null;
    public LocationHandle locationHandle = null;
    public MapHandle mapHandle = null;
    public GameObject debugView = null;

    private Rect windowRect;

    private void OnEnable()
    {
        if (settings == null)
            settings = FindObjectOfType<SettingsManager>();

        windowRect = new Rect(Screen.width - 250, 200, 240, 220);
    }

    private void OnGUI()
    {
        windowRect = GUI.Window(3, windowRect, DrawConfigGUI, "Config");
    }

    private void DrawConfigGUI(int windowId)
    {
        if (GUILayout.Button("Restart"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        GUILayout.BeginHorizontal();
        if (settings != null)
        {
            GUI.changed = false;
            bool tutorialsEnabled = GUILayout.Toggle(settings.tutorialsEnabled, "Tutorials");
            if (GUI.changed)
            {
                settings.tutorialsEnabled = tutorialsEnabled;
                settings.ApplySettings();
            }
        }
        if (debugView != null)
        {
            GUI.changed = false;
            bool debugViewActive = GUILayout.Toggle(debugView.activeSelf, "Debug View");
            if (GUI.changed)
            {
                debugView.SetActive(debugViewActive);
            }
        }
        GUILayout.EndHorizontal();
        if (mapHandle != null)
        {
            mapHandle.mapConfiguration.hazards.hazardsEnabled = GUILayout.Toggle(mapHandle.mapConfiguration.hazards.hazardsEnabled, "Hazards");
        }

        GUILayout.Label("Timescale:");
        GUILayout.BeginHorizontal();
        GUILayout.Label(locationHandle.timeScale.ToString("0.##"), GUILayout.Width(64));
        float slider = PowLeaveSign(locationHandle.timeScale, 1/5f);
        slider = GUILayout.HorizontalSlider(slider, -Mathf.Pow(10000, 1/5f), Mathf.Pow(10000, 1/5f));
        locationHandle.timeScale = Mathf.Pow(slider, 5f);
        GUILayout.EndHorizontal();

        GUILayout.Label("Time control:");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+1 h"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddHours(1);
        if (GUILayout.Button("+1 d"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddDays(1);
        if (GUILayout.Button("+1 m"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddMonths(1);
        if (GUILayout.Button("+1 y"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddYears(1);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-1 h"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddHours(-1);
        if (GUILayout.Button("-1 d"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddDays(-1);
        if (GUILayout.Button("-1 m"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddMonths(-1);
        if (GUILayout.Button("-1 y"))
            locationHandle.location.dateTimeOffset = locationHandle.location.dateTimeOffset.AddYears(-1);
        GUILayout.EndHorizontal();

        GUI.DragWindow();
    }

    private static float PowLeaveSign(float a, float b)
    {
        float sign = Mathf.Sign(a);
        return sign * Mathf.Pow(Mathf.Abs(a), b);
    }

}
