using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneStorm : MonoBehaviour {

    //Thomas Creature Shit
    GameObject creature;
    GameObject creatureBeweging;
    Vector3 distanceToDraak;
    Animator creatureAnim;
    Animator creatureBewegingAnim;

    bool creatureMoving = false;
    //Thomas Shit

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

        //Thomas
        creature = GameObject.Find("SneeuwStormCreature");
        creatureBeweging = GameObject.Find("BewegingSneeuwstorm");
        creatureAnim = creature.GetComponent<Animator>();
        creatureAnim.SetBool("isFlying", true);
        creatureBewegingAnim = creatureBeweging.GetComponent<Animator>();
        creatureBewegingAnim.SetBool("isPlaying", false);
        //Thomas
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

        distanceToDraak = player.transform.position - creature.transform.position;
        distanceToDraak = new Vector3(distanceToDraak.x, distanceToDraak.y, distanceToDraak.z);
        print(distanceToDraak);

        if (creatureMoving == true && distanceToDraak.x > -15f)
        {
            creatureBewegingAnim.enabled = true;
        }
        else
        {
            creatureBewegingAnim.enabled = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            creatureBewegingAnim.SetBool("isPlaying", true);
            creatureMoving = true;
            followPlayer = true;
            storm.SetActive(true);
            startToAccelerate = true;
        }
    }
}
