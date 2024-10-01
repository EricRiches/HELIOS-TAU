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

    CharacterController controller;
    Generator gen;

    bool isUIOn = false;
    bool isDead = false;

    float breakerSwitch = 3f;
    float timer = 0;

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
                    RepairGenerator();
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
                AddRepairUI();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Generator"))
        {
            RemoveRepairUI();
        }
    }

    // Adds a Pop-Up telling the player to press a button to turn on the generator
    void AddRepairUI()
    {
        isUIOn = true;
    }


    // Removes a Pop-Up telling the player to press a button to turn on the generator
    void RemoveRepairUI()
    {
        isUIOn = false;
    }

    void RepairGenerator()
    {
        gen.TurnOnOff();

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
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, GroundMask);

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
}
