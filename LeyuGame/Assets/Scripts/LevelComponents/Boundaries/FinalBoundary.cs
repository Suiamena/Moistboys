using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoundary : MonoBehaviour {

    //VECTOR CALCULATION
    public GameObject objectA;
    public GameObject objectB;

    Vector3 a;
    Vector3 b;
    Vector3 origin;
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

        //pendicular = Vector3.Cross(b, a);

        origin = b - a;

        pendicular = Vector3.Cross(origin, Vector3.up.normalized);

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
        playerScript.enablePlayerPushBack = false;
    }

}
