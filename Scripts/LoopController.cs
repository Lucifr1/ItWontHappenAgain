using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;
using Cache = Unity.VisualScripting.Cache;

public class LoopController : MonoBehaviour
{
    // Scripts
    private PlayerInteractions playerInteractions;
    private AudioController audioController;
    private AutoPathing autoPathingScript;
    private AutoPathingGuy autoPathingGuy;
    private VoicelineController voicelineController;

    private Vector3 startPlayerPositionEachLoop;
    private Quaternion startPlayerRotationEachLoop;
    private float loopDuration;

    private VideoPlayer videoPlayer;
    private RenderTexture renderTexture;

    public List<VideoClip> LoopCutscenes;
    public List<GameObject> Interactables;
    public List<VideoClip> StartCutscenes;

    // conditions
    private int currentLoop = 0;
    private bool completedCurrentLoopConditions = false;
    public GameObject erinnerungsObjekte;
    private bool startEnding;

    private GameObject videoRawImageGameObject;

    private GameObject player;


    //private AutoPathingCamera autoPathingCameraScript;
    private Camera pathingCamera;
    private int numberOfCorrectInteractions;
    private VideoClip preSave;

    // Autopathing Camera
    private CinemachineController cinemachineControllerScript;

    //Fading in
    private bool fadeVideoIn;
    private RawImage videoRawImage;
    private float alpha;
    private float x;

    private bool interactedWithAllThreeObjects;

    private Image blackscreen;
    private bool fadeBlackscreenOut;
    private float blackScreenAlpha = 1;

    private float timeRemainingInCurrentLoop;
    private bool withinAGamePlayLoop = false;

    // Voicelines
    private AudioSource voicelineplayer;
    private bool justPlayed;
    private bool firstTimeNeedMemories;
    private Coroutine lastRoutine;

    // Skip cutscene
    private float videolength;
    public float loopTime;
    private bool normal;
    private bool triggeredMusicChange;
    private bool currentlyCutscenePlaying;
    private float timeRemainingInCurrentCutscene;
    private float amountRightInteractables;

    [SerializeField] private GameObject skipLoopUI;
    [SerializeField] private GameObject skipCutsceneUI;
    private bool cutsceneStarted;

    [SerializeField] private TextMeshProUGUI timerUI;
    private string timerString;
    private TimeSpan timeSpan;

    private bool cutsceneSkipable;

    public bool playerAutoWalkingStarts;


    void Start()
    {
        blackscreen = GameObject.Find("Blackscreen").GetComponent<Image>();

        videoPlayer = GameObject.FindGameObjectWithTag("UIVideoPlayer").GetComponent<VideoPlayer>();
        videoRawImageGameObject = GameObject.FindGameObjectWithTag("UIVideoRawImage");
        videoRawImage = videoRawImageGameObject.GetComponent<RawImage>();
        renderTexture = videoPlayer.targetTexture;
        videoPlayer.targetTexture.Release();

        // Get scripts
        player = GameObject.FindGameObjectWithTag("Player");
        startPlayerPositionEachLoop = player.transform.position;
        startPlayerRotationEachLoop = player.transform.rotation;
        Debug.Log(startPlayerRotationEachLoop);
        playerInteractions = player.GetComponent<PlayerInteractions>();
        cinemachineControllerScript = player.GetComponent<CinemachineController>();
        autoPathingScript = player.GetComponent<AutoPathing>();
        audioController = player.GetComponent<AudioController>();
        voicelineController = player.transform.GetChild(2).GetComponent<VoicelineController>();

        voicelineplayer = voicelineController.GetComponent<AudioSource>();

        autoPathingGuy = GameObject.FindGameObjectWithTag("GuyController").GetComponent<AutoPathingGuy>();

        startEnding = false;
        justPlayed = false;
        normal = true;
        triggeredMusicChange = false;
        currentlyCutscenePlaying = false;
        firstTimeNeedMemories = false;
        lastRoutine = null;

        // play first cutscene
        FreezePlayer(true);
        videoPlayer.clip = StartCutscenes[0];
        videoRawImageGameObject.SetActive(true);
        StartCoroutine(FirstStartCutscene());
    }

    void Update()
    {
        // Skip loop
        if (withinAGamePlayLoop && amountRightInteractables == 3)
        {
            startEnding = true;
        }

        if (withinAGamePlayLoop && CheckAllTaskDoneLoop() && normal && !startEnding && !playerInteractions.hasObjectInHand)
        {
            // Enable UI 
            skipLoopUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                skipLoopUI.SetActive(false);
                Debug.Log("Loop speed up");
                normal = false;
            }
        }
        else
        {
            skipLoopUI.SetActive(false);
        }
        

