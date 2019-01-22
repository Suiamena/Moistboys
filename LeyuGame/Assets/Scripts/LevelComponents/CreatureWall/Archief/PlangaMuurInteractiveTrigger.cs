using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Creature;

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
        if (other.gameObject.tag == "Player")
        {
            //wallScript.StartJump();
        }
    }

}
