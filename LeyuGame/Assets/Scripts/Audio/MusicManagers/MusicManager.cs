using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    //FMOD SETUP
    [Header("FMOD Parameters")]
    public float sound;
    public float launchSound;

    [Header("FMOD Event References")]
    [FMODUnity.EventRef]
    public string music = "event:/Music";
    public string launch = "event:/Dragon/Launch";
    public FMOD.Studio.EventInstance Music;
    public FMOD.Studio.EventInstance Launch;

    public FMOD.Studio.ParameterInstance MusicParameter;
    public FMOD.Studio.ParameterInstance LaunchParameter;

    //MUSIC AND SOUND MANAGEMENT
    [Header("Management")]
    public int musicStage;
    bool launchSoundStarted;
    bool playTutorialSound, playCreatureSound;
    bool playBuildLaunch, playExecuteLaunch;

    //PLAYER
    GameObject player;
    GameObject launchParticleTransform;
    PlayerController playerScript;

    public GameObject launchParticles;

    private void Awake()
    {
        //FMOD SETUP
        Music = FMODUnity.RuntimeManager.CreateInstance(music);
        Launch = FMODUnity.RuntimeManager.CreateInstance(launch);
        Music.getParameter("Music", out MusicParameter);
        Launch.getParameter("Launch", out LaunchParameter);
        Music.start();

        //PLAYER
        player = GameObject.Find("Character");
        launchParticleTransform = GameObject.Find("LandingIndicator");
        playerScript = player.GetComponent<PlayerController>();

        //WAKE UP
        sound = 0.5f;
    }

    private void FixedUpdate()
    {
        RegulateMusic();
        PlayLaunch();
        MusicParameter.setValue(sound);
    }

    void RegulateMusic()
    {
        //BOUNCE TUTORIAL
        if (musicStage == 1) {
            if (!playTutorialSound) {
                sound = 2.5f;
                playTutorialSound = true;
            }
        }
        //MEET CREATURE
        if (musicStage == 2) {
            if (!playCreatureSound) {
                sound = 3.5f;
                playCreatureSound = true;
            }
        }
    }

    void PlayLaunch()
    {
        //BUILD LAUNCH POWER
        if (playerScript.isBuildingLaunch && !playBuildLaunch) {
            launchSound = 0f;
            if (!launchSoundStarted)
            {
                Launch.start();
                launchSoundStarted = true;
            }
            LaunchParameter.setValue(launchSound);
            playBuildLaunch = true;
            playExecuteLaunch = false;
        }
        //LAUNCH IN THE AIR
        if (playerScript.isPreLaunching && !playExecuteLaunch)
        {
            Instantiate(launchParticles, launchParticleTransform.transform.position, Quaternion.Euler(90, 0, 0));
            launchSound = 1f;
            //Launch.start();
            LaunchParameter.setValue(launchSound);
            playExecuteLaunch = true;
            playBuildLaunch = false;
        }
    }
}
