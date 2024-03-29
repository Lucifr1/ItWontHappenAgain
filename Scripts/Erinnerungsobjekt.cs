using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class Erinnerungsobjekt : MonoBehaviour
{
    public VideoClip video;
    public GameObject videoPlayingObjects;

    private GameObject player;
    private GameObject camera;
    private GameObject canvasInteragieren;
    private RaycastHit hit;

    public bool erinnerungsObjektAngeschaut;
    public bool isPlaying;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = player.transform.GetChild(0).gameObject;
        erinnerungsObjektAngeschaut = false;
        isPlaying = false;
    }

    public void startVideoSequence()
    {
        //PLAY SOUND VON ERINNERUNGSOBJEKT SCHATTENSPIEL CUTSCENE !! uwu (audioquelle auf player damit mans überall gut hört? + auch weniger pain so zu adden)
        isPlaying = true;

        for (int i = 0; i < videoPlayingObjects.transform.childCount; i++)
        {
            //turn on spotlight
            videoPlayingObjects.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);

            videoPlayingObjects.transform.GetChild(i).GetComponent<VideoPlayer>().clip = video;
        }

        StartCoroutine(CloseVideo());
    }

    IEnumerator CloseVideo()
    {
        float videolength = (float) video.length;
        yield return new WaitForSeconds(videolength);

        for (int i = 0; i < videoPlayingObjects.transform.childCount; i++)
        {
            //turn off spotlight
            videoPlayingObjects.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

            videoPlayingObjects.transform.GetChild(i).GetComponent<VideoPlayer>().clip = null;
        }

        erinnerungsObjektAngeschaut = true;
        isPlaying = false;
    }
}