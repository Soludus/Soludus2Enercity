using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageData : MonoBehaviour
{

    public string state;

    public float movementSpeed;

    public float grabbedRotationSpeed;

    public float distanceBeforeEnablingSuck;

    public float releaseVelocityMultiplier;

    public float maxPullSpeed;

    public float grabPointDistance;

    public float shrinkSpeed;

    public float initialGarbageAmount;

    public float garbageAmount;

    public ParticleSystem pullParticles;
}
