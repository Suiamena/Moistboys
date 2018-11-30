using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBoundary : MonoBehaviour
{

    //PLAYER
    GameObject player;
    PlayerController playerScript;

    //MANAGEMENT
    bool playerInBoundary;

    public GameObject kube;

    [Header("Boundary Settings")]
    public float windAcceleration;
    float windForce;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        kube.transform.position = transform.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            windForce += windAcceleration;
            windForce = Mathf.Clamp(windForce, 0, 2);

            if (playerScript.playerIsAirborne)
            {
                //player is airborne
                playerScript.enablePlayerPushBack = false;
            }
            else
            {
                //player is grounded
                Vector3 windDirection = Quaternion.Inverse(transform.rotation) * (player.transform.position - transform.position);
                playerScript.boundaryPushingDirection = new Vector3(windDirection.x, 0, windDirection.z).normalized * windForce;
                playerScript.enablePlayerPushBack = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            windForce = 0;
            playerScript.enablePlayerPushBack = false;
        }
    }

}
