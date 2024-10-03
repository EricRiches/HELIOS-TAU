using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public GameObject footstep;
    
    // Start is called before the first frame update
    void Start()
    {
        footstep.SetActive(false);
    }

    private bool InputW;
    private bool InputA;
    private bool InputS;
    private bool InputD;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("w"))
        {
            footsteps();
            InputW = true;
        }
        if (Input.GetKeyDown("a"))
        {
            footsteps();
            InputA = true;
        }
        if (Input.GetKeyDown("s"))
        {
            footsteps();
            InputS = true;
        }
        if (Input.GetKeyDown("d"))
        {
            footsteps();
            InputD = true;
        }
        if (Input.GetKeyUp("w"))
        {
            InputW = false;
        }
        if (Input.GetKeyUp("a"))
        {
            InputA = false;
        }
        if (Input.GetKeyUp("s"))
        {
            InputS = false;
        }
        if (Input.GetKeyUp("d"))
        {
            InputD = false;
        }

        if (InputD == false && InputS == false && InputA == false && InputW == false)
        {
            StopFootsteps();
        }
    }

    void footsteps()
    {
        footstep.SetActive(true);
    }
    void StopFootsteps()
    {
        footstep.SetActive(false);
    }
}
