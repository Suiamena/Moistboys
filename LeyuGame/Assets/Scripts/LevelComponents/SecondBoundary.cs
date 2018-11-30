using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBoundary : MonoBehaviour {

    //PLAYER
    GameObject player;
    PlayerController playerScript;
    Rigidbody playerRig;

    //STARTING MOVEMENT SPEED
    float startingAirborneVelocity;
    Vector3 startingVelocity;

    //MANAGEMENT
    bool startCoroutine;

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
            if (playerScript.enableBoundaryPushBack)
            {
                //player is airborne
                DeaccelerateSpeed();
            }
            else
            {
                //player is grounded
                if (!startCoroutine)
                {
                    StartCoroutine(PushBackPlayer());
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
        playerScript.boundaryPushingDirection = new Vector3(0, 0, 0);

        //MANAGEMENT
        startCoroutine = false;
        playerScript.inBoundary = false;
    }

    IEnumerator PushBackPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        playerScript.boundaryPushingDirection = new Vector3(0, 0, -10);
        playerScript.inBoundary = true;
    }

}
