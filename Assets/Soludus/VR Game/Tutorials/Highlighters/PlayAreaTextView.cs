using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

/// <summary>
/// This object provides a text separately from ToString().
/// </summary>
public interface ITextSource
{
    string GetText();
}

public class PlayAreaTextView : MonoBehaviour
{
    public GameObject textSource = null;
    public GameObject textUIPrefab = null;
    public float distance = 15;
    public float lerp = 5;

    private Transform playArea = null;
    private Transform headset = null;

    private GameObject textUI = null;
    private Text textComponent = null;

    private void Awake()
    {
        textUI = Instantiate(textUIPrefab);
        textComponent = textUI.GetComponentInChildren<Text>();
        textUI.SetActive(false);
    }

    private void OnEnable()
    {
        playArea = VRTK_DeviceFinder.PlayAreaTransform();
        headset = VRTK_DeviceFinder.HeadsetTransform();

        textUI.SetActive(true);
        if (textSource == null)
            textSource = gameObject;
    }

    private void OnDisable()
    {
        if (textUI != null)
            textUI.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(textUI);
    }

    private void LateUpdate()
    {
        // pos and orientation
        var headPos = headset.position;
        var pos = headPos;
        pos.y = playArea.position.y;
        var lookVector = Vector3.ProjectOnPlane(headset.forward, Vector3.up).normalized;
        pos += lookVector * distance;

        textUI.transform.position = Vector3.Lerp(textUI.transform.position, pos, Time.deltaTime * lerp);
        textUI.transform.forward = pos - headPos;
        //textUI.transform.LookAt(headset);

        // text
        var source = textSource.GetComponent<ITextSource>();
        textComponent.text = source != null ? source.GetText() : "";
    }
}
