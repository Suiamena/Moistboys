using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoundary : MonoBehaviour {

    //VECTOR CALCULATION
    public GameObject objectA;
    public GameObject objectB;

    Vector3 a;
    Vector3 b;
    Vector3 c;
    Vector3 side1;
    Vector3 side2;
    Vector3 pushDirection;

    //PLAYER
    GameObject player;
    PlayerController playerScript;

    //WIND SETTINGS
    public float windStrength;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();

        //CALCULATE PENDICULAR
        a = objectA.transform.position;
        b = objectB.transform.position;
        c = new Vector3(0, 0, 0);

        side1 = b - a;
        side2 = c - a;

        pushDirection = Vector3.Cross(side1, side2).normalized;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript.enablePlayerPushBack = true;

            windStrength = 1;
            print(windStrength);
            pushDirection = pushDirection * windStrength;
            playerScript.boundaryPushingDirection = pushDirection;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript.enablePlayerPushBack = false;
        }
    }

}
