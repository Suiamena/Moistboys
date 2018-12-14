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

    bool runCoroutineOnce;

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
        Debug.Log(musicStage);
        MusicParameter.setValue(musicStage);

        if (!runCoroutineOnce)
        {
            StartCoroutine(PlayCompetentMusic());
            runCoroutineOnce = true;
        }
    }

    IEnumerator PlayCompetentMusic()
    {
        musicStage = 6.5f;
        yield return new WaitForSeconds(3F);
        musicStage = 7.5f;
    }

}
