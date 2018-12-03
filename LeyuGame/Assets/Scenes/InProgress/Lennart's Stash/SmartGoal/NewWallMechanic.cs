using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour {

    //PLAYER
    GameObject player;
    GameObject playerModel;
    PlayerController playerScript;
    Rigidbody playerRig;
    Animator playerAnim;

    Vector3 playerPositionLerp;
    Vector3 playerMovementTarget;
    Vector3 playerDistanceToPlatform;

    public int playerLerpSpeed;
    public int playerJumpSpeed;

    //CREATURE
    GameObject moustacheBoi;
    Animator moustacheAnim;

    //UI
    public GameObject pressButtonPopup;

    [Header("Camera Settings")]
    public GameObject sequenceCamera;

    public float cameraXOffset, cameraYOffset, cameraZOffset;

    //MANAGER
    bool enableSequence, creatureSpawnsPlatforms, sequenceIsRunning, playerIsJumping;
    int platformsReached;

    [Header("Platform Settings")]
    public GameObject platformsObject;
    public List<GameObject> platforms = new List<GameObject>();

    [Header("Settings")]
    public int triggerAbilityRange = 10;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerModel = GameObject.Find("MOD_Draak");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerAnim = playerModel.GetComponent<Animator>();

        moustacheBoi = GameObject.Find("Creature");
        moustacheAnim = moustacheBoi.GetComponent<Animator>();

        playerJumpSpeed = 50;
        playerLerpSpeed = 5;
    }

    private void FixedUpdate()
    {
        Debug.Log(platformsReached);
        TriggerSequence();
        StartSequence();
        StartJump();
        MakeJump();
    }

    void TriggerSequence()
    {
        if (Vector3.Distance(moustacheBoi.transform.position, player.transform.position) < triggerAbilityRange)
        {
            if (!creatureSpawnsPlatforms)
            {
                pressButtonPopup.SetActive(true);
            }
            enableSequence = true;
        }
        else
        {
            pressButtonPopup.SetActive(false);
            enableSequence = false;
        }
    }

    void StartSequence()
    {
        if (Input.GetButtonDown("A Button") && enableSequence && !creatureSpawnsPlatforms)
        {
            //DISABLE PLAYER ANIMATION
            playerAnim.SetBool("IsBouncing", false);
            playerAnim.SetBool("IsLaunching", false);
            playerAnim.SetBool("IsAirborne", false);

            //DISABLE PLAYER MOVEMENT
            player.transform.position = new Vector3(player.transform.position.x, moustacheBoi.transform.position.y, playerRig.transform.position.z);
            playerScript.enabled = false;
            playerRig.velocity = new Vector3(0, 0, 0);

            //SHOW CAMERA
            sequenceCamera.SetActive(true);
            sequenceCamera.transform.rotation = player.transform.rotation;
            sequenceCamera.transform.position = player.transform.position;
            sequenceCamera.transform.position = sequenceCamera.transform.rotation * new Vector3(0, 0, 10);

            sequenceCamera.transform.LookAt(platforms[0].transform);

            //SPAWN OBJECTS
            creatureSpawnsPlatforms = true;
            StartCoroutine(CreatureDoesTrick());
        }
    }

    void StartJump()
    {
        if (Input.GetButtonDown("A Button") && sequenceIsRunning && !playerIsJumping)
        {
            playerIsJumping = true;
        }
    }

    void MakeJump()
    {
        if (playerIsJumping)
        {
            sequenceCamera.transform.LookAt(player.transform);
            player.transform.LookAt(platforms[platformsReached].transform);

            playerMovementTarget = platforms[platformsReached].transform.position;
            playerPositionLerp = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, playerMovementTarget.y, playerLerpSpeed * Time.deltaTime), player.transform.position.z);
            player.transform.position = Vector3.MoveTowards(playerPositionLerp, playerMovementTarget, playerJumpSpeed * Time.deltaTime);
            playerDistanceToPlatform = playerMovementTarget - playerPositionLerp;

            if (player.transform.position == playerMovementTarget)
            {
                playerDistanceToPlatform.x = 2;
                platformsReached += 1;
                if (platformsReached == platforms.Count)
                {
                    EndSequence();
                }
                playerIsJumping = false;
            }
        }
    }

    void EndSequence()
    {
        playerScript.enabled = true;
        sequenceCamera.SetActive(false);
        creatureSpawnsPlatforms = false;
        sequenceIsRunning = false;
        playerRig.velocity = new Vector3(0, 0, 0);
    }

    IEnumerator CreatureDoesTrick()
    {
        moustacheAnim.SetBool("UseAbility", true);
        pressButtonPopup.SetActive(false);
        yield return new WaitForSeconds(1F);
        moustacheAnim.SetBool("UseAbility", false);
        platformsObject.SetActive(true);
        yield return new WaitForSeconds(0.5F);
        pressButtonPopup.SetActive(true);
        sequenceIsRunning = true;
    }

}
