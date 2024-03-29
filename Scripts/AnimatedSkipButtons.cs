using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedSkipButtons : MonoBehaviour
{
    private RawImage image;
    private float alpha;

    void Start()
    {
        image = GetComponent<RawImage>();
    }

    void Update()
    {
        //anfang auf 0
        //langsam einfaden
        //kurz chillen
        //ausfaden
        //again
        alpha = 0;


/*
        // fade in video
        if (fadeVideoIn)
        {
            if (alpha <= 1)
            {
                x += 0.05f;
                alpha += 0.0005f * x;
                image.color = new Color(image.color.r, image.color.g, image.color.b,
                    alpha);
            }
            else
            {
                blackscreen.gameObject.SetActive(true);
                fadeVideoIn = false;
                alpha = 0;
                x = 0f;
            }
        }*/
    }
}