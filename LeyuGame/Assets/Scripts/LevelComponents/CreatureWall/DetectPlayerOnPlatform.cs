using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerOnPlatform : MonoBehaviour {

    public GameObject wallObject;
    PlangeMuurInteractive wallScript;
    public bool playerOnPlatform;

    private void Awake()
    {
        wallScript = wallObject.GetComponent<PlangeMuurInteractive>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!playerOnPlatform)
            {
                playerOnPlatform = true;
                wallScript.NewPlatform(playerOnPlatform);
            }
        }
    }

}
