using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoundary : MonoBehaviour {

    [Header("Wall Endpoints")]
    public GameObject objectA;
    public GameObject objectB;

    [Header("Particle Settings")]
    public GameObject snowParticlesObject;
    public GameObject snowParticlesWindObject;
    ParticleSystem snowParticlesSystem;
    ParticleSystem.EmissionModule emissionModule;
    ParticleSystem.MainModule main;
    public int maxParticlesSpeed;
    public int maxParticlesAmount;
    float particlesSpeed;
    float particlesAmount;

    [Header("Wind Settings")]
    public float windStrengthAcceleration;
    public float maxWindStrength;
    float windStrength;

    //PENDICULAR CALCULATION
    Vector3 a;
    Vector3 b;
    Vector3 c;
    Vector3 side1;
    Vector3 side2;
    Vector3 pushDirection;
    Vector3 appliedPushDirection;

    //PLAYER
    GameObject player;
    PlayerController playerScript;

    //MANAGER
    bool snowSpawned, snowDespawned;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();

        snowParticlesSystem = snowParticlesWindObject.GetComponent<ParticleSystem>();
        emissionModule = snowParticlesSystem.emission;
        main = snowParticlesSystem.main;

        //CALCULATE PENDICULAR
        a = objectA.transform.position;
        b = objectB.transform.position;
        c = new Vector3(0, 0, 0);

        side1 = b - a;
        side2 = c - a;

        pushDirection = Vector3.Cross(side1, side2).normalized;
		pushDirection.y = 0;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!playerScript.enablePlayerPushBack)
            {
                StartCoroutine(IncreaseWindStrength());
                playerScript.enablePlayerPushBack = true;
            }

            if (!snowSpawned)
            {
                StartCoroutine(SpawnSnowParticles());
                snowSpawned = true;
            }
            snowDespawned = false;

            //WIND FORCE
            windStrength += windStrengthAcceleration;
            windStrength = Mathf.Clamp(windStrength, 0, maxWindStrength);
            appliedPushDirection = pushDirection * windStrength;
            playerScript.boundaryPushingDirection = appliedPushDirection;

            //PARTICLES
            particlesSpeed = windStrength;
            particlesSpeed = Mathf.Clamp(particlesSpeed, 0, maxParticlesSpeed);
            main.startSpeed = particlesSpeed;

            particlesAmount = 100 * windStrength;
            particlesAmount = Mathf.Clamp(particlesAmount, 0, maxParticlesAmount);
            emissionModule.rateOverTime = particlesAmount;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            windStrength = 0;
            playerScript.enablePlayerPushBack = false;
            snowSpawned = false;

            if (!snowDespawned)
            {
                StartCoroutine(DecreaseWindStrength());
                snowDespawned = true;
            }
        }
    }

    IEnumerator IncreaseWindStrength()
    {
        for (int i = 0; i < 500; i++)
        {
            windStrength += windStrengthAcceleration;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator DecreaseWindStrength()
    {
        for (int i = 0; i < 100; i++)
        {
            //PARTICLES
            particlesAmount = particlesAmount / 2;
            particlesAmount = Mathf.Clamp(particlesAmount, 0, maxParticlesAmount);
            emissionModule.rateOverTime = particlesAmount;
            Debug.Log(particlesAmount);
            if (particlesAmount < 500)
            {
                snowParticlesObject.SetActive(true);
            }
            if (particlesAmount < 1)
            {
                snowParticlesWindObject.SetActive(false);
                particlesAmount = 0;
                main.startSpeed = 10;
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SpawnSnowParticles()
    {
        snowParticlesWindObject.SetActive(true);
        yield return new WaitForSeconds(1F);
        snowParticlesObject.SetActive(false);
    }

}
