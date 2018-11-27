using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    [Header("fmod parameters")]
    public float sound;

    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public FMOD.Studio.EventInstance Music;

    public FMOD.Studio.ParameterInstance MusicParameter;

    public int musicStage;

    private void Awake()
    {
        //FMOD SETUP
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);
        Music.start();

        //WAKE UP
        sound = 0.5f;
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
            sound = 2.5f;
        }
        if (musicStage == 2)
        {
            //MEET CREATURE
            sound = 3.5f;
        }
    }

}
