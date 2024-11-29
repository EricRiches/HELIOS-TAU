using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Airlock : MonoBehaviour
{
    bool inAirlock = false;

    CapsuleCollider lockSwitch;
    CapsuleCollider lockSwitch2;

    // Start is called before the first frame update
    void Start()
    {
        lockSwitch = transform.GetChild(0).gameObject.GetComponentInChildren<CapsuleCollider>();
        lockSwitch2 = transform.GetChild(1).gameObject.GetComponentInChildren<CapsuleCollider>();
    }

    public void SetEndGameTrue()
    {
        lockSwitch.enabled = true;
        lockSwitch2.enabled = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            inAirlock = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            inAirlock = false;
        }
    }

    public void TrySwitch()
    {
        if (inAirlock)
        {
            SceneManager.LoadScene(1);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
