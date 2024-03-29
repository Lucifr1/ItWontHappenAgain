using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Subtitles : MonoBehaviour
{
    private TextMeshProUGUI subtitles;
    private GameObject background;
    public bool currentyTriggered;

    private void Start()
    {
        background = transform.GetChild(0).gameObject;
        subtitles = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void StartSubtitles(string text, float time)
    {/*
        if (currentyTriggered)
        {
            return;
        }*/
        currentyTriggered = true;
        background.SetActive(true);
        subtitles.text = "";
        subtitles.text = text;
        StartCoroutine(WaitForSubtitlesToFinish(time));
    }

    private IEnumerator WaitForSubtitlesToFinish(float time)
    {
        yield return new WaitForSeconds(time);
        DisableSubtitles();
        
    }

    private void DisableSubtitles()
    {
        background.SetActive(false);
        subtitles.text = "";
        currentyTriggered = false;
    }
}
