using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
    public Transform MonsterSearchPoint;
    public Transform SoundOriginPoint;
    public Transform MonsterWalkOutOfVentPoint;
    ScaryMonster monsterRef;

    private void OnDrawGizmos/*Selected*/()
    {
        if (monsterRef == null)
        {
            monsterRef = FindAnyObjectByType<ScaryMonster>();
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(MonsterSearchPoint.position, monsterRef.AttackRange);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(MonsterWalkOutOfVentPoint.position, 0.1f);
    }
}
