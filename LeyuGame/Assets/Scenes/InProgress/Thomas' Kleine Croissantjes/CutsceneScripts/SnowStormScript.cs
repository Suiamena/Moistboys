using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SnowStormScript : MonoBehaviour {

    public Image image;
    bool FadingToWhite = false;

    Color tempColor;

    float windStormStrength, particlesSpeed;

    bool accelerateSnowstorm;
    bool screechPlayed = false;
    
    GameObject creature;
    AudioSource creatureScreech;
    public AudioClip screech;

    [Header("Particle Settings")]
    public GameObject snowParticlesWindObject;
    ParticleSystem snowParticlesSystem;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.MainModule main;

    void Start ()
    {
        creature = GameObject.Find("SneeuwStormCreature");
        creatureScreech = creature.GetComponent<AudioSource>();

        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;

        snowParticlesSystem = snowParticlesWindObject.GetComponent<ParticleSystem>();
        emissionModule = snowParticlesSystem.emission;
        main = snowParticlesSystem.main;
    }

    void OnTriggerEnter ()
    {
        FadingToWhite = true;
        accelerateSnowstorm = true;
    }
	
	void Update () {
        if (accelerateSnowstorm)
        {
            windStormStrength += 100F;
            windStormStrength = Mathf.Clamp(windStormStrength, 5000, 20000);
            emissionModule.rateOverTime = windStormStrength;

            particlesSpeed += 0.2f;
            particlesSpeed = Mathf.Clamp(particlesSpeed, 10, 40);
            main.startSpeed = particlesSpeed;
        }
        if (FadingToWhite == true)
        {
            var tempColor = image.color;
            tempColor.a += 0.0028f;
            image.color = tempColor;
        }

        if (image.color.a > 0.41f && screechPlayed == false)
        {
            creatureScreech.PlayOneShot(screech);
            screechPlayed = true;
        }

        if (image.color.a >= 1)
        {
            StartCoroutine(CutsceneTime());
        }

    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(1f);
        print("hallo?");
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("Level 3");
    }
}
