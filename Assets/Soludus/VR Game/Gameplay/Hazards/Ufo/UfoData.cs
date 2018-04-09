using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoData : MonoBehaviour
{
    /// <summary>
    /// Describes what the ufo is currently doing.
    /// </summary>
    public string state;

    /// <summary>
    /// The constant movement speed of the ufo.
    /// </summary>
    public float moveSpeed;

    public float throwSpeedTreshold;

    public float throwRotationTreshold;

    /// <summary>
    /// The lowest altitude from which the ufo searches for new solar panel. This should be higher than any obstacle in the scene.
    /// </summary>
    public float objectSearchHeightMin;
    /// <summary>
    /// The highest altitude from which the ufo searches for new solar panel
    /// </summary>
    public float objectSearchHeightMax;
    /// <summary>
    /// The minimum times the ufo wanders around before seeking for solar panel.
    /// </summary>
    public int minTimesWanderingAtStart;
    /// <summary>
    /// The maximal times the ufo wanders around before seeking for solar panel.
    /// </summary>
    public int maxTimesWanderingAtStart;
    /// <summary>
    /// The height above solar panel where the ufo will hover.
    /// </summary>
    public float heightAboveDestination;
    /// <summary>
    /// The maximum height of the ufo's hovering. The minimum height is the ufo's height above destination value.
    /// </summary>
    public float hoverMaxHeight;
    /// <summary>
    /// The speed of the hovering above solar panel.
    /// </summary>
    public float hoverSpeed;
    /// <summary>
    /// The time the ufo spends draining energy from current target solar panel. Later this should be set by the amount of energy the solar panel has left.
    /// </summary>
    public float timeDrainingEnergy;
    /// <summary>
    /// The delay between certain state transitions.
    /// </summary>
    public float stateTransitionDelay;
    /// <summary>
    /// The rotation speed of the disk part of the placeholder model.
    /// </summary>
    public float bodyRotateSpeed;

    public float maxRotateAngle;

    public float swayingSpeed;
}
