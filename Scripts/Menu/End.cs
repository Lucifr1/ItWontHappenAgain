using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class End : MonoBehaviour
{
    [SerializeField] private VideoClip endScreen;
    [SerializeField] private VideoClip notrufnummern;
    private bool secondVideoPlaying;
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer.targetTexture.Release();
        videoPlayer.clip = endScreen;
    }

    void Update()
    {
        if(Time.timeSinceLevelLoad > 9)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                videoPlayer.clip = notrufnummern;
                secondVideoPlaying = true;
            }
        }
        
        if(secondVideoPlaying && Time.timeSinceLevelLoad > 15)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(4);
            }
        }
    }
}
