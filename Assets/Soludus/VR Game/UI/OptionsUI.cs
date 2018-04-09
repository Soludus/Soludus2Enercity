using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public SettingsManager settings = null;
    public Toggle tutorialsToggle = null;

    private void Awake()
    {
        tutorialsToggle.onValueChanged.AddListener(OnTutorialsToggleChanged);
    }

    private void OnEnable()
    {
        if (settings == null)
            settings = FindObjectOfType<SettingsManager>();

        FetchSettingsToUI(settings);
    }

    private void FetchSettingsToUI(SettingsManager settings)
    {
        tutorialsToggle.isOn = settings.tutorialsEnabled;
    }

    private void OnTutorialsToggleChanged(bool val)
    {
        settings.tutorialsEnabled = val;
        settings.ApplySettings();
    }
}
