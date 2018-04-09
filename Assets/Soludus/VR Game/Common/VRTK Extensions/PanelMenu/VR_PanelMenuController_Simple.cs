// using code from VRTK.VRTK_PanelMenuController

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Attach this under the controller and a canvas as a children.
/// </summary>
public class VR_PanelMenuController_Simple : MonoBehaviour
{
    [Tooltip("The GameObject the panel should rotate towards, which is the Camera (eye) by default.")]
    public GameObject rotateTowards;
    [Tooltip("The scale multiplier, which relates to the scale of parent interactable object.")]
    public float zoomScaleMultiplier = 1f;
    public GameObject menu;

    // Relates to scale of canvas on panel items.
    protected const float CanvasScaleSize = 0.001f;

    protected VRTK_ControllerEvents controllerEvents;
    protected GameObject canvasObject;
    protected bool isShown = false;

    /// <summary>
    /// The ToggleMenu method is used to show or hide the menu.
    /// </summary>
    public virtual void ToggleMenu()
    {
        if (isShown)
        {
            HideMenu(true);
        }
        else
        {
            ShowMenu();
        }
    }

    /// <summary>
    /// The ShowMenu method is used to show the menu.
    /// </summary>
    public virtual void ShowMenu()
    {
        if (!isShown)
        {
            isShown = true;
            StopCoroutine("TweenMenuScale");
            if (enabled)
            {
                StartCoroutine("TweenMenuScale", isShown);
            }
        }
    }

    /// <summary>
    /// The HideMenu method is used to hide the menu.
    /// </summary>
    /// <param name="force">If true then the menu is always hidden.</param>
    public virtual void HideMenu(bool force)
    {
        if (isShown && force)
        {
            isShown = false;
            StopCoroutine("TweenMenuScale");
            if (enabled)
            {
                StartCoroutine("TweenMenuScale", isShown);
            }
        }
    }

    /// <summary>
    /// The HideMenuImmediate method is used to immediately hide the menu.
    /// </summary>
    public virtual void HideMenuImmediate()
    {
        if (menu != null && isShown)
        {
            HandlePanelMenuVisibility();
        }
        transform.localScale = Vector3.zero;
        canvasObject.transform.localScale = Vector3.zero;
        isShown = false;
    }

    protected virtual void Awake()
    {
        Initialize();
        VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void Start()
    {
        controllerEvents.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPress);

        canvasObject = gameObject.transform.GetChild(0).gameObject;
        if (canvasObject == null || canvasObject.GetComponent<Canvas>() == null)
        {
            VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuController", "Canvas", "a child"));
        }
    }

    protected virtual void OnDestroy()
    {
        VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
    }

    protected virtual void Update()
    {
        if (rotateTowards == null)
        {
            rotateTowards = VRTK_DeviceFinder.HeadsetTransform().gameObject;
            if (rotateTowards == null)
            {
                VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.COULD_NOT_FIND_OBJECT_FOR_ACTION, "PanelMenuController", "an object", "rotate towards"));
            }
        }

        if (isShown)
        {
            if (rotateTowards != null)
            {
                transform.rotation = Quaternion.LookRotation((rotateTowards.transform.position - transform.position) * -1, Vector3.up);
            }
        }
    }

    protected virtual void Initialize()
    {
        if (Application.isPlaying)
        {
            if (!isShown)
            {
                transform.localScale = Vector3.zero;
            }
        }

        if (controllerEvents == null)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            controllerEvents = GetComponentInParent<VRTK_ControllerEvents>();
        }
    }

    protected virtual void BindControllerEvents()
    {
        //controllerEvents.TouchpadPressed += new ControllerInteractionEventHandler(DoTouchpadPress);
        //controllerEvents.TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouched);
        //controllerEvents.TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadUntouched);
        //controllerEvents.TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
        //controllerEvents.TriggerPressed += new ControllerInteractionEventHandler(DoTriggerPressed);
    }

    protected virtual void UnbindControllerEvents()
    {
        //controllerEvents.TouchpadPressed -= new ControllerInteractionEventHandler(DoTouchpadPress);
        //controllerEvents.TouchpadTouchStart -= new ControllerInteractionEventHandler(DoTouchpadTouched);
        //controllerEvents.TouchpadTouchEnd -= new ControllerInteractionEventHandler(DoTouchpadUntouched);
        //controllerEvents.TouchpadAxisChanged -= new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
        //controllerEvents.TriggerPressed -= new ControllerInteractionEventHandler(DoTriggerPressed);
    }

    protected virtual void HandlePanelMenuVisibility()
    {
        if (isShown)
        {
            menu.SetActive(false);
            HideMenu(true);
        }
        else
        {
            menu.SetActive(true);
            ShowMenu();
        }
    }

    protected virtual IEnumerator TweenMenuScale(bool show)
    {
        float targetScale = 0;
        Vector3 direction = -1 * Vector3.one;
        if (show)
        {
            canvasObject.transform.localScale = new Vector3(CanvasScaleSize, CanvasScaleSize, CanvasScaleSize);
            targetScale = zoomScaleMultiplier;
            direction = Vector3.one;
        }
        int i = 0;
        while (i < 250 && ((show && transform.localScale.x < targetScale) || (!show && transform.localScale.x > targetScale)))
        {
            transform.localScale += direction * Time.deltaTime * 4f * zoomScaleMultiplier;
            yield return true;
            i++;
        }
        transform.localScale = direction * targetScale;
        StopCoroutine("TweenMenuScale");

        if (!show)
        {
            canvasObject.transform.localScale = Vector3.zero;
        }
    }

    protected virtual void DoTouchpadPress(object sender, ControllerInteractionEventArgs e)
    {
        if (menu != null)
        {
            HandlePanelMenuVisibility();
        }
    }

}
