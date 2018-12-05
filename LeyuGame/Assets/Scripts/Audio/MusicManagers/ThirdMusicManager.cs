using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdMusicManager : MonoBehaviour
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
    bool abilityGot;

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
        if (!abilityGot)
        {
            sound += Mathf.Lerp(0f, 6.5f, 0.005f);
            sound = Mathf.Clamp(sound, 0, 6.5f);
        }
        if (sound == 6.5f && sound < 7.5f)
        {
            abilityGot = true;
            StartCoroutine(PlayCompetentMusic());
        }
    }

    IEnumerator PlayCompetentMusic()
    {
        yield return new WaitForSeconds(3F);
        sound += Mathf.Lerp(0f, 7.5f, 0.005f);
        sound = Mathf.Clamp(sound, 0, 7.5f);
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
