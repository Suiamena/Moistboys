using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBoundary : MonoBehaviour {

    //PLAYER
    GameObject player;
    PlayerController playerScript;
    Rigidbody playerRig;

    [Header("Boundary Settings")]
    public int windStrength;

    //STARTING MOVEMENT SPEED
    float startingAirborneVelocity;
    Vector3 startingVelocity;

    //MANAGEMENT
    bool startCoroutine, playerInBoundary;
    bool pushbackForceAdded, pushbackForceSubtracted;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();

        startingVelocity = playerScript.leapingVelocity;
        startingAirborneVelocity = playerScript.airborneMovementSpeed;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //playerInBoundary = true;
            //playerScript.enablePlayerPushBack = true;

            if (playerScript.playerIsAirborne)
            {
                //player is airborne
                DeaccelerateSpeed();
            }
            else
            {
                //player is grounded
                if (!startCoroutine)
                {
                    //StartCoroutine(PushBackPlayer());
                    startCoroutine = true;
                }
            }
        }
    }

    void DeaccelerateSpeed()
    {
        playerScript.airborneMovementSpeed -= 0.1f;
        playerScript.airborneMovementSpeed = Mathf.Clamp(playerScript.airborneMovementSpeed, 1, playerScript.airborneMovementSpeed);

        playerScript.leapingVelocity.z -= 0.1f;
        playerScript.leapingVelocity.z = Mathf.Clamp(playerScript.leapingVelocity.z, 1, playerScript.leapingVelocity.z);
    }

    private void OnTriggerExit(Collider other)
    {
        //RESET SPEED
        playerScript.airborneMovementSpeed = startingAirborneVelocity;
        playerScript.leapingVelocity = startingVelocity;
    //    playerScript.boundaryPushingDirection = new Vector3(0, 0, 0);

    //    //MANAGEMENT
    //    playerInBoundary = false;
    //    startCoroutine = false;
    //    playerScript.enablePlayerPushBack = false;
    //    StopCoroutine(PushBackPlayer());
    //}

    //IEnumerator PushBackPlayer()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    if (!playerScript.playerIsAirborne && playerInBoundary)
    //    {
    //        playerScript.boundaryPushingDirection = new Vector3(windStrength, 0, 0);
    //        playerScript.enablePlayerPushBack = true;
    //    }
    //    else
    //    {
    //        playerScript.enablePlayerPushBack = false;
        //}
        //startCoroutine = false;
    }

}
