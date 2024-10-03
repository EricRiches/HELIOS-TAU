using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class ScaryMonster : MonoBehaviour
{
    public bool AreAllGeneratorsOn = false;
    public MonsterState currentState;
    [SerializeField] Transform PlayerRefrence;
    [SerializeField] GameObject MonsterAISounds;
    [SerializeField] AudioSource MonsterVoiceSource;

    [Header("Change Behvaior")]
    [SerializeField] Vector2 TimeSpentInBehavior;
    [SerializeField] float BehaviorTime;

    [Header("Vent")]
    [SerializeField] Vent[] InitialisedVents;
    [SerializeField] int CurrentVent;
    public float AttackRange;
    [SerializeField] GameObject VentSoundBox;

    [Header("AI")]
    public NavMeshAgent monsterAI;
    [SerializeField] float KillDistance = 1.25f;
    Renderer monsterAIRenderer = null;

    [Header("Peek A Boo")]
    [SerializeField] float PeekABoo_Speed;
    public bool isSeen = false;
    [SerializeField] AudioClip WalkingSound;
    [SerializeField] AudioClip StandingSound;

    [Header("Silent Sneak")]
    [SerializeField] float SilentSneak_Speed;

    private void Start()
    {
        ChooseRandomChangeBehaviorTime();
        Vent_Start();
    }

    private void Update()
    {
        VentSoundBox.SetActive(currentState == MonsterState.Vent);

        if (!AreAllGeneratorsOn)
        {
            if (currentState == MonsterState.Vent)
            {
                Vent_Update();
                MonsterAISounds.SetActive(false);
            }
            else if (currentState == MonsterState.PeekABoo)
            {
                PeekABoo_Update();
                MonsterAISounds.SetActive(true);
            }
            else if (currentState == MonsterState.SilentSneak)
            {
                SilentSneak_Update();
                MonsterAISounds.SetActive(false);
            }

            BehaviorTime -= Time.deltaTime;
            if (BehaviorTime <= 0)
            {
                ChangeBehavior();
            }
        }
        else
        {
            SilentSneak_Update();
            MonsterAISounds.SetActive(true);
            MonsterVoiceSource.clip = WalkingSound;
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
        if (!AreAllGeneratorsOn)
        {
            Vent_Start();
            ChooseRandomChangeBehaviorTime();
        }
    }

    #endregion

    #region Vent

    void Vent_Start()
    {
        currentState = MonsterState.Vent;
        CurrentVent = Random.Range(0, InitialisedVents.Length);
        monsterAI.gameObject.SetActive(false);
        VentSoundBox.transform.position = InitialisedVents[CurrentVent].SoundOriginPoint.position;
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
            MonsterVoiceSource.clip = StandingSound;
        }
        else
        {
            monsterAI.speed = PeekABoo_Speed;
            monsterAI.SetDestination(PlayerRefrence.position);
            MonsterVoiceSource.clip = WalkingSound;

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

    public void ActivateHunt()
    {
        SilentSneak_Start();
        AreAllGeneratorsOn = true;
    }

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