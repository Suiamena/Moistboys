using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlangaMuurInteractiveTrigger : MonoBehaviour
{

    public GameObject wallScriptObject;
    PlangeMuurInteractive wallScript;

    private void Awake()
    {
        wallScript = wallScriptObject.GetComponent<PlangeMuurInteractive>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("duh");
        if (other.gameObject.tag == "Player")
        {
            wallScript.StartJump();
        }
    }

}
