using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAmbience : MonoBehaviour
{
    [SerializeField] AudioSource m_AudioSource;
    [SerializeField] AudioClip[] AmbienceSounds;
    [SerializeField] Vector2 RandomPlaySoundLength;
    float RandomSoundPlayTimer = 10;

    private void Start()
    {
        RandomTime();
    }

    private void Update()
    {
        RandomSoundPlayTimer -= Time.deltaTime;

        if (RandomSoundPlayTimer <= 0)
        {
            PlaySound();
        }
    }

    void RandomTime()
    {
        RandomSoundPlayTimer = Random.Range(RandomPlaySoundLength.x, RandomPlaySoundLength.y);
    }

    void PlaySound()
    {
        int RandomSound = Random.Range(0, AmbienceSounds.Length);
        m_AudioSource.clip = AmbienceSounds[RandomSound];
        m_AudioSource.Play();
        RandomTime();
    }
}
