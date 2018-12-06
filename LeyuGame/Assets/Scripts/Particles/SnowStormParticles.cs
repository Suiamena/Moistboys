using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStormParticles : MonoBehaviour {

    GameObject playerCamera;

    private void Awake()
    {
        playerCamera = GameObject.Find("Main Camera");
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(playerCamera.transform.position.x + 25, playerCamera.transform.position.y + 1, playerCamera.transform.position.z);
    }

}
