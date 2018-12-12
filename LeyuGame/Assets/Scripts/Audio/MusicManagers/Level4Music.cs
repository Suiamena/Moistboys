using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4Music : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public static FMOD.Studio.EventInstance Music;
    public static FMOD.Studio.ParameterInstance MusicParameter;

    public static float musicStage;

    private void Awake()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);
        musicStage = 9.5f;

        AmbienceManager.windStage = 0.40f;
        AmbienceManager.insideStage = 0f;

        //AmbienceManager.amethystStage = 0f;
        //AmbienceManager.amethystStage = 0.5f;
        //AmbienceManager.amethystStage = 1f;

        Music.start();
    }

    private void Update()
    {
        MusicParameter.setValue(musicStage);
    }

}
