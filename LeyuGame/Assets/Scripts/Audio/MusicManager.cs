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

    int progress;

    private void Awake()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);
        Music.start();


        //na het ontwaken
        sound = 0.5f;
        //sound = 7.5f;
    }

    private void FixedUpdate()
    {
        float curValue = 0.0f;
        Debug.Log(progress);
        progress += 1;

        RegulateMusic();
        MusicParameter.setValue(sound);

       
        print(sound);
    }

    void RegulateMusic()
    {
        Debug.Log("et");
        if (progress == 500)
        {
            //tutorial bounce
            sound = 2.5f;
        }

        if (progress == 1000)
        {
            //creature met
            sound = 3.5f;
        }

        if (progress == 2000)
        {
            //creature uses ability
            sound = 4.5f;
        }

        if (progress == 3000)
        {
            //player gets ability
            sound = 5.5f;
        }
    }

}
