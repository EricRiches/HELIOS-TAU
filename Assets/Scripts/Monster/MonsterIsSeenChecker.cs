using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MonsterIsSeenChecker : MonoBehaviour
{
    [SerializeField] Transform CheckIsMonsgterInView_Shootpoint;
    [SerializeField] LayerMask hitMask;
    Transform Monster;
    Camera cam;
    ScaryMonster monsterManager;

    private void Start()
    {
        monsterManager = FindObjectOfType<ScaryMonster>();
        Monster = monsterManager.monsterAI.transform;
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 ViewportPositionOfMonster = cam.WorldToViewportPoint(Monster.position);
        bool isInCamera = true;

        if (ViewportPositionOfMonster.x < 0 || ViewportPositionOfMonster.x > 1)
        {
            isInCamera = false;
            Debug.Log("Function");
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
            if (Physics.Raycast(CheckIsMonsgterInView_Shootpoint.position, CheckIsMonsgterInView_Shootpoint.forward, out hit, Mathf.Infinity, hitMask))
            {
                if (hit.transform != Monster)
                {
                    isInCamera = false;
                }
            }
        }

        monsterManager.isSeen = isInCamera;
    }
}
