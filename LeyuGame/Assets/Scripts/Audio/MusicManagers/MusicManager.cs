using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    //FMOD SETUP
    [Header("FMOD Parameters")]
    public float launchSound;

    [Header("FMOD Event References")]
    [FMODUnity.EventRef]
    public string launch = "event:/Dragon/Launch";
    public FMOD.Studio.EventInstance Launch;

    public FMOD.Studio.ParameterInstance LaunchParameter;

    //MUSIC AND SOUND MANAGEMENT
    [Header("Management")]
    public int countMusicStage;
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
        Launch = FMODUnity.RuntimeManager.CreateInstance(launch);
        Launch.getParameter("Launch", out LaunchParameter);

        //PLAYER
        player = GameObject.Find("Character");
        launchParticleTransform = GameObject.Find("LandingIndicator");
        playerScript = player.GetComponent<PlayerController>();

        //WAKE UP
        DecemberAudio.musicStage = 0.5f;
    }

    private void FixedUpdate()
    {
        RegulateMusic();
        PlayLaunch();
    }

    void RegulateMusic()
    {
        //BOUNCE TUTORIAL
        if (countMusicStage == 1) {
            if (!playTutorialSound) {
                DecemberAudio.musicStage = 2.5f;
                playTutorialSound = true;
            }
        }
        //MEET CREATURE
        if (countMusicStage == 2) {
            if (!playCreatureSound) {
                DecemberAudio.musicStage = 2.5f;
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
