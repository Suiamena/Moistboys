using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Music : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public static FMOD.Studio.EventInstance Music;
    public static FMOD.Studio.ParameterInstance MusicParameter;

    public static float musicStage;

    public static bool startMusic;

    bool startCoroutineOnce;

    private void Awake()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);

        AmbienceManager.windStage = 0.40f;
        AmbienceManager.insideStage = 0f;

        AmbienceManager.amethystStage = 1f;

        Music.start();
    }

    private void Update()
    {
        MusicParameter.setValue(musicStage);

        if (startMusic)
        {
            musicStage = 7.5f;
            startMusic = false;
        }
    }

    IEnumerator DelayMusic()
    {
        yield return new WaitForSeconds(1f);
        //musicStage = 7.5f;
    }

}
