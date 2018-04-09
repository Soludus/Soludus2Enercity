using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the collection of solar power points and starting and stopping rounds.
/// </summary>
public class SolarPowerGameManager : MonoBehaviour
{
    [SerializeField]
    private int m_maxActivePanels = 5;
    [SerializeField]
    private HazardManager m_hazardManager;
    [SerializeField]
    private VR_StartPoint m_startPoint;
    [SerializeField]
    private VR_StartPoint m_menuPoint;
    [SerializeField]
    private Canvas m_menuCanvas;
    [SerializeField]
    private Canvas m_endCanvas;
    [SerializeField]
    private FormatText m_endScoreText;

    [Header("Day")]
    [SerializeField]
    private DayNightCycle m_dayNightCycle = null;
    [SerializeField]
    private float m_startIngameHour = 5;
    [SerializeField]
    private float m_roundDurationIngameHours = 12;
    [SerializeField]
    private float m_roundDurationRealSeconds = 144;

    [Header("HUD")]
    [SerializeField]
    private VR_HUDCanvas m_hudCanvas = null;
    [SerializeField]
    private Text m_activePanelsText = null;
    [SerializeField]
    private Text m_scoreText = null;

    [Header("Controllers")]
    [SerializeField]
    private VR_ControllerModes m_leftController;
    [SerializeField]
    private VR_ControllerModes m_rightController;

    private bool m_playing = false;
    private float m_stopRoundTime = -1;
    private int m_activePanelCount = 0;
    private float m_score = 0;

    private List<SolarPanel> m_panels = new List<SolarPanel>();

    /// <summary>
    /// Return all panels, active panels are sorted first.
    /// </summary>
    public IEnumerable<SolarPanel> panels
    {
        get
        {
            m_panels.Sort((p2, p1) => p1.isActiveAndEnabled.CompareTo(p2.isActiveAndEnabled));
            foreach (var item in m_panels)
                yield return item;
        }
    }

    public int activePanelCount
    {
        get { return m_activePanelCount; }
        private set
        {
            m_activePanelCount = value;
            if (m_activePanelsText != null)
                m_activePanelsText.text = value + "/" + maxActivePanels;
        }
    }

    public float score
    {
        get { return m_score; }
        set
        {
            m_score = value;
            if (m_scoreText != null)
                m_scoreText.text = value.ToString("0.00");
        }
    }

    public int maxActivePanels
    {
        get { return m_maxActivePanels; }
        set
        {
            m_maxActivePanels = value;
        }
    }

    public void RegisterPanel(SolarPanel p)
    {
        m_panels.Add(p);
    }

    public void UnregisterPanel(SolarPanel p)
    {
        m_panels.Remove(p);
    }

    public void OnActivatePanel(SolarPanel p, bool active)
    {
        if (active)
            activePanelCount++;
        else
            activePanelCount--;
    }

    public void OnTryActivatePanel()
    {
        if (activePanelCount >= maxActivePanels)
        {
            m_activePanelsText.GetComponent<Animation>().Play();
        }
    }

    private void Awake()
    {
        score = 0;
        activePanelCount = 0;
    }

    private void Update()
    {
        if (m_playing)
        {
            if (Time.time >= m_stopRoundTime)
                StopRound();
        }
    }

    public void StartRound()
    {
        Debug.Log("Start round");

        m_hudCanvas.gameObject.SetActive(true);
        m_menuCanvas.gameObject.SetActive(false);
        if (m_startPoint != null)
            m_startPoint.MovePlayAreaToPoint();
        m_rightController.SetMode(1);
        m_leftController.SetMode(1);

        m_stopRoundTime = Time.time + m_roundDurationRealSeconds;
        m_dayNightCycle.time = m_startIngameHour / 24;
        m_dayNightCycle.timeScale = (m_roundDurationIngameHours * 60 * 60) / m_roundDurationRealSeconds;

        m_hazardManager.enabled = true;
        m_playing = true;
    }

    public void StopRound()
    {
        Debug.Log("Stop round");

        m_playing = false;
        m_hazardManager.enabled = false;

        m_dayNightCycle.timeScale = 0;

        m_endScoreText.SetArgument(0, m_score);
        m_hudCanvas.gameObject.SetActive(false);
        m_endCanvas.gameObject.SetActive(true);
        m_menuPoint.MovePlayAreaToPoint();
        m_rightController.SetMode(0);
        m_leftController.SetMode(0);
    }
}
