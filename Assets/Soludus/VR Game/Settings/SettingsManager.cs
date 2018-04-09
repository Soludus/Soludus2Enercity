using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private const string tutorialsEnabledId = "tutorialsEnabled";

    public bool applyOnEnable = true;

    [Header("References")]
    public GameObject tutorials = null;

    private void OnEnable()
    {
        if (applyOnEnable)
            ApplySettings();
    }

    public void ApplySettings()
    {
        tutorials.SetActive(tutorialsEnabled);
    }

    public bool tutorialsEnabled
    {
        get
        {
            return PlayerPrefs.GetInt(tutorialsEnabledId, 1) != 0;
        }
        set
        {
            PlayerPrefs.SetInt(tutorialsEnabledId, value ? 1 : 0);
        }
    }
}
