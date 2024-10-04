using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Random_On : MonoBehaviour
{
    [SerializeField] List<Generator> generators;

    int genOn;
    bool activated = false;

    // Looks at the length of the generators list and determines how many will need to be turned off
    void Start()
    {
        if (generators != null)
        {
            genOn = generators.Count;

            PickGen(genOn);
        }
    }

    private void Update()
    {
        int genCount = 0;

        for (int i = 0; i < generators.Count; i++)
        {
            if (generators[i] != null && generators[i].GetIsOn())
            {
                genCount++;
            }
            if (genCount == generators.Count)
            {
                SceneManager.LoadScene(1);
            }
        }
    }

    // Chooses generators to turn off
    void PickGen(int num)
    {
        int pick = generators.Count;
        int index;
        Generator tempGen;

        for (int i = 0; i < num; i++)
        {
            index = Random.Range(0, pick);

            generators[index].TurnOnOff();

            tempGen = generators[pick - 1];
            generators[pick - 1] = generators[index];
            generators[index] = tempGen;

            pick--;
        }
    }
}
