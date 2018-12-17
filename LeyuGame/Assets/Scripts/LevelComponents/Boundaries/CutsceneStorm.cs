using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneStorm : MonoBehaviour {

    GameObject player;

    float windStormStrength, particlesSpeed;

    [Header("Particle Settings")]
    public GameObject snowParticlesWindObject;
    ParticleSystem snowParticlesSystem;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.MainModule main;

    private void Awake()
    {
        player = GameObject.Find("Character");

        snowParticlesSystem = snowParticlesWindObject.GetComponent<ParticleSystem>();
        emissionModule = snowParticlesSystem.emission;
        main = snowParticlesSystem.main;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            transform.position = player.transform.position;

            windStormStrength += 50f;
            windStormStrength = Mathf.Clamp(windStormStrength, 5000, 20000);
            emissionModule.rateOverTime = windStormStrength;

            particlesSpeed += 0.2f;
            particlesSpeed = Mathf.Clamp(particlesSpeed, 10, 40);
            main.startSpeed = particlesSpeed;
        }
    }

}
