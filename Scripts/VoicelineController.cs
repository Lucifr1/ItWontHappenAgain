using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class VoicelineController : MonoBehaviour
{
    // Voiceline references
    // Er
    private AudioClip[] denyVoicelinesErinnerungsobjektEr;
    private AudioClip[] denyVoicelinesInteraktionsobjektEr;
    private AudioClip[] emiliaHerrufenWaeschekorb;
    private AudioClip[] emiliaHerrufenWeinflasche;
    private AudioClip[] emiliaHerrufenFernbedienung;

    // Sie
    private AudioClip[] aufrufErinnernHandy;
    private AudioClip[] aufrufErinnernMesser;
    private AudioClip[] aufrufErinnernBlumenvase;
    private AudioClip[] aufrufErinnernKette;
    private AudioClip[] aufrufWegraeumenWaeschekorb;
    private AudioClip[] aufrufWegraeumenWeinflasche;
    private AudioClip[] aufrufWegraeumenFernbedienung;
    private AudioClip[] denyInteraktionSie;
    private AudioClip[] kannNochNichtHerausgehen;
    private AudioClip[] kannHerausgehen;

    private AudioClip randomizedVoiceline;
    private AudioSource audioPlayer;

    // Memory objects
    [SerializeField] private GameObject memoryObjects;

    //Subtitles
    private Subtitles subtitles;

    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();

        // Set all voiceline references
        denyVoicelinesErinnerungsobjektEr = Resources.LoadAll<AudioClip>("Voiceline/Er/Deny/Erinnerungsobjekt");
        denyVoicelinesInteraktionsobjektEr = Resources.LoadAll<AudioClip>("Voiceline/Er/Deny/Interaktionsobjekt");
        emiliaHerrufenWaeschekorb = Resources.LoadAll<AudioClip>("Voiceline/Er/Emilia/Cutscene Waeschekorb herrufen");
        emiliaHerrufenWeinflasche = Resources.LoadAll<AudioClip>("Voiceline/Er/Emilia/Cutscene Weinflasche herrufen");
        emiliaHerrufenFernbedienung =
            Resources.LoadAll<AudioClip>("Voiceline/Er/Emilia/Cutscene Fernbedienung herrufen");
        aufrufErinnernHandy = Resources.LoadAll<AudioClip>("Voiceline/Sie/Handy");
        aufrufErinnernMesser = Resources.LoadAll<AudioClip>("Voiceline/Sie/Messer");
        aufrufErinnernBlumenvase = Resources.LoadAll<AudioClip>("Voiceline/Sie/Blumenvase");
        aufrufErinnernKette = Resources.LoadAll<AudioClip>("Voiceline/Sie/Kette");
        aufrufWegraeumenWaeschekorb = Resources.LoadAll<AudioClip>("Voiceline/Sie/Waeschekorb");
        aufrufWegraeumenWeinflasche = Resources.LoadAll<AudioClip>("Voiceline/Sie/Weinflasche");
        aufrufWegraeumenFernbedienung = Resources.LoadAll<AudioClip>("Voiceline/Sie/Fernbedienung");
        denyInteraktionSie = Resources.LoadAll<AudioClip>("Voiceline/Sie/Deny bei Anschauen");
        kannNochNichtHerausgehen =
            Resources.LoadAll<AudioClip>("Voiceline/Sie/Kann noch nicht herausgehen Erinnerungen");
        kannHerausgehen = Resources.LoadAll<AudioClip>("Voiceline/Sie/Kann zur Tuere rausgehen");

        subtitles = GameObject.FindGameObjectWithTag("Subtitle").GetComponent<Subtitles>();
    }


    // Die Methode in den unterschiedlichen Skripten aufrufen und dann den Typ der Voiceline referenzieren,
    // dann wird eine randomized ausgewählt aus dem bestimmten Pool
    public void SetCurrentVoiceline(string voicelineTyp)
    {
        Debug.Log(voicelineTyp);
        if (audioPlayer.isPlaying)
        {
            Debug.Log("Please wait");
            return;
        }

        switch (voicelineTyp)
        {
            case "DenyInteractionSie":
                subtitles.StartSubtitles("Ich sollte das nicht machen, wenn er in der Nähe ist...", 5f);
                randomizedVoiceline = denyInteraktionSie[Random.Range(0, denyInteraktionSie.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            case "DenyInteraktionEr":
                subtitles.StartSubtitles("Tom: Lass das.", 3f);
                randomizedVoiceline =
                    denyVoicelinesInteraktionsobjektEr[Random.Range(0, denyVoicelinesInteraktionsobjektEr.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            case "DenyErinnerungEr":
                /*
                subtitles.StartSubtitles("Tom: Lass das.", 3f);
                randomizedVoiceline =
                    denyVoicelinesErinnerungsobjektEr[Random.Range(0, denyVoicelinesErinnerungsobjektEr.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                */
                break;
            // First cutscene voiceline
            case "0":
                subtitles.StartSubtitles("Tom: Emilia? Hast du die Wäsche etwa nicht gemacht?", 5f);
                randomizedVoiceline = emiliaHerrufenWaeschekorb[Random.Range(0, emiliaHerrufenWaeschekorb.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            // Second cutscene voiceline
            case "1":
                subtitles.StartSubtitles("Tom: Emilia?", 3f);
                randomizedVoiceline = emiliaHerrufenWeinflasche[Random.Range(0, emiliaHerrufenWeinflasche.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            // Third cutscene voiceline
            case "2":
                subtitles.StartSubtitles("Tom: Emilia?", 3f);
                randomizedVoiceline = emiliaHerrufenFernbedienung[Random.Range(0, emiliaHerrufenFernbedienung.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            // First memory object enabled
            case "0Erinnerung":
                randomizedVoiceline = aufrufErinnernHandy[Random.Range(0, aufrufErinnernHandy.Length)];
                if (!memoryObjects.transform.GetChild(0).GetComponent<Erinnerungsobjekt>()
                    .erinnerungsObjektAngeschaut && !AnyMemoryCutsceneIsPlaying())
                {
                    subtitles.StartSubtitles("Ich sollte mein Handy finden...", 5f);
                    PlayCurrentVoiceline(randomizedVoiceline);
                }

                break;
            // Second memory object enabled
            case "1Erinnerung":
                randomizedVoiceline = aufrufErinnernMesser[Random.Range(0, aufrufErinnernMesser.Length)];
                if (!memoryObjects.transform.GetChild(1).GetComponent<Erinnerungsobjekt>()
                    .erinnerungsObjektAngeschaut && !AnyMemoryCutsceneIsPlaying())
                {
                    subtitles.StartSubtitles("Ich sollte das Messer nicht so offen rumliegen lassen...", 5f);
                    PlayCurrentVoiceline(randomizedVoiceline);
                }

                break;
            // Third memory object enabled
            case "2Erinnerung":
                randomizedVoiceline = aufrufErinnernBlumenvase[Random.Range(0, aufrufErinnernBlumenvase.Length)];
                if (!memoryObjects.transform.GetChild(2).GetComponent<Erinnerungsobjekt>()
                    .erinnerungsObjektAngeschaut && !AnyMemoryCutsceneIsPlaying())
                {
                    subtitles.StartSubtitles("Was ist eigentlich mit den Blumen die er mir letztens geschenkt hat?",
                        5f);
                    PlayCurrentVoiceline(randomizedVoiceline);
                }

                break;
            // Fourth memory object enabled
            case "3Erinnerung":
                randomizedVoiceline = aufrufErinnernKette[Random.Range(0, aufrufErinnernKette.Length)];
                if (!memoryObjects.transform.GetChild(3).GetComponent<Erinnerungsobjekt>()
                    .erinnerungsObjektAngeschaut && !AnyMemoryCutsceneIsPlaying())
                {
                    subtitles.StartSubtitles("Er wird bestimmt sauer, wenn ich die Kette von ihm nicht trage...", 5f);
                    PlayCurrentVoiceline(randomizedVoiceline);
                }

                break;
            // First cutscene object enabled
            case "0Interaktion":
                subtitles.StartSubtitles("Wenn Ich den Wäschekorb wegstelle, sollte nichts passieren...", 5f);
                randomizedVoiceline = aufrufWegraeumenWaeschekorb[Random.Range(0, aufrufWegraeumenWaeschekorb.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            // Second cutscene object enabled
            case "1Interaktion":
                subtitles.StartSubtitles("Ich sollte die Weinflasche wegräumen, bevor er noch mehr trinken kann...",
                    5f);
                randomizedVoiceline = aufrufWegraeumenWeinflasche[Random.Range(0, aufrufWegraeumenWeinflasche.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            // Third cutscene object enabled
            case "2Interaktion":
                subtitles.StartSubtitles(
                    "Wenn ich die Fernbedienung wegräume, kann es erst gar nicht so weit kommen...", 5f);
                randomizedVoiceline =
                    aufrufWegraeumenFernbedienung[Random.Range(0, aufrufWegraeumenFernbedienung.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            case "DoorInteractionNeedsMemories":
                subtitles.StartSubtitles("Ich muss mich noch allen Erinnerungen stellen, bevor ich so weit bin.", 5f);
                randomizedVoiceline = kannNochNichtHerausgehen[Random.Range(0, kannNochNichtHerausgehen.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            case "DoorInteractionEnabled":
                subtitles.StartSubtitles("Ich bin endlich bereit zu gehen...", 5f);
                randomizedVoiceline = kannHerausgehen[Random.Range(0, kannHerausgehen.Length)];
                PlayCurrentVoiceline(randomizedVoiceline);
                break;
            default:
                Debug.Log("Something went wrong");
                break;
        }
    }

    private void PlayCurrentVoiceline(AudioClip current)
    {
        Debug.Log(current);
        audioPlayer.clip = current;
        audioPlayer.Play();
    }

    private bool AnyMemoryCutsceneIsPlaying()
    {
        for (int i = 0; i < memoryObjects.transform.childCount; i++)
        {
            if (memoryObjects.transform.GetChild(i).GetComponent<Erinnerungsobjekt>().enabled)
            {
                if (memoryObjects.transform.GetChild(i).GetComponent<Erinnerungsobjekt>()
                    .isPlaying)
                {
                    return true;
                }
            }
        }

        return false;
    }
}