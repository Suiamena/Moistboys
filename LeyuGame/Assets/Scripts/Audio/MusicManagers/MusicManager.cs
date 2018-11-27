using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    [Header("fmod parameters")]
    public float sound;

    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public string launch = "event:/Dragon/Launch";
    public FMOD.Studio.EventInstance Music;
    public FMOD.Studio.EventInstance Launch;

    public FMOD.Studio.ParameterInstance MusicParameter;
    public FMOD.Studio.ParameterInstance LaunchParameter;

    public int musicStage;

    private void Awake()
    {
        //FMOD SETUP
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);
        Launch.getParameter("Launch", out LaunchParameter);
        Music.start();
        Launch.start();

        //WAKE UP
        //sound = 0.5f;
    }

    private void FixedUpdate()
    {
        RegulateMusic();
        MusicParameter.setValue(sound);
    }

    void RegulateMusic()
    {
        if (musicStage == 1)
        {
            //BOUNCE TUTORIAL
            //sound = 2.5f;

            // laat los van loading bar
            LaunchParameter.setValue(1f);
            Debug.Log("ey");
        }
        if (musicStage == 2)
        {
            //MEET CREATURE
            sound = 3.5f;

            //laad loading bar weer
            LaunchParameter.setValue(0f);
        }
    }
}
