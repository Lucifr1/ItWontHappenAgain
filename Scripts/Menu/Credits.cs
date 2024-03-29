using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Credits : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
   
    void Start()
    {
        videoPlayer.targetTexture.Release();
    }

    
    void Update()
    {
        if (Time.timeSinceLevelLoad > 14)
        {
            SceneManager.LoadScene(0);
        }
    }
}
