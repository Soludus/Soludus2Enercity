using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component tracks the amount of sun a building receives and manages indicators. Handles the users click (OnUse) to activate or reset power.
/// </summary>
public class SolarPanel : MonoBehaviour
{
    [SerializeField]
    private Material m_inactiveMaterial = null;
    [SerializeField]
    private Material m_normalMaterial = null;
    [SerializeField]
    private Material m_shadowedMaterial = null;

    [SerializeField]
    private ShadowDetectorBase m_shadowDetector = null;
    [SerializeField]
    private SolarPowerGameManager m_gameManager = null;
    [SerializeField]
    private float m_powerPerSec = 1;
    [SerializeField]
    private float m_reducePerSec = 4;
    [SerializeField]
    private float m_maxPower = 10;

    [SerializeField]
    private FloatValue m_powerIndicator = null;
    [SerializeField]
    private Animator m_powerIndicatorAnim = null;

    private static int indicatorFlashParamId = Animator.StringToHash("Flash");

    private Renderer m_renderer = null;

    private bool m_inShadow;
    private float m_power;
    private bool m_emptying;

    public float maxPower
    {
        get
        {
            return m_maxPower;
        }

        set
        {
            m_maxPower = value;
        }
    }

    public float power
    {
        get
        {
            return m_power;
        }

        set
        {
            m_power = value;
        }
    }

    private void Reset()
    {
        FindDependencies();
    }

    private void Awake()
    {
        FindDependencies();
        SetMaterial(m_inactiveMaterial);
        m_gameManager.RegisterPanel(this);
    }

    private void OnDestroy()
    {
        m_gameManager.UnregisterPanel(this);
    }

    private void OnEnable()
    {
        FindDependencies();

        if (m_inShadow == true)
            OnEnterShadow();
        else
            OnExitShadow();

        m_gameManager.OnActivatePanel(this, true);
        m_shadowDetector.enabled = true;
        m_powerIndicator.gameObject.SetActive(true);
        UpdateNormalizedPower();
    }

    private void OnDisable()
    {
        m_powerIndicator.gameObject.SetActive(false);
        m_shadowDetector.enabled = false;
        SetMaterial(m_inactiveMaterial);
        m_gameManager.OnActivatePanel(this, false);
    }

    private void Update()
    {
        var inShadow = m_shadowDetector.percentageInShadow > 0.33f;
        if (inShadow != m_inShadow)
        {
            if (inShadow == true)
                OnEnterShadow();
            else
                OnExitShadow();
        }

        if (m_emptying)
        {
            // collecting points (reducing charge)
            var delta = -m_reducePerSec * Time.deltaTime;
            if (m_power + delta <= 0)
            {
                delta = -m_power;
                m_emptying = false;
                enabled = false;
            }
            m_power += delta;
            m_gameManager.score += -delta;
            UpdateNormalizedPower();
        }
        else if (m_inShadow == false)
        {
            // generating power
            if (m_power < m_maxPower)
            {
                m_power += m_powerPerSec * Time.deltaTime;
                if (m_power >= m_maxPower)
                {
                    m_power = m_maxPower;
                    Debug.Log(this + " power is full!");
                }

                UpdateNormalizedPower();
            }
        }

        m_powerIndicatorAnim.SetBool(indicatorFlashParamId, m_power >= m_maxPower);
    }

    private void FindDependencies()
    {
        if (m_gameManager == null)
            m_gameManager = FindObjectOfType<SolarPowerGameManager>();
        if (m_shadowDetector == null)
            m_shadowDetector = GetComponentInChildren<ShadowDetectorBase>();
    }

    private void UpdateNormalizedPower()
    {
        m_powerIndicator.value = m_power / m_maxPower;
    }

    private void SetMaterial(Material mat)
    {
        if (m_renderer == null)
            m_renderer = GetComponentInChildren<Renderer>();

        if (m_renderer.sharedMaterial != mat)
        {
            m_renderer.sharedMaterial = mat;

            var vrtk_highlighter = m_renderer.GetComponentInParent<VRTK.Highlighters.VRTK_BaseHighlighter>();
            if (vrtk_highlighter != null)
                vrtk_highlighter.ResetHighlighter();
        }
    }

    public void OnUse()
    {
        if (enabled == false)
        {
            if (m_gameManager.activePanelCount < m_gameManager.maxActivePanels)
                enabled = true;
            m_gameManager.OnTryActivatePanel();
        }
        else
        {
            m_emptying = !m_emptying;
            Debug.Log("Collecting " + m_power + " points.");
        }
    }

    public void OnEnterShadow()
    {
        m_inShadow = true;
        if (enabled)
        {
            SetMaterial(m_shadowedMaterial);
            Debug.Log(this + " is now in shadow!");
        }
    }

    public void OnExitShadow()
    {
        m_inShadow = false;
        if (enabled)
        {
            SetMaterial(m_normalMaterial);
            Debug.Log(this + " is now generating power.");
        }
    }
}
