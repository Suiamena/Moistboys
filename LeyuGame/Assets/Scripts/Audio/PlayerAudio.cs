﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

    //MUSIC AND SOUND MANAGEMENT
    bool launchSoundStarted, playBuildLaunch, playExecuteLaunch;
    bool playBounceOnce;

    //PLAYER
    GameObject player;
    GameObject launchParticleTransform;
    PlayerController playerScript;

    //LAUNCH
    string launch = "event:/Dragon/Launch";
    static FMOD.Studio.EventInstance Launch;
    static FMOD.Studio.ParameterInstance LaunchParameter;

    //BOUNCE
    static string bounce = "event:/Dragon/Bounce";
    static FMOD.Studio.EventInstance Bounce;
    static FMOD.Studio.ParameterInstance HeightParameter;
    static FMOD.Studio.ParameterInstance GroundParameter;

    static float launchStage;

    static float heightStage;
    static float groundStage;

    public GameObject launchParticles;

    private void Awake()
    {
        //PLAYER SETUP
        player = GameObject.Find("Character");
        launchParticleTransform = GameObject.Find("LandingIndicator");
        playerScript = player.GetComponent<PlayerController>();

        //LAUNCH SETUP
        Launch = FMODUnity.RuntimeManager.CreateInstance(launch);
        Launch.getParameter("Launch", out LaunchParameter);

        //BOUNCE SETUP
        Bounce = FMODUnity.RuntimeManager.CreateInstance(bounce);
        Bounce.getParameter("Height", out HeightParameter);
        Bounce.getParameter("Ground", out GroundParameter);
    }

    private void Update()
    {
        PlayBounce();
        PlayLaunch();

        //LaunchParameter.setValue(launchStage);
        //HeightParameter.setValue(heightStage);
        //GroundParameter.setValue(groundStage);
    }

    void PlayBounce()
    {
        if (!playerScript.playerIsAirborne)
        {
            if (!playBounceOnce)
            {
                groundStage = 1f;
                heightStage = 0f;
                HeightParameter.setValue(heightStage);
                GroundParameter.setValue(groundStage);
                Bounce.start();
                playBounceOnce = true;
            }
        }
        else
        {
            playBounceOnce = false;
        }
    }

    void PlayLaunch()
    {
        //BUILD LAUNCH POWER
        if (playerScript.isBuildingLaunch && !playBuildLaunch)
        {
            launchStage = 0f;
            LaunchParameter.setValue(launchStage);
            if (!launchSoundStarted)
            {
                Launch.start();
                launchSoundStarted = true;
            }
            playBuildLaunch = true;
            playExecuteLaunch = false;
        }
        //LAUNCH IN THE AIR
        if (playerScript.isPreLaunching && !playExecuteLaunch)
        {
            Instantiate(launchParticles, launchParticleTransform.transform.position, Quaternion.Euler(90, 0, 0));
            launchStage = 1f;
            LaunchParameter.setValue(launchStage);
            Launch.start();
            playExecuteLaunch = true;
            playBuildLaunch = false;
        }
    }

}
