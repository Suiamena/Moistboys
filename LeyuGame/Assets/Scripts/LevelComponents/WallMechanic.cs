using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMechanic : MonoBehaviour
{

    [Header("Camera Settings")]
    public GameObject cam;
    public float cameraXOffset, cameraYOffset, cameraZOffset, cameraYRotation;
    public float cameraSpeed, cameraRotationSpeed;
    float desiredCameraYRotation;
    float cameraX, cameraY, cameraZ;
    Quaternion cameraRotation;

    [Header("Player")]
    public GameObject player;
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
    bool enableSequence, sequenceIsRunning, fuckingBoolean, coroutineRunning;

    public GameObject pressAObject;

    private void Awake()
    {
        //playerScript = player.GetComponent<CameraPlayerInProgress>();
        playerRig = player.GetComponent<Rigidbody>();
        cameraXOffset = -2f;
        cameraYOffset = 1f;
        cameraZOffset = 10f;
        cameraYRotation = 10f;
        playerJumpSpeed = 50;
        playerLerpSpeed = 5;
        cameraSpeed = 10f;
        cameraRotationSpeed = 3f;
        desiredCameraYRotation = transform.rotation.y;
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
            platformsObject.SetActive(true);
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
                playerIsJumping = true;
                if (platformsJumped == 0)
                {
                    StartSequence();
                }
            }
        }
    }

    void StartSequence()
    {
        playerScript.enabled = false;
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
        sequenceIsRunning = false;
        enableSequence = false;
        platformsJumped = 0;
    }

    //Make the camera follow the player
    void SeparateCamera()
    {
        if (sequenceIsRunning)
        {
            cameraX = Mathf.Lerp(cam.transform.eulerAngles.x, 0, cameraRotationSpeed * Time.deltaTime);
            cameraY = Mathf.Lerp(cam.transform.eulerAngles.y, desiredCameraYRotation, cameraRotationSpeed * Time.deltaTime);
            cameraZ = Mathf.Lerp(cam.transform.eulerAngles.z, 0, cameraRotationSpeed * Time.deltaTime);

            cam.transform.position = new Vector3(Mathf.Lerp(cam.transform.position.x, player.transform.position.x + cameraXOffset, cameraSpeed * Time.deltaTime), Mathf.Lerp(cam.transform.position.y, player.transform.position.y + cameraYOffset, cameraSpeed * Time.deltaTime), Mathf.Lerp(cam.transform.position.z, player.transform.position.z - cameraZOffset, cameraSpeed * Time.deltaTime));
            //cameraRotation = Quaternion.Euler(0, cameraY, 0);
            //cameraRotation = Quaternion.Euler(0, 100, 0);
            //cam.transform.rotation = cameraRotation;
            //cam.transform.rotation = Quaternion.Euler(0, 100, 0);
            cam.transform.rotation = player.transform.rotation;
        }
    }

    IEnumerator WaitForNextJump()
    {
        yield return new WaitForSeconds(0.5f);
        fuckingBoolean = true;
    }
}
