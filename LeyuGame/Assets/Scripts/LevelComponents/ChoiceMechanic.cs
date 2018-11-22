using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceMechanic : MonoBehaviour {

    GameObject player;
    GameObject moustacheBoiCutscene;
    GameObject moustacheBoiTarget;
    GameObject playerCamera;
    GameObject cutsceneCamera;

    PlayerController playerScript;
    Rigidbody playerRig;

    bool cutsceneRunning;

    int moustacheBoiSpeed = 10;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        moustacheBoiCutscene = GameObject.Find("MoustacheBoiCutscene");
        moustacheBoiTarget = GameObject.Find("MoustacheBoiTarget");
        playerCamera = GameObject.Find("Main Camera");
        cutsceneCamera = GameObject.Find("CutsceneCamera");

        cutsceneCamera.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (cutsceneRunning)
        {
            StartCutscene();
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            cutsceneRunning = true;
        }
    }

    void StartCutscene()
    {
        playerRig.velocity = new Vector3(0, 0, 0);
        playerScript.enabled = false;
        playerCamera.SetActive(false);
        moustacheBoiCutscene.transform.position = Vector3.MoveTowards(moustacheBoiCutscene.transform.position, moustacheBoiTarget.transform.position, moustacheBoiSpeed * Time.deltaTime);
        Debug.Log("start cutscene");
    }
}
