using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool isOn = true;
    private bool isOnCheck = true;
    public Transform MonsterStandPlace;

    private void Update()
    {
        if (isOn != isOnCheck)
        {
            TurnOnOff();
        }
    }

    // Toggles whether the children's light components are on or off
    public void TurnOnOff()
    {
        isOn = isOnCheck;

        for (int i = 0; i < transform.childCount; i++)
        {
            Light light = transform.GetChild(i).gameObject.GetComponentInChildren<Light>();

            if (light != null)
            {
                light.enabled = !isOn;
            }
        }

        isOn = !isOn;
        isOnCheck = isOn;
    }

    public bool GetIsOn()
    {
        return isOn;
    }
}
