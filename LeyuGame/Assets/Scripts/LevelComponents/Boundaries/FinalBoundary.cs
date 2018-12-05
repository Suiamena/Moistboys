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
    ParticleSystem.MainModule main;
    public int maxParticlesSpeed;
    float particlesSpeed;

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
    bool coroutineStarted;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();

        snowParticlesSystem = snowParticlesWindObject.GetComponent<ParticleSystem>();
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

            snowParticlesWindObject.SetActive(true);

            //WIND FORCE
            windStrength += windStrengthAcceleration;
            windStrength = Mathf.Clamp(windStrength, 0, maxWindStrength);
            appliedPushDirection = pushDirection * windStrength;
            playerScript.boundaryPushingDirection = appliedPushDirection;

            //PARTICLES
            particlesSpeed = windStrength;
            particlesSpeed = Mathf.Clamp(particlesSpeed, 0, maxParticlesSpeed);
            main.startSpeed = particlesSpeed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            snowParticlesObject.SetActive(true);
            snowParticlesWindObject.SetActive(false);

            main.startSpeed = 10;
            windStrength = 0;
            playerScript.enablePlayerPushBack = false;
        }
    }

    IEnumerator IncreaseWindStrength()
    {
        for (int i = 0; i < 100; i++)
        {
            windStrength += windStrengthAcceleration;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SpawnSnowParticles()
    {
        yield return new WaitForSeconds(1F);
        snowParticlesObject.SetActive(false);
    }

}
