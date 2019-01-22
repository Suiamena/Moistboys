using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowParticles : MonoBehaviour {

    GameObject playerCamera;

    private void Awake()
    {
        playerCamera = GameObject.Find("Main Camera");
    }

    private void FixedUpdate()
    {
        transform.position = new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y + 8, playerCamera.transform.position.z);
    }

}
