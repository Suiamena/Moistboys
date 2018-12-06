using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2MusicManager : MonoBehaviour
{

    //MUSIC AND SOUND MANAGEMENT
    [Header("Management")]
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
    }

    private void FixedUpdate()
    {
        RegulateMusic();
        PlayLaunch();
    }

    void RegulateMusic()
    {
        DecemberAudio.musicStage += Mathf.Lerp(0f, 4.5f, 0.005f);
        DecemberAudio.musicStage = Mathf.Clamp(DecemberAudio.musicStage, 0, 4.5f);
    }

    void PlayLaunch()
    {
        //BUILD LAUNCH POWER
        if (playerScript.isBuildingLaunch && !playBuildLaunch)
        {
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
