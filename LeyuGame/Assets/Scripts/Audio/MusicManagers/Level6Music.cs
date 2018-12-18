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

        AmbienceManager.windStage = 0f;
        AmbienceManager.insideStage = 0f;
        AmbienceManager.areaStage = 1f;

        AmbienceManager.amethystStage = 0.5f;

        if (VariablesGlobal.chosenForCompetence)
        {
            musicStage = 14.5f;
        }

        if (VariablesGlobal.chosenForSocial)
        {
            musicStage = 13.5f;
        }

        Music.start();
    }

    private void Update()
    {
        MusicParameter.setValue(musicStage);
    }

}
