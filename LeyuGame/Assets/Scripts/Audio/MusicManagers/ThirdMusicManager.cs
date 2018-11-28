using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdMusicManager : MonoBehaviour
{

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
    }

    private void FixedUpdate()
    {
        MusicParameter.setValue(sound);
        sound += Mathf.Lerp(0f, 5.5f, 0.01f);
        sound = Mathf.Clamp(sound, 0, 5.5f);
    }

}