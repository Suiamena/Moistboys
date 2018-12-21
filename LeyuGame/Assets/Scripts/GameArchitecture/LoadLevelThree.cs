using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelThree : MonoBehaviour
{
    public int avalancheFallingSpeed = 10, creatureMovingSpeed = 5;

    bool coroutineRunning, avalancheFall;

    GameObject creature;
    GameObject creatureTransformTarget;
    GameObject avalanche;
    GameObject avalancheTransformTarget;

    GameObject player;
    GameObject playerModel;
    GameObject playerCamera;
    PlayerController playerScript;
    Rigidbody playerRig;
    Animator playerAnim;

    public GameObject cutsceneCamera;

    private void Awake()
    {
        creature = GameObject.Find("Creature");
        creatureTransformTarget = GameObject.Find("CreatureTarget");
        avalanche = GameObject.Find("MOD_Lawine");
        avalancheTransformTarget = GameObject.Find("AvalancheTarget");

        player = GameObject.Find("Character");
        playerModel = GameObject.Find("MOD_Draak");
        playerCamera = GameObject.Find("Main Camera");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerAnim = playerModel.GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level 3");
        }
    }

}
