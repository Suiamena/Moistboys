using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnchor : MonoBehaviour {

    public GameObject player;
    CameraPlayerInProgress playerScript;

    float cameraYAngle;

    private void Awake()
    {
        playerScript = player.GetComponent<CameraPlayerInProgress>();
    }

    private void Update()
    {
        transform.position = player.transform.position;
        if (playerScript.disableRotation)
        {
            cameraYAngle = Input.GetAxis("Right Stick X") * 2;
            transform.eulerAngles = transform.eulerAngles - new Vector3(0, cameraYAngle, 0);
        }
    }
}
