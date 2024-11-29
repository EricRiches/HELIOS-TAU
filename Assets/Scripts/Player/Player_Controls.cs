using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player_Controls : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float gravity;
    [SerializeField] Transform GroundCheck;
    [SerializeField] LayerMask GroundMask;
    [SerializeField] GameObject canvas;

    CharacterController controller;
    Generator gen;
    Airlock airlock;

    bool isUIOn = false;
    bool isDead = false;

    float breakerSwitch = 3f;
    float timer = 0;
    int whichKind; //To determine if its a generator or airlock switch

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!isDead)
        {
            MoveUpdate();
            GravityUpdate();

            if (Input.GetKey(KeyCode.E) && isUIOn)
            {
                timer += Time.deltaTime;

                if (timer >= breakerSwitch)
                {
                    RepairGenerator(whichKind);
                }
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                timer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Generator"))
        {
            gen = other.transform.gameObject.GetComponent<Generator>();

            if (gen != null && !gen.GetIsOn())
            {
                whichKind = 0;
                AddRepairUI();
            }
        }

        if (other.gameObject.CompareTag("Airlock"))
        {
            airlock = other.transform.parent.gameObject.GetComponent<Airlock>();
            
            whichKind = 1;
            AddRepairUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Generator") || other.gameObject.CompareTag("Airlock"))
        {
            RemoveRepairUI();
        }
    }

    // Adds a Pop-Up telling the player to press a button to turn on the generator
    void AddRepairUI()
    {
        Debug.Log("Is In Repair Area");
        isUIOn = true;
        canvas.SetActive(true);
    }


    // Removes a Pop-Up telling the player to press a button to turn on the generator
    void RemoveRepairUI()
    {
        isUIOn = false;
        canvas.SetActive(false);
    }

    void RepairGenerator(int num)
    {
        if (num == 0)
        {
            gen.TurnOnOff();
        }
        else if (num == 1)
        {
            airlock.TrySwitch();
        }
        else
        {

        }
        RemoveRepairUI();
    }

    #region Movement

    void MoveUpdate()
    {
        float y = Input.GetAxisRaw("Vertical");
        float x = Input.GetAxisRaw("Horizontal");

        Vector3 MoveDir = (transform.forward * y) + (transform.right * x);
        if (MoveDir.magnitude > 1)
        {
            MoveDir.Normalize();
        }

        controller.Move(speed * Time.deltaTime * MoveDir);
    }

    bool isGrounded = false;
    float YVelocity = 0;
    void GravityUpdate()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, 0.1f, GroundMask);

        if (!isGrounded)
        {
            YVelocity -= gravity * Time.deltaTime;
        }
        else
        {
            YVelocity = -0.1f;
        }

        controller.Move(YVelocity * Time.deltaTime * Vector3.up);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(GroundCheck.position, 0.1f);
    }
}
