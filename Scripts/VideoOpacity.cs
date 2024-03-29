using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoOpacity : MonoBehaviour
{
    private RawImage rawImage;
    private float alpha;
    
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        alpha += 0.001f;
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, alpha);
    }
}
