using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoLight : MonoBehaviour {

    MeshRenderer mr;

    bool lightSwitchingOn;
    public float lightSwitchCooldown;
    float lastSwitchTime;
    float lightSpot;

	// Use this for initialization
	void Start () {
        mr = GetComponent<MeshRenderer>();
        lightSwitchingOn = true;
        lightSpot = 0;
    }
	
	// Update is called once per frame
	void Update () {
		
        if (Time.time > lastSwitchTime + lightSwitchCooldown)
        {
            lastSwitchTime = Time.time;
            ChangeLightOffset();
        }
	}

    void ChangeLightOffset()
    {
        if (lightSwitchingOn)
        {
            if (lightSpot < 0.9f)
            {
                lightSpot += 0.1f;
            }
            else
            {
                lightSpot = 0;
            }

            mr.material.mainTextureOffset = new Vector2(lightSpot, 0f);
        }
    }
}
