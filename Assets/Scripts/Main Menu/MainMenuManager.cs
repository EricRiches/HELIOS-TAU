using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    GameManager manager;
    [SerializeField] Slider Volume;
    [SerializeField] Slider XSensitivity;
    [SerializeField] Slider YSensitivity;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        Volume.value = manager.Volume;
        XSensitivity.value = manager.Sensitivity.x;
        YSensitivity.value = manager.Sensitivity.y;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeVolume(float newVolume)
    {
        manager.Volume = newVolume;
    }

    public void ChangeXSensitivity(float newSensitivity)
    {
        manager.Sensitivity.x = newSensitivity;
    }

    public void ChangeYSensitivity(float newSensitivity)
    {
        manager.Sensitivity.y = newSensitivity;
    }
}
