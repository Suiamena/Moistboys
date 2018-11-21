using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour {

    public GameObject moustacheBoi;
    public int creatureRange = 10;
    public GameObject player;

    private void FixedUpdate()
    {
        CheckDistance();
    }

    void CheckDistance()
    {
        //FOLLOW THE PLAYER
        if (Vector3.Distance(moustacheBoi.transform.position, player.transform.position) < creatureRange)
        {
            Debug.Log("start");
        }
    }

}
