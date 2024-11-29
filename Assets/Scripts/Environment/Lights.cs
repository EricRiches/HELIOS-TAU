using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour
{
    Light lightSource;

    float maxInterval = 0.2f;
    float maxFlicker = 0.2f;
    float defaultIntensity;
    float minIntensity = 0.6f;
    float timer;
    float delay;

    bool isOn = true;


    bool isMonsterWithinArea = false;

    // Sets the default light intensity to the current intensity
    private void Start()
    {
        lightSource = GetComponent<Light>();

        defaultIntensity = lightSource.intensity;
    }

    // If the monster is within range this starts the flickering
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            isMonsterWithinArea = true;
        Debug.Log("Is Trigger Enter");
        }
    }

    // Resets the light intensity to default when the monster leaves the range
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            isOn = true;
            lightSource.intensity = defaultIntensity;
            isMonsterWithinArea = false;
        }
    }

    // Adjusts the light intensity
    void Flicker()
    {
        isOn = !isOn;

        if (isOn)
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, defaultIntensity, timer / delay);
            delay = Random.Range(0, maxInterval);
        }
        else
        {
            lightSource.intensity = Mathf.Lerp(lightSource.intensity, Random.Range(minIntensity, defaultIntensity), timer / delay);
            delay = Random.Range(0, maxFlicker);
        }

        timer = 0;
    }

    private void Update()
    {
        if (isMonsterWithinArea)
        {
            timer += Time.deltaTime;

            if (timer > delay)
            {
                Flicker();
            }
        }
    }
}
