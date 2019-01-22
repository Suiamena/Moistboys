using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowStormParticles : MonoBehaviour {

    GameObject playerCamera;
    Vector3 cameraDesiredPosition;

    private void Awake()
    {
        playerCamera = GameObject.Find("Character");
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y + 8, playerCamera.transform.position.z);
    }

}
