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

    public GameObject level4Ambience;

    private void Awake()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);

        AmbienceManager.windStage = 0.40f;
        AmbienceManager.insideStage = 1f;

        AmbienceManager.ToggleAmbience();

        AmbienceManager.amethystStage = 1f;

        Music.start();
    }

    private void Update()
    {
        MusicParameter.setValue(musicStage);
    }

}
