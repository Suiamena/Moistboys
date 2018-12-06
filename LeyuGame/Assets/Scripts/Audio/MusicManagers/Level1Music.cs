using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Music : MonoBehaviour {

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
                DecemberAudio.musicStage = 3.5f;
                playCreatureSound = true;
            }
        }
    }

    void PlayLaunch()
    {
        //BUILD LAUNCH POWER
        if (playerScript.isBuildingLaunch && !playBuildLaunch) {
            DecemberAudio.launchStage = 0f;
            if (!launchSoundStarted)
            {
                DecemberAudio.Launch.start();
                launchSoundStarted = true;
            }
            playBuildLaunch = true;
            playExecuteLaunch = false;
        }
        //LAUNCH IN THE AIR
        if (playerScript.isPreLaunching && !playExecuteLaunch)
        {
            Instantiate(launchParticles, launchParticleTransform.transform.position, Quaternion.Euler(90, 0, 0));
            DecemberAudio.launchStage = 1f;
            DecemberAudio.Launch.start();
            playExecuteLaunch = true;
            playBuildLaunch = false;
        }
    }
}
