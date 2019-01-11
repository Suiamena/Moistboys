using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTriggerWall : MonoBehaviour {

    public GameObject wallScriptObject;
    PlangaMuur wallScript;

    private void Awake()
    {
        wallScript = wallScriptObject.GetComponent<PlangaMuur>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            wallScript.StartJump();
        }
    }

}
