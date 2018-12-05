using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondMusicManager : MonoBehaviour
{

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
    bool launchSoundStarted;
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

        //PLAYER
        player = GameObject.Find("Character");
        launchParticleTransform = GameObject.Find("LandingIndicator");
        playerScript = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        PlayLaunch();
        MusicParameter.setValue(sound);
        sound += Mathf.Lerp(0f, 4.5f, 0.005f);
        sound = Mathf.Clamp(sound, 0, 4.5f);
    }

    void PlayLaunch()
    {
        //BUILD LAUNCH POWER
        if (playerScript.isBuildingLaunch && !playBuildLaunch)
        {
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
