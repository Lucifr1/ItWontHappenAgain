using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip endOfLoopMusic;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StopBackgroundMusic()
    {
        audioSource.Stop();
    }

    public void RestartBackgroundMusic()
    {
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

    public void SwitchToEndOfLoopMusic()
    {
        Debug.Log("would switch to end of loop music but it's auskommentiert");
        /*
        audioSource.clip = endOfLoopMusic;
        audioSource.Play();
        Debug.Log("meow?");
        */
    }
}