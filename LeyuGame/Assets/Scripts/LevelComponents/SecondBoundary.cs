using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBoundary : MonoBehaviour {

    //PLAYER
    GameObject player;
    PlayerController playerScript;
    Rigidbody playerRig;

    float startingAirborneVelocity;
    Vector3 startingVelocity;

    bool playerInBoundary, startCoroutine;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();

        startingVelocity = playerScript.leapingVelocity;
        startingAirborneVelocity = playerScript.airborneMovementSpeed;
    }

    private void FixedUpdate()
    {
        if (!playerInBoundary)
        {
            CheckPlayerInBoundary();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerScript.enableBoundaryPushBack)
            {
                //player is airborne
                playerScript.airborneMovementSpeed -= 0.1f;
                playerScript.airborneMovementSpeed = Mathf.Clamp(playerScript.airborneMovementSpeed, 1, playerScript.airborneMovementSpeed);

                playerScript.leapingVelocity.z -= 0.1f;
                playerScript.leapingVelocity.z = Mathf.Clamp(playerScript.leapingVelocity.z, 1, playerScript.leapingVelocity.z);
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

    private void OnTriggerExit(Collider other)
    {
        playerScript.airborneMovementSpeed = startingAirborneVelocity;
        playerScript.leapingVelocity = startingVelocity;
        playerScript.boundaryPushingDirection = new Vector3(0, 0, 0);
        playerScript.inBoundary = false;
        playerInBoundary = false;
        Debug.Log("out");
    }

    void CheckPlayerInBoundary()
    {
        playerScript.inBoundary = false;
    }

    IEnumerator PushBackPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        if (!playerScript.enableBoundaryPushBack)
        {
            playerScript.boundaryPushingDirection = new Vector3(0, 0, -10);
            playerScript.inBoundary = true;
        }
        startCoroutine = false;
    }

}
