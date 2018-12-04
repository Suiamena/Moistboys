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
    Vector3 appliedPushDirection;

    //PLAYER
    GameObject player;
    PlayerController playerScript;

    //WIND SETTINGS
    public float windStrengthAcceleration;
    float windStrength;

    //MANAGER
    bool coroutineStarted;

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
            windStrength += windStrengthAcceleration;
            appliedPushDirection = pushDirection * windStrength;
            playerScript.boundaryPushingDirection = appliedPushDirection;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
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

}
