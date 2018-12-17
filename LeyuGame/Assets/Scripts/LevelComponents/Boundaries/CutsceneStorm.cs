using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneStorm : MonoBehaviour {

    public GameObject storm;
    GameObject player;

    bool followPlayer, startToAccelerate;

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

    private void Update()
    {
        if (followPlayer)
        {
            storm.transform.position = new Vector3(player.transform.position.x + 10, player.transform.position.y, player.transform.position.z);
        }
        if (startToAccelerate)
        {
            Debug.Log("hay");
            windStormStrength += 100F;
            windStormStrength = Mathf.Clamp(windStormStrength, 0, 5000);
            emissionModule.rateOverTime = windStormStrength;

            if (windStormStrength == 5000)
            {
                startToAccelerate = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            followPlayer = true;
            storm.SetActive(true);
            startToAccelerate = true;
        }
    }

}
