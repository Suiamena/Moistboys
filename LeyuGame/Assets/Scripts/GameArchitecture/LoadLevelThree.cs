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
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level3-rough_lenny");
            //if (!coroutineRunning)
            //{
            //    StartCoroutine(AvalancheFalls());
            //    coroutineRunning = true;
            //}

            //if (coroutineRunning)
            //{
            //    if (!avalancheFall)
            //    {
            //        CreatureMoving();
            //    }
            //    else
            //    {
            //        AvalancheFalling();
            //    }
            //}
        }
    }

   // void CreatureMoving()
   // {
   //     creature.transform.position = Vector3.MoveTowards(creature.transform.position, creatureTransformTarget.transform.position, creatureMovingSpeed * Time.deltaTime);
   //}

   // void AvalancheFalling()
   // {
   //     avalanche.transform.position = Vector3.MoveTowards(avalanche.transform.position, avalancheTransformTarget.transform.position, avalancheFallingSpeed * Time.deltaTime);
   // }

   // IEnumerator AvalancheFalls()
   // {
   //     playerCamera.SetActive(false);
   //     cutsceneCamera.SetActive(true);
   //     player.transform.position = new Vector3(player.transform.position.x, avalancheTransformTarget.transform.position.y, player.transform.position.z);
   //     playerAnim.SetBool("IsLaunching", false);
   //     playerAnim.SetBool("IsBouncing", false);
   //     playerRig.velocity = new Vector3(0, 0, 0);
   //     playerScript.enabled = false;
   //     yield return new WaitForSeconds(5F);
   //     avalancheFall = true;
   //     yield return new WaitForSeconds(3F);
   //     SceneManager.LoadScene("Level3_rough");
   // }

}
