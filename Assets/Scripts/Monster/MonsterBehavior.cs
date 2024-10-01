using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehavior : MonoBehaviour
{
    [Header("Developer")]
    [SerializeField] bool LetMonsterChooseBehavior = true;

    [Header("Base Refrences")]
    [SerializeField] MonBehavior currentBehavior;
    [SerializeField] NavMeshAgent AIMonster;
    [SerializeField] AudioSource AIMonsterSound;
    [SerializeField] Transform PlayerPosition;
    [SerializeField] NavMeshObstacle PlayerAIObstacle;

    [Header("Base")]
    [SerializeField] float CurrentLightViewDistance;

    [Header("Stalk")]
    [SerializeField] float Stalk_FollowDistance;
    [SerializeField] float Stalk_LightOffset;

    [Header("Scatter")]
    [SerializeField] Vector2 Scatter_ForwardDistance;
    [SerializeField] float Scatter_HorizontalDistance;
    [SerializeField] float Scatter_WaitTillSwitchTime;
    float Scatter_WaitTimer;

    [Header("Feign Attack")]
    [SerializeField] float Attack_TargetBehindPlayer;
    [SerializeField] float Attack_WaitTime;
    float Attack_Timer;
    int Attack_Step = 0;

    [Header("Roam")]
    [SerializeField] Transform[] Roam_DestinationPoints;

    [Header("Break Generators")]
    [SerializeField] Generator[] Generators;
    [SerializeField] float Generator_BreakTime;
    Generator currentGeneratorDestination = null;
    float Generator_Timer;

    [Header("Hunting")]
    [SerializeField] float Hunt_ViewingDistance;
    [SerializeField] float Hunt_StayTime;
    [SerializeField] Transform Hunt_ChaserPointer;
    [SerializeField] LayerMask Hunt_Mask;
    float Hunt_Timer = 0;
    bool Hunt_IsChasing = false;

    [Header("Behaviors")]
    [SerializeField] GeneratorBehaviorWeights[] RandomBehaviorWeights;

    [Header("Randomize Behavior Times")]
    [SerializeField] Vector2 ChangeBehavior_Time;
    [SerializeField] int ChangeBehavior_Weight;
    [SerializeField] int ChangeBehavior_MaxWeight;
    float ChangeBehavior_Timer;

    private void Start()
    {
        ChooseRandomBehavior();
    }

    private void Update()
    {
        if (currentBehavior == MonBehavior.Stalk)
        {
            StalkUpdate();
            ChangeBehaviorUpdate();
        }
        else if (currentBehavior == MonBehavior.Scatter)
        {
            ScatterUpdate();
        }
        else if (currentBehavior == MonBehavior.FeignAttack)
        {
            AttackUpdate();
        }
        else if (currentBehavior == MonBehavior.Roam)
        {
            RoamUpdate();
            ChangeBehaviorUpdate();
        }
        else if (currentBehavior == MonBehavior.BreakGenerator)
        {
            BreakGeneratorUpdate();
        }
        else if (currentBehavior == MonBehavior.Hunt)
        {
            HuntUpdate();
        }

        PlayerAIObstacle.enabled = currentBehavior != MonBehavior.Hunt;
    }

    #region Behavior
    void ChooseRandomBehavior()
    {
        if (LetMonsterChooseBehavior)
        {
            List<Generator> workingGenerators = new List<Generator>();
            foreach (Generator generator in Generators)
            {
                if (generator.isOn)
                {
                    workingGenerators.Add(generator);
                }
            }

            int NumberOfWorkingGenerators = workingGenerators.Count;

            int currentAmountOfGenrators = NumberOfWorkingGenerators - 1;
            int TotalWeight = RandomBehaviorWeights[currentAmountOfGenrators].Roam +
                RandomBehaviorWeights[currentAmountOfGenrators].Stalk +
                RandomBehaviorWeights[currentAmountOfGenrators].BreakGenerator;
            int RandomWeight = Random.Range(0, TotalWeight + 1);

            if (RandomWeight <= RandomBehaviorWeights[currentAmountOfGenrators].Roam)
            {
                ChooseRoamPosition();
                currentBehavior = MonBehavior.Roam;
            }
            else if (RandomWeight <= RandomBehaviorWeights[currentAmountOfGenrators].Roam +
                RandomBehaviorWeights[currentAmountOfGenrators].Stalk)
            {
                currentBehavior = MonBehavior.Stalk;
            }
            else
            {
                StartBreakGenerator();
            }
        }
        else
        {
            if (currentBehavior == MonBehavior.Roam)
            {
                ChooseRoamPosition();
            }
            else if (currentBehavior == MonBehavior.BreakGenerator)
            {
                StartBreakGenerator();
            }
            else if (currentBehavior == MonBehavior.Hunt)
            {
                StartHunt();
            }
        }
    }

    #endregion

    #region Stalk

    void StalkUpdate()
    {
        if (Vector3.Distance(AIMonster.transform.position, PlayerPosition.position) > Stalk_FollowDistance)
        {
            AIMonster.destination = PlayerPosition.position;
        }
        else if (Vector3.Distance(PlayerPosition.position, AIMonster.transform.position) < CurrentLightViewDistance - Stalk_LightOffset)
        {
            StartScatter();
            Debug.Log("Monster is in Scatter");
        }
        else
        {
            AIMonster.destination = AIMonster.transform.position;
        }
    }

    #endregion

    #region Scatter

    void StartScatter()
    {
        AIMonster.destination = PlayerPosition.position +
            (PlayerPosition.forward * Random.Range(Scatter_ForwardDistance.x, Scatter_ForwardDistance.y)) +
            (PlayerPosition.right * Random.Range(-Scatter_HorizontalDistance, Scatter_HorizontalDistance));

        currentBehavior = MonBehavior.Scatter;
        Scatter_WaitTimer = Scatter_WaitTillSwitchTime;
    }

    void ScatterUpdate()
    {
        if (AIMonster.velocity.magnitude <= 0.25f)
        {
            Scatter_WaitTimer -= Time.deltaTime;

            if (Scatter_WaitTimer <= 0)
            {
                if (Vector3.Distance(PlayerPosition.position, AIMonster.transform.position) <= CurrentLightViewDistance)
                {
                    StartAttack();
                }
                else
                {
                    ChooseRandomBehavior();
                }
            }
        }
        else
        {
            Scatter_WaitTimer = Scatter_WaitTillSwitchTime;
        }
    }

    #endregion

    #region Feign Attack

    void StartAttack()
    {
        Debug.Log("Monster feigned attack");
        AIMonster.destination = PlayerPosition.position + (-PlayerPosition.forward * Attack_TargetBehindPlayer);
        currentBehavior = MonBehavior.FeignAttack;
        Attack_Step = 0;
    }

    void AttackUpdate()
    {
        if (Attack_Step == 0)
        {
            if (Vector3.Distance(new Vector3(AIMonster.transform.position.x, AIMonster.destination.y, AIMonster.transform.position.z), AIMonster.destination) <= 0.1f)
            {
                Attack_Step++;
                ChooseRoamPosition();
                Attack_Timer = Attack_WaitTime;
            }
        }
        else
        {
            Attack_Timer -= Time.deltaTime;

            if (Vector3.Distance(new Vector3(AIMonster.transform.position.x, AIMonster.destination.y, AIMonster.transform.position.z), AIMonster.destination) <= 0.1f)
            {
                EndAttack();
            }
            else if (Attack_Timer <= 0)
            {
                EndAttack();
            }
        }
    }

    void EndAttack()
    {
        ChooseRandomBehavior();
    }

    #endregion

    #region Roam

    void ChooseRoamPosition()
    {
        int randomDestination = Random.Range(0, Roam_DestinationPoints.Length);

        AIMonster.destination = Roam_DestinationPoints[randomDestination].position;
    }

    void RoamUpdate()
    {
        if (AIMonster.remainingDistance <= 0.1f)
        {
            ChooseRoamPosition();
        }
    }

    #endregion

    #region Break Generator

    void StartBreakGenerator()
    {
        ChooseGenerator();
        currentBehavior = MonBehavior.BreakGenerator;
    }

    void ChooseGenerator()
    {
        Generator_Timer = Generator_BreakTime;

        int RandomGenerator = Random.Range(0, Generators.Length);
        currentGeneratorDestination = Generators[RandomGenerator];

        AIMonster.destination = currentGeneratorDestination.MonsterStandPlace.position;
    }

    void BreakGeneratorUpdate()
    {
        if (AIMonster.remainingDistance <= 0.1f)
        {
            if (currentGeneratorDestination.isOn)
            {
                Generator_Timer -= Time.deltaTime;

                if (Generator_Timer <= 0)
                {
                    currentGeneratorDestination.isOn = false;
                    bool isThereAPoweredGenerator = false;

                    foreach (Generator gen in Generators)
                    {
                        if (gen.isOn)
                        {
                            isThereAPoweredGenerator = true;
                            break;
                        }
                    }

                    if (isThereAPoweredGenerator)
                    {
                        ChooseRandomBehavior();
                    }
                    else
                    {
                        currentBehavior = MonBehavior.Hunt;
                        StartHunt();
                    }
                }

                if (Vector3.Distance(PlayerPosition.position, AIMonster.transform.position) < CurrentLightViewDistance - Stalk_LightOffset)
                {
                    StartScatter();
                    Debug.Log("Monster is in Scatter");
                }
            }
            else
            {
                int RandomCheckAnother = Random.Range(1,3);
                if (RandomCheckAnother == 1)
                {
                    ChooseGenerator();
                }
                else
                {
                    ChooseRandomBehavior();
                }
            }
        }
    }

    #endregion

    #region Hunt

    void StartHunt()
    {
        ChooseRoamPosition();
    }

    void HuntUpdate()
    {
        if (!Hunt_IsChasing)
        {
            Hunt_ChaserPointer.LookAt(PlayerPosition.position);
            RaycastHit hit;
            if (Physics.Raycast(Hunt_ChaserPointer.position, Hunt_ChaserPointer.forward, out hit , Hunt_ViewingDistance, Hunt_Mask))
            {
                if (hit.transform.tag == "Player")
                {
                    Hunt_IsChasing = true;
                }
            }
            else if (AIMonster.remainingDistance <= 0.1f)
            {
                ChooseRoamPosition();
            }
        }
        else
        {
            Hunt_ChaserPointer.LookAt(PlayerPosition.position);
            RaycastHit hit;
            if (Physics.Raycast(Hunt_ChaserPointer.position, Hunt_ChaserPointer.forward, out hit, Hunt_ViewingDistance, Hunt_Mask))
            {
                if (hit.transform.tag == "Player")
                {
                    AIMonster.destination = hit.point;
                    Hunt_Timer = Hunt_StayTime;
                    if (Vector3.Distance(AIMonster.transform.position, PlayerPosition.position) <= 0.75f)
                    {
                        FindObjectOfType<GameManager>().ChangeSceenToMainMenu();
                    }
                }
            }
            else if (AIMonster.remainingDistance <= 0.1f)
            {
                Hunt_Timer -= Time.deltaTime;

                if (Hunt_Timer <= 0)
                {
                    Hunt_IsChasing = false;
                    ChooseRoamPosition();
                }
            }
        }
    }

    #endregion

    #region Change Behavior Timers

    void UpdateChangeBehaviorTimer()
    {
        ChangeBehavior_Timer = Random.Range(ChangeBehavior_Time.x, ChangeBehavior_Time.y);
        Debug.Log("Set Change Behavior Timer to " + ChangeBehavior_Timer.ToString() + " seconds.");
    }

    void ChangeBehaviorUpdate()
    {
        ChangeBehavior_Timer -= Time.deltaTime;

        if (ChangeBehavior_Timer <= 0)
        {
            int randomWeight = Random.Range(1, ChangeBehavior_MaxWeight + 1);
            if (randomWeight <= ChangeBehavior_Weight)
            {
                ChooseRandomBehavior();
                Debug.Log("Did change weight.");
            }
            else
            {
                Debug.Log("Did not change weight.");
            }
            UpdateChangeBehaviorTimer();
        }
    }

    #endregion
    private void OnDrawGizmosSelected()
    {
        if (currentBehavior == MonBehavior.Stalk)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(PlayerPosition.position, Stalk_FollowDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(PlayerPosition.position, CurrentLightViewDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(PlayerPosition.position, CurrentLightViewDistance - Stalk_LightOffset);
        }
        else if (currentBehavior == MonBehavior.Scatter)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(PlayerPosition.position, PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.x));
            Gizmos.DrawWireSphere(PlayerPosition.position, CurrentLightViewDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.y),
                PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.x));

            Gizmos.DrawLine(PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.y) +
                (PlayerPosition.right * Scatter_HorizontalDistance),
                PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.y) +
                (PlayerPosition.right * -Scatter_HorizontalDistance));

            Gizmos.DrawLine(PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.y) +
                (PlayerPosition.right * Scatter_HorizontalDistance),
                PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.x));

            Gizmos.DrawLine(PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.y) +
                (PlayerPosition.right * -Scatter_HorizontalDistance),
                PlayerPosition.position + (PlayerPosition.forward * Scatter_ForwardDistance.x));
        }
        else if (currentBehavior == MonBehavior.Hunt)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(AIMonster.transform.position, Hunt_ViewingDistance);
        }
    }
}

public enum MonBehavior
{
    Stalk,
    BreakGenerator,
    Hunt,
    Scatter,
    FeignAttack,
    Roam
}

[System.Serializable]
public class GeneratorBehaviorWeights
{
    public int Roam;
    public int Stalk;
    public int BreakGenerator;
}