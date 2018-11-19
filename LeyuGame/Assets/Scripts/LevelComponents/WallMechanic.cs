using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMechanic : MonoBehaviour
{

    [Header("Camera Settings")]
    public GameObject camAnchor;
    public float cameraSpeed, cameraRotationSpeed;

    [Header("Player")]
    public GameObject player;
    public GameObject playerCam;
    Vector3 playerPositionLerp;
    public int playerJumpSpeed, playerLerpSpeed;
    bool playerIsJumping;

    PlayerController playerScript;
    Rigidbody playerRig;

    [Header("Platform Settings")]
    public GameObject platformsObject;
    public List<GameObject> platforms = new List<GameObject>();
    Vector3 playerMovementTarget;
    int platformsJumped;
    public int maxPlatformsAmount;

    //Sequence Manager
    bool enableSequence, sequenceIsRunning, fuckingBoolean, coroutineRunning, creatureSpawnedPlatforms;
       
    public GameObject pressAObject, creatureAnim;

    private void Awake()
    {
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerJumpSpeed = 50;
        playerLerpSpeed = 5;
        cameraSpeed = 10f;
    }

    private void Update()
    {
        JumpInput();
        SeparateCamera();
        JumpExecution();

        if (!enableSequence)
        {
            if (!sequenceIsRunning)
            {
                platformsObject.SetActive(false);
                pressAObject.SetActive(false);
                enableSequence = false;
            }
        }

    }


    //Detect wether or not the player can start the sequence
    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "Player")
        {
            pressAObject.SetActive(true);
            enableSequence = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            if (!sequenceIsRunning)
            {
                enableSequence = false;
            }
        }
    }


    //Perform jumps from platform to platform
    void JumpInput()
    {
        if (Input.GetButtonDown("A Button"))
        {
            if (enableSequence && !playerIsJumping)
            {
                if (creatureSpawnedPlatforms)
                {
                    playerIsJumping = true;
                    if (platformsJumped == 0)
                    {
                        StartSequence();
                    }
                }
                else
                {
                    StartCoroutine(CreatureDoesTrick());
                    creatureSpawnedPlatforms = true;
                    platformsObject.SetActive(true);
                    playerScript.enabled = false;

                    //CUTSCENE CAMERA
                    //camAnchor.transform.position = new Vector3(Mathf.Lerp(camAnchor.transform.position.x, 50, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.y, 50, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.z, 50, cameraSpeed * Time.deltaTime));
                    //camAnchor.transform.LookAt(platforms[0].transform);
                }
            }
        }
    }

    void StartSequence()
    {
        playerCam.SetActive(false);
        camAnchor.SetActive(true);
        sequenceIsRunning = true;
    }

    void JumpExecution()
    {
        if (playerIsJumping)
        {
            //player.transform.eulerAngles = new Vector3(0, 0, 0);
            player.transform.rotation = transform.rotation;
            playerMovementTarget = platforms[platformsJumped].transform.position;
            playerPositionLerp = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, playerMovementTarget.y, playerLerpSpeed * Time.deltaTime), player.transform.position.z);
            player.transform.position = Vector3.MoveTowards(playerPositionLerp, playerMovementTarget, playerJumpSpeed * Time.deltaTime);
            //if (player.transform.position == playerMovementTarget)
            if (!coroutineRunning)
            {
                StartCoroutine(WaitForNextJump());
                coroutineRunning = true;
            }
            if (fuckingBoolean)
            {
                playerRig.velocity = new Vector3(0, 0, 0);
                platformsJumped += 1;
                playerIsJumping = false;
                fuckingBoolean = false;
                coroutineRunning = false;
                if (platformsJumped == maxPlatformsAmount)
                {
                    EndSequence();
                }
            }
        }
    }

    void EndSequence()
    {
        playerScript.enabled = true;
        playerCam.SetActive(true);
        camAnchor.SetActive(false);
        sequenceIsRunning = false;
        enableSequence = false;
        creatureSpawnedPlatforms = false;
        platformsJumped = 0;
    }

    //Make the camera follow the player
    void SeparateCamera()
    {
        if (sequenceIsRunning)
        {
            camAnchor.transform.position = new Vector3(Mathf.Lerp(camAnchor.transform.position.x, player.transform.position.x, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.y, player.transform.position.y, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.z, player.transform.position.z, cameraSpeed * Time.deltaTime));
            camAnchor.transform.rotation = player.transform.rotation;
        }
    }

    IEnumerator WaitForNextJump()
    {
        yield return new WaitForSeconds(0.5f);
        fuckingBoolean = true;
    }

    IEnumerator CreatureDoesTrick()
    {
        creatureAnim.SetActive(true);
        yield return new WaitForSeconds(1F);
        creatureAnim.SetActive(false);
    }
}
