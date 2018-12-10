using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6Music : MonoBehaviour
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
        musicStage = 13.5f;

        AmbienceManager.windStage = 0f;
        AmbienceManager.areaStage = 1f;

        //AmbienceManager.amethystStage = 0f;
        //AmbienceManager.amethystStage = 0.5f;
        //AmbienceManager.amethystStage = 1f;

        //FINAL MUSIC TO DO:
        //kies voor creature: 13.5
        //kies voor competence: 14.5

        Music.start();
    }

    private void Update()
    {
        MusicParameter.setValue(musicStage);
    }

}
