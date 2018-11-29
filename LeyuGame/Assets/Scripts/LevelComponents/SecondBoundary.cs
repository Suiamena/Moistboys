using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBoundary : MonoBehaviour {

    //PLAYER
    GameObject player;
    PlayerController playerScript;
    Rigidbody playerRig;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript.inBoundary = true;
            playerScript.boundaryPushingDirection = new Vector3(100, 0, 0);
        }
    }

}
