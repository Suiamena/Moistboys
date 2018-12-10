using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Music : MonoBehaviour {

    [Header("fmod Parameters")]

    // BACKGROUND MUSIC
    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public static FMOD.Studio.EventInstance Music;
    public static FMOD.Studio.ParameterInstance MusicParameter;

    public static float musicStage;

    //MUSIC AND SOUND MANAGEMENT
    [Header("Management")]
    public int countMusicStage;
    bool playTutorialSound, playCreatureSound;

    //PLAYER
    GameObject player;
    GameObject launchParticleTransform;
    PlayerController playerScript;

    private void Awake()
    {
        //PLAYER
        player = GameObject.Find("Character");
        launchParticleTransform = GameObject.Find("LandingIndicator");
        playerScript = player.GetComponent<PlayerController>();

        //WAKE UP
        //PlaySound.musicStage = 0.5f;

        //music
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Music.getParameter("Music", out MusicParameter);
        musicStage = 2.5f;

        //START MUSIC AND AMBIENCE
        Music.start();
    }

    private void FixedUpdate()
    {
        MusicParameter.setValue(musicStage);
        //RegulateMusic();
    }

    void RegulateMusic()
    {
        //BOUNCE TUTORIAL
        if (countMusicStage == 1) {
            if (!playTutorialSound) {
                PlaySound.musicStage = 2.5f;
                playTutorialSound = true;
            }
        }
        //MEET CREATURE
        if (countMusicStage == 2) {
            if (!playCreatureSound) {
                PlaySound.musicStage = 3.5f;
                playCreatureSound = true;
            }
        }
    }

}
