using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    [Header("fmod parameters")]
    public float sound, launchSound;

    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public string launch = "event:/Dragon/Launch";
    public FMOD.Studio.EventInstance Music;
    public FMOD.Studio.EventInstance Launch;

    public FMOD.Studio.ParameterInstance MusicParameter;
    public FMOD.Studio.ParameterInstance LaunchParameter;

    public int musicStage;
    bool playTutorialSound, playCreatureSound, playCoroutineOnce;

    private void Awake()
    {
        //FMOD SETUP
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Launch = FMODUnity.RuntimeManager.CreateInstance(launch);
        Music.getParameter("Music", out MusicParameter);
        Launch.getParameter("Launch", out LaunchParameter);
        Music.start();
        //Launch.start();

        //WAKE UP
        sound = 0.5f;
    }

    private void FixedUpdate()
    {
        RegulateMusic();
        MusicParameter.setValue(sound);
        LaunchParameter.setValue(launchSound);

        if (!playCoroutineOnce)
        {
            StartCoroutine(LaunchEffect());
            playCoroutineOnce = true;
        }
    }

    void RegulateMusic()
    {
        if (musicStage == 1)
        {
            if (!playTutorialSound)
            {
                //BOUNCE TUTORIAL
                sound = 2.5f;
                playTutorialSound = true;
            }
        }
        if (musicStage == 2)
        {
            if (!playCreatureSound)
            {
                //MEET CREATURE
                sound = 3.5f;
                playCreatureSound = true;
            }
        }
    }

    IEnumerator LaunchEffect()
    {
        Launch.start();
        //LOAD LAUNCH
        launchSound = 0f;
        yield return new WaitForSeconds(1F);
        //FREE LAUNCH
        launchSound = 1f;
    }
}