        //Time.timeSinceLevelLoad für erste Cutscene, weil die nicht eingefaded wird
        if (currentlyCutscenePlaying && !fadeVideoIn && cutsceneSkipable)
        {
            //skipCutsceneUI.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                skipCutsceneUI.SetActive(false);
                Debug.Log("Skipped cutscene");
                videolength = 0f;
                cutsceneStarted = false;
            }
        }

        if (startEnding)
        {
            if (CheckSeenAllMemories())
            {
                if (!justPlayed)
                {
                    StartCoroutine(DoorVoiceline(true));
                }

                StopTimerAndActivateDoorInteraction();
            }
            else
            {
                if (!justPlayed)
                {
                    StartCoroutine(DoorVoiceline(false));
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Loop timer
        timeRemainingInCurrentLoop = loopTime;

        if (withinAGamePlayLoop)
        {
            if (!normal)
            {
                loopTime -= Time.deltaTime * 20f;
            }
            else
            {
                loopTime -= Time.deltaTime;
            }

            if (timeRemainingInCurrentLoop < 15 && !triggeredMusicChange)
            {
                audioController.SwitchToEndOfLoopMusic();
                triggeredMusicChange = true;
            }

            if (timeRemainingInCurrentLoop <= 0)
            {
                withinAGamePlayLoop = false;
            }
        }

        //Loop Timer UI

        if (!startEnding && !currentlyCutscenePlaying)
        {
            timeSpan = TimeSpan.FromSeconds(timeRemainingInCurrentLoop);
            timerString = timeSpan.ToString("mm':'ss");
            timerUI.text = timerString;
        }
        else
        {
            timerUI.text = "";
        }

        // Cutscene timer
        timeRemainingInCurrentCutscene = videolength;

        if (currentlyCutscenePlaying)
        {
            videolength -= Time.deltaTime;

            if (timeRemainingInCurrentCutscene <= 0)
            {
                currentlyCutscenePlaying = false;
            }
        }

        // fade in video
        if (fadeVideoIn)
        {
            if (alpha <= 1)
            {
                x += 0.05f;
                alpha += 0.0005f * x;
                videoRawImage.color = new Color(videoRawImage.color.r, videoRawImage.color.g, videoRawImage.color.b,
                    alpha);
            }
            else
            {
                blackscreen.gameObject.SetActive(true);
                fadeVideoIn = false;
                alpha = 0;
                x = 0f;
            }
        }

        // fade out blackscreen
        if (fadeBlackscreenOut)
        {
            if (blackScreenAlpha >= 0)
            {
                blackScreenAlpha -= 0.01f;
                blackscreen.color = new Color(blackscreen.color.r, blackscreen.color.g, blackscreen.color.b,
                    blackScreenAlpha);
            }
            else
            {
                blackscreen.color = new Color(blackscreen.color.r, blackscreen.color.g, blackscreen.color.b, 1);
                blackscreen.gameObject.SetActive(false);
                blackScreenAlpha = 1;
                fadeBlackscreenOut = false;
            }
        }
    }
    
    private void StartLoop()
    {
        skipCutsceneUI.SetActive(false);
        
        Debug.Log("currentLoop = " + currentLoop);
        if (currentLoop != 0)
        {
            completedCurrentLoopConditions = false;
        }

        loopDuration = 60 + 90 * currentLoop; //60 + 30 * loopNumber
        Debug.Log("loopDuration" + loopDuration);
        StartCoroutine(LoopTimer(loopDuration));
    }

    IEnumerator FirstStartCutscene()
    {
        StartCoroutine(SkipCutsceneUI(skipCutsceneUI, 6, 5));
        videolength = (float) videoPlayer.length;
        currentlyCutscenePlaying = true;
        yield return new WaitUntil(() => !currentlyCutscenePlaying);

        FadeOutBlackScreen();

        videoRawImageGameObject.SetActive(false);
        videoPlayer.targetTexture.Release();
        videoPlayer.clip = null;

        FreezePlayer(false);
        StartLoop();
        autoPathingGuy.StartWalkCycle();

        audioController.RestartBackgroundMusic();
        yield return new WaitForSeconds(7);
        if (voicelineplayer.isPlaying)
        {
            yield return new WaitUntil(() => !voicelineplayer.isPlaying);
        }

        if (withinAGamePlayLoop)
        {
            voicelineController.SetCurrentVoiceline(currentLoop.ToString() + "Erinnerung");
        }
    }

    IEnumerator LoopTimer(float time)
    {
        loopTime = time;
        withinAGamePlayLoop = true;
        Debug.Log("Started" + currentLoop + " at : " + Time.time);
        yield return new WaitUntil(() => !withinAGamePlayLoop);
        Debug.Log("Finished" + currentLoop + " at: " + Time.time);
        normal = true;

        // wait for memory cutscene to finish
        for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
        {
            if (erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().isPlaying)
            {
                yield return new WaitUntil(() =>
                    !erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().isPlaying);
            }
        }

        // wait for player to place object
        if (playerInteractions.hasObjectInHand)
        {
            yield return new WaitUntil(() => !playerInteractions.hasObjectInHand);

            // ENTSCHEIDUNG TREFFEN: IM GOOGLE DOC ALS TO DO GANZ UNTEN EXTRA
            // GERADE HIER DESWEGEN ABER DOPPELT DAMIT ES SINNVOLL FUNKTIONIERT ABER SO IST NICHT GEIL
            for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
            {
                if (erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().isPlaying)
                {
                    yield return new WaitUntil(() =>
                        !erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().isPlaying);
                }
            }
        }

        UpdateCurrentLoop();
        autoPathingGuy.loopNumber++;

        if (interactedWithAllThreeObjects)
        {
            startEnding = true;
            yield break;
        }

        skipLoopUI.SetActive(false);

        // Save correct video
        preSave = ReturnVideoClip();

        // Voiceline Emilia herrufen anhand number of correct interactions
        voicelineController.SetCurrentVoiceline(numberOfCorrectInteractions.ToString());


        FreezePlayer(true);
        audioController.StopBackgroundMusic();

        // Switch cameras
        cinemachineControllerScript.SetPathingCameraSetup(numberOfCorrectInteractions);
        cinemachineControllerScript.hasFinishedTransitionBlending = false;
        
        // player starts walking
        autoPathingScript.hasReachedDestination = false;
        autoPathingScript.GetToCurrentCutscenePoint(numberOfCorrectInteractions);
        playerAutoWalkingStarts = true;

        //yield return new WaitUntil(() => autoPathingScript.hasReachedDestination && cinemachineControllerScript.hasFinishedTransitionBlending);
        yield return new WaitUntil(() => cinemachineControllerScript.hasFinishedTransitionBlending);

        //guy stop walking
        autoPathingGuy.navMeshAgent.enabled = false;
        autoPathingGuy.loopHasStarted = false;
        ResetObjects();
        Debug.Log("Coroutine wurde gestoppt für den Loop");
        
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine); 
        }
        
        playerAutoWalkingStarts = false;

        Debug.Log("Laufen und Drehen wurde alles abgeschlossen");
        //Set correct Video
        videoPlayer.clip = preSave;

        // Play video
        Debug.Log("Playing the video");
        videoRawImageGameObject.SetActive(true);
        fadeVideoIn = true;
        alpha = 0;

        StartCoroutine(SkipCutsceneUI(skipCutsceneUI, 6, 5));

        StartCoroutine(CloseVideo());
    }

    private bool CheckSeenAllMemories()
    {
        for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
        {
            if (!erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().erinnerungsObjektAngeschaut)
            {
                return false;
            }
        }

        return true;
    }

    private void StopTimerAndActivateDoorInteraction()
    {
        audioController.StopBackgroundMusic();
        //if interact with door --> end game (in PlayerInteractions.cs)
        Interactables[3].GetComponent<BoxCollider>().enabled = true;
    }

    public void EndGame()
    {
        Debug.Log("start endcutscene");
        //wird über PlayerInteractions aufgerufen
        //startet 4. cutscene
        FreezePlayer(true);
        cinemachineControllerScript.EndTransition();
        StartCoroutine(EndDoorTransition());
    }

    IEnumerator EndDoorTransition()
    {
        yield return new WaitForSeconds(2f);
        videoPlayer.clip = LoopCutscenes[3];
        videoRawImageGameObject.SetActive(true);
        fadeVideoIn = true;
        alpha = 0;
        StartCoroutine(CloseEndcutscene());
    }

    private void UpdateCurrentLoop()
    {
        int tempCurrentLoop = 1;

        if (Interactables[0].GetComponent<Interactable>().isInFinalPosition)
        {
            tempCurrentLoop = 2;

            if (Interactables[1].GetComponent<Interactable>().isInFinalPosition)
            {
                tempCurrentLoop = 3;

                if (Interactables[2].GetComponent<Interactable>().isInFinalPosition)
                {
                    interactedWithAllThreeObjects = true;
                }
            }
        }

        if (tempCurrentLoop > currentLoop)
        {
            currentLoop = tempCurrentLoop;
        }
    }

    IEnumerator CloseVideo()
    {
        videolength = (float) videoPlayer.length;
        currentlyCutscenePlaying = true;
        yield return new WaitUntil(() => !currentlyCutscenePlaying);

        cutsceneSkipable = false;

        //Reset Player + Objects
        player.transform.position = startPlayerPositionEachLoop;
        player.transform.rotation = startPlayerRotationEachLoop;
        Debug.Log("Set player rotation, player.transform.rotation = " + player.transform.rotation +
                  "startPlayerRotation = " + startPlayerRotationEachLoop);
        cinemachineControllerScript.ResetAllCameraSettings();
        //Reset Guy
        autoPathingGuy.ResetGuyBackToStartofLoop();

        videoPlayer.targetTexture.Release();
        if (currentLoop == 0)
        {
            currentLoop++;
        }

        videoPlayer.clip = null;
        StartCoroutine(CurrentStartCutscene());
    }

    IEnumerator CloseEndcutscene()
    {
        videolength = (float) videoPlayer.length;
        currentlyCutscenePlaying = true;
        yield return new WaitUntil(() => !currentlyCutscenePlaying);

        SceneManager.LoadScene(3);
    }

    IEnumerator CurrentStartCutscene()
    {
        StartCoroutine(SkipCutsceneUI(skipCutsceneUI, 6, 5));
        // Play video
        videoPlayer.clip = StartCutscenes[currentLoop];
        videolength = (float) videoPlayer.length;
        currentlyCutscenePlaying = true;
        yield return new WaitUntil(() => !currentlyCutscenePlaying);
        // reset video player
        videoRawImageGameObject.SetActive(false);
        videoPlayer.targetTexture.Release();
        videoPlayer.clip = null;
        FadeOutBlackScreen();

        cutsceneSkipable = false;

        // start loop again
        FreezePlayer(false);
        EnableInteractables();
        StartLoop();
        autoPathingGuy.StartWalkCycle();
        audioController.RestartBackgroundMusic();

        player.transform.rotation = startPlayerRotationEachLoop;

        yield return new WaitForSeconds(7);
        if (voicelineplayer.isPlaying)
        {
            yield return new WaitUntil(() => !voicelineplayer.isPlaying);
        }
        
        if (withinAGamePlayLoop)
        {
            voicelineController.SetCurrentVoiceline(numberOfCorrectInteractions.ToString() + "Interaktion");
            StartCoroutine(PlayRandomInteraktionsobjektVoiceLineDuringLoop());
        }

        yield return new WaitForSeconds(10);
        if (voicelineplayer.isPlaying)
        {
            yield return new WaitUntil(() => !voicelineplayer.isPlaying);
        }

        if (withinAGamePlayLoop)
        {
            voicelineController.SetCurrentVoiceline(currentLoop.ToString() + "Erinnerung");
        }
    }

    private VideoClip ReturnVideoClip()
    {
        numberOfCorrectInteractions = 0;
        VideoClip lastVideoClip = LoopCutscenes[0];

        foreach (var interactable in Interactables)
        {
            if (interactable.gameObject.activeSelf)
            {
                if (interactable.GetComponent<Interactable>().isInFinalPosition)
                {
                    numberOfCorrectInteractions++;
                    Debug.Log("Number of correct interactions: " + numberOfCorrectInteractions);
                    lastVideoClip = LoopCutscenes[numberOfCorrectInteractions];
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        Debug.Log("lastVideoClip = " + lastVideoClip);
        return lastVideoClip;
    }


    private void EnableInteractables()
    {
        Debug.Log("currentLoop - 1 = " + (currentLoop - 1));
        Interactables[currentLoop - 1].gameObject.GetComponent<Interactable>().enabled = true;
        Interactables[currentLoop - 1].gameObject.transform.GetChild(3).gameObject.SetActive(true);
        erinnerungsObjekte.transform.GetChild(currentLoop).GetComponent<Erinnerungsobjekt>().enabled = true;
    }

    private void FreezePlayer(bool freeze)
    {
        if (freeze)
        {
            player.GetComponent<FirstPersonMovement>().enabled = false;
            player.transform.GetChild(0).GetComponent<FirstPersonLook>().enabled = false;
        }
        else
        {
            player.GetComponent<FirstPersonMovement>().enabled = true;
            player.transform.GetChild(0).GetComponent<FirstPersonLook>().enabled = true;
        }
    }


    private void ResetObjects()
    {
        foreach (var interactable in Interactables)
        {
            if (interactable.gameObject.activeSelf)
            {
                if (interactable.CompareTag("Door") || !interactable.GetComponent<Interactable>().enabled)
                {
                    break;
                }

                interactable.GetComponent<Interactable>().ResetObjectToPickUpPosition();
            }
        }
    }

    private void FadeOutBlackScreen()
    {
        fadeBlackscreenOut = true;
    }


    private bool CheckAllTaskDoneLoop()
    {
        amountRightInteractables = 0f;
        // check all interactables
        for (int i = 0; i < Interactables.Count - 1; i++)
        {
            if (Interactables[i].GetComponent<Interactable>().enabled)
            {
                if (!Interactables[i].GetComponent<Interactable>().isInFinalPosition &&
                    !Interactables[i].GetComponent<Interactable>().impossibleToInteract)
                {
                    return false;
                }

                if (Interactables[i].GetComponent<Interactable>().isInFinalPosition)
                {
                    amountRightInteractables++;
                }
            }
        }

        // check all memory objects
        for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
        {
            if (erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().enabled)
            {
                if (!erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>()
                    .erinnerungsObjektAngeschaut)
                {
                    return false;
                }
            }
        }

        return true;
    }

    IEnumerator DoorVoiceline(bool enabled)
    {
        if (currentlyCutscenePlaying)
        {
            yield break;
        }

        justPlayed = true;
        if (enabled)
        {
            voicelineController.SetCurrentVoiceline("DoorInteractionEnabled");
        }
        else if (!enabled)
        {
            if (!firstTimeNeedMemories)
            {
                if (voicelineplayer.isPlaying)
                {
                    yield return new WaitUntil(() => !voicelineplayer.isPlaying);
                }

                voicelineController.SetCurrentVoiceline("DoorInteractionNeedsMemories");
                firstTimeNeedMemories = true;
            }
            else
            {
                for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
                {
                    if (!erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>()
                        .erinnerungsObjektAngeschaut)
                    {
                        voicelineController.SetCurrentVoiceline(i.ToString() + "Erinnerung");
                        break;
                    }
                }
            }
        }

        yield return new WaitForSeconds(20);
        justPlayed = false;
    }

    IEnumerator SkipCutsceneUI(GameObject cutsceneUI, float timeBeforeUI, float enabledForSeconds)
    {
        yield return new WaitForSeconds(timeBeforeUI);
        cutsceneSkipable = true;

        //cutsceneStarted = true;
        if (currentlyCutscenePlaying)
        {
            cutsceneUI.SetActive(true);
        }

        yield return new WaitForSeconds(enabledForSeconds);
        cutsceneUI.SetActive(false);
    }

    IEnumerator PlayRandomInteraktionsobjektVoiceLineDuringLoop()
    {
        Debug.Log("PlayRandomInteraktionsObjektVOicelineDuringLoop Couroutine aufgerufen.");
        yield return new WaitForSeconds(35f);

        Debug.Log("PlayRandomInteraktionsObjektVOicelineDuringLoop 35sek um.");

        if (!withinAGamePlayLoop || startEnding)
        {
            yield break;
        }

        lastRoutine = StartCoroutine(PlayRandomInteraktionsobjektVoiceLineDuringLoop());
        
        for (int i = 0; i < Interactables.Count - 1; i++)
        {
            if (Interactables[i].GetComponent<Interactable>().enabled)
            {
                if (!Interactables[i].GetComponent<Interactable>().isInFinalPosition)
                {
                    Debug.Log("folgende voiceline sollte spielen" + i.ToString() + "Interaktion");
                    voicelineController.SetCurrentVoiceline(i.ToString() + "Interaktion");
                    yield break;
                }
            }
        }

        for (int i = 0; i < erinnerungsObjekte.transform.childCount; i++)
        {
            if (erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().enabled)
            {
                if (!erinnerungsObjekte.transform.GetChild(i).GetComponent<Erinnerungsobjekt>()
                    .erinnerungsObjektAngeschaut)
                {
                    voicelineController.SetCurrentVoiceline(i.ToString() + "Erinnerung");
                    yield break;
                }
            }
        }
    }
}