using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Camera))]
public class MonsterIsSeenChecker : MonoBehaviour
{
    [SerializeField] Transform CheckIsMonsgterInView_Shootpoint;
    [SerializeField] LayerMask hitMask;

    [Header("Flashlight")]
    [SerializeField] GameObject FlashlightOBJ;
    [SerializeField] float CurrentFlashlightPower;
    [SerializeField] float FlashlightUsePower;
    [SerializeField] float FlashlightReplenishPower;
    [SerializeField] float FlashbangPowerUse;
    bool isInFlashlightRechargeState = false;
    bool isFlashlightOn = false;
    Transform Monster;
    Camera cam;
    ScaryMonster monsterManager;
    bool CanFlashMonster;

    [Header("UI")]
    [SerializeField] Slider FlashlightBattery;
    [SerializeField] Image BatteryOutline;
    [SerializeField] Image BatteryFill;
    [SerializeField] Color NormalFlashlightPower = Color.white;
    [SerializeField] Color ExhaustFlashlightPower = Color.white;
    [SerializeField] Color FlashlightOnColor = Color.green;
    [SerializeField] Color FlashlightOffColor = Color.white;

    private void Start()
    {
        monsterManager = FindObjectOfType<ScaryMonster>();
        Monster = monsterManager.monsterAI.transform;
        cam = GetComponent<Camera>();
        FlashlightBattery.maxValue = 100;
    }

    private void Update()
    {
        Vector3 ViewportPositionOfMonster = cam.WorldToViewportPoint(Monster.position);
        bool isInCamera = true;

        if (ViewportPositionOfMonster.x < 0 || ViewportPositionOfMonster.x > 1)
        {
            isInCamera = false;
        }
        if (ViewportPositionOfMonster.y < 0 || ViewportPositionOfMonster.y > 1)
        {
            isInCamera = false;
        }
        if (ViewportPositionOfMonster.z <= 0)
        {
            isInCamera = false;
        }

        if (isInCamera)
        {
            CheckIsMonsgterInView_Shootpoint.LookAt(Monster.position);

            RaycastHit hit;
            if (Physics.Raycast(CheckIsMonsgterInView_Shootpoint.position, CheckIsMonsgterInView_Shootpoint.forward, out hit, Mathf.Infinity, hitMask, QueryTriggerInteraction.Ignore))
            {
                Debug.Log(hit.transform.name);

                if (hit.transform != Monster)
                {
                    isInCamera = false;
                    CanFlashMonster = false;
                }
                else
                {
                    CanFlashMonster = true;
                }
            }
            else
            {
                CanFlashMonster = false;
            }
        }
        else
        {
            CanFlashMonster = false;
        }

        monsterManager.isSeen = isInCamera;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (CanFlashMonster && !isInFlashlightRechargeState)
            {
                monsterManager.MonsterFlashed();
                Debug.Log("Flashed Monster");
            }
            CurrentFlashlightPower -= FlashbangPowerUse;
        }


        if (CurrentFlashlightPower <= 0)
        {
            isFlashlightOn = false;
            isInFlashlightRechargeState = true;
        }



        if (isFlashlightOn)
        {
            BatteryOutline.color = FlashlightOnColor;
            FlashlightOBJ.SetActive(true);
            CurrentFlashlightPower -= FlashlightUsePower * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                isFlashlightOn = false;
            }
        }
        else
        {
            BatteryOutline.color = FlashlightOffColor;
            FlashlightOBJ.SetActive(false);
            if (CurrentFlashlightPower < 100)
            {
                CurrentFlashlightPower += FlashlightReplenishPower * Time.deltaTime;
            }
            else
            {
                CurrentFlashlightPower = 100;
            }

            if (CurrentFlashlightPower < 0)
            {
                CurrentFlashlightPower = 0;
            }

                if (CurrentFlashlightPower > 50)
            {
                isInFlashlightRechargeState = false;
            }


                if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!isInFlashlightRechargeState)
                {
                    isFlashlightOn = true;
                }
            }
        }

        if (isInFlashlightRechargeState)
        {
            BatteryFill.color = ExhaustFlashlightPower;
        }
        else
        {
            BatteryFill.color = NormalFlashlightPower;
        }

        FlashlightBattery.value = CurrentFlashlightPower;
    }
}
