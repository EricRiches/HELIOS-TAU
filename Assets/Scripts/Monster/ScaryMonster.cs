using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ScaryMonster : MonoBehaviour
{
    [SerializeField] MonsterState currentState;
    [SerializeField] Transform PlayerRefrence;

    [Header("Change Behvaior")]
    [SerializeField] Vector2 TimeSpentInBehavior;
    [SerializeField] float BehaviorTime;

    [Header("Vent")]
    [SerializeField] Vent[] InitialisedVents;
    [SerializeField] int CurrentVent;
    public float AttackRange;

    [Header("AI")]
    public NavMeshAgent monsterAI;
    [SerializeField] float KillDistance = 1.25f;
    Renderer monsterAIRenderer = null;

    [Header("Peek A Boo")]
    [SerializeField] float PeekABoo_Speed;
    public bool isSeen = false;

    [Header("Silent Sneak")]
    [SerializeField] float SilentSneak_Speed;

    private void Start()
    {
        ChooseRandomChangeBehaviorTime();
        Vent_Start();
    }

    private void Update()
    {
        if (currentState == MonsterState.Vent)
        {
            Vent_Update();
        }
        else if (currentState == MonsterState.PeekABoo)
        {
            PeekABoo_Update();
        }
        else if (currentState == MonsterState.SilentSneak)
        {
            SilentSneak_Update();
        }

        BehaviorTime -= Time.deltaTime;
        if (BehaviorTime <= 0) 
        {
            ChangeBehavior();
        }
    }

    #region Change Behavior

    void ChangeBehavior()
    {
        if (currentState == MonsterState.Vent)
        {
            int nextBehavior = Random.Range(0, 2);
            if (nextBehavior == 0)
            {
                PeekABoo_Start();
            }
            else if (nextBehavior == 1)
            {
                SilentSneak_Start();
            }
            monsterAI.gameObject.SetActive(true);
        }
        else
        {
            Vent_Start();
        }
        ChooseRandomChangeBehaviorTime();
    }

    void ChooseRandomChangeBehaviorTime()
    {
        BehaviorTime = Random.Range(TimeSpentInBehavior.x, TimeSpentInBehavior.y);
    }

    public void MonsterFlashed()
    {
        Vent_Start();
        ChooseRandomChangeBehaviorTime();
    }

    #endregion

    #region Vent

    void Vent_Start()
    {
        currentState = MonsterState.Vent;
        CurrentVent = Random.Range(0, InitialisedVents.Length);
        monsterAI.gameObject.SetActive(false);
    }

    void Vent_Update()
    {
        if (Vector3.Distance(InitialisedVents[CurrentVent].MonsterSearchPoint.position, PlayerRefrence.position) <= AttackRange)
        {
            PlayerKilled();
        }
    }

    #endregion

    #region Peek A Boo

    void PeekABoo_Start()
    {
        monsterAI.transform.position = InitialisedVents[CurrentVent].MonsterWalkOutOfVentPoint.position;
        currentState = MonsterState.PeekABoo;
    }

    void PeekABoo_Update()
    {
        if (isSeen)
        {
            monsterAI.speed = 0;
            monsterAI.SetDestination(monsterAI.transform.position);
        }
        else
        {
            monsterAI.speed = PeekABoo_Speed;
            monsterAI.SetDestination(PlayerRefrence.position);

            if (Vector3.Distance(monsterAI.transform.position, PlayerRefrence.position) <= KillDistance)
            {
                PlayerKilled();
            }
        }
    }

    #endregion

    #region Silent Sneak

    void SilentSneak_Start()
    {
        monsterAI.transform.position = InitialisedVents[CurrentVent].MonsterWalkOutOfVentPoint.position;
        monsterAI.speed = SilentSneak_Speed;
        currentState = MonsterState.SilentSneak;
    }

    void SilentSneak_Update()
    {
        monsterAI.SetDestination(PlayerRefrence.position);
        if (Vector3.Distance(monsterAI.transform.position, PlayerRefrence.position) <= KillDistance)
        {
            PlayerKilled();
        }
    }

    #endregion

    void PlayerKilled()
    {
        SceneManager.LoadScene(1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(monsterAI.transform.position, KillDistance);
    }
}

public enum MonsterState
{
    Vent,
    PeekABoo,
    SilentSneak
}