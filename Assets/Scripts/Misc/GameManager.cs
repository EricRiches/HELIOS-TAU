using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float Volume;
    public Vector2 Sensitivity;

    private void Start()
    {
        DontDestroyOnLoad(this);
        ChangeSceenToMainMenu();
    }

    public void ChangeSceenToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
