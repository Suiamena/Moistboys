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
    Vector3 pendicular;

    //PLAYER
    GameObject player;
    PlayerController playerScript;

    bool playCoroutine;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        a = objectA.transform.position;
        b = objectB.transform.position;
        c = new Vector3(0, 0, 0);

        side1 = b - a;
        side2 = c - a;

        pendicular = Vector3.Cross(side1, side2).normalized;
        pendicular = pendicular * 10;

        if (!playCoroutine)
        {
            StartCoroutine(Move());
            playCoroutine = true;
        }
    }

    IEnumerator Move()
    {
        Debug.Log(pendicular);
        playerScript.enablePlayerPushBack = true;
        playerScript.boundaryPushingDirection = pendicular;
        yield return new WaitForSeconds(0.5F);
        //playerScript.enablePlayerPushBack = false;
    }

}
