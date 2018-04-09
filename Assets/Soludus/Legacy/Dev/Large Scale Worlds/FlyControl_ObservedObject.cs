using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyControl_ObservedObject : MonoBehaviour
{
    [SerializeField]
    private ObservedObject observedObject;

    [SerializeField]
    private float speedMultiplier = 1;

    [SerializeField]
    private float runMultiplier = 5;

    [SerializeField]
    private float altitudeMultiplier = 1;

    private void Update()
    {
        //Debug.DrawRay(transform.position, observedObject.WorldToObservedDirection(transform.forward) * 100, Color.blue);
        //Debug.DrawRay(transform.position, observedObject.WorldToObservedDirection(transform.up) * 100, Color.green);
        //Debug.DrawRay(transform.position, observedObject.WorldToObservedDirection(transform.right) * 100, Color.red);

        float forward = Input.GetAxis("Vertical");
        float right = Input.GetAxis("Horizontal");

        float run = 1 + (runMultiplier - 1) * Input.GetAxis("Fire3");

        if (forward != 0 || right != 0)
        {
            Vector3 input = speedMultiplier * run * Time.deltaTime * (forward * transform.forward + right * transform.right);

            input *= 1 + (observedObject.observerAltitude) * altitudeMultiplier;

            observedObject.observerPosition += observedObject.WorldToObservedDirection(input);
        }
    }
}
