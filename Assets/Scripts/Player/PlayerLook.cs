using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform PlayerBody;
    [SerializeField] Vector2 Sensitivity;

    float yRotation;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            Sensitivity = manager.Sensitivity;
        }
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");

        PlayerBody.Rotate(0, Time.deltaTime * x * Sensitivity.x, 0);

        yRotation += Time.deltaTime * y * Sensitivity.y;
        yRotation = Mathf.Clamp(yRotation, -61, 45);
        transform.localRotation = Quaternion.Euler(yRotation, 0, 0);
    }
}
