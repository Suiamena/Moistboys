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

        Music.start();
    }

    private void Update()
    {
        MusicParameter.setValue(musicStage);
    }

}
