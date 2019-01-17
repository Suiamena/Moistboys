using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlangeMuurInteractive : MonoBehaviour
{
    public static int currentCreatureLocation = 0;

    GameObject player;
    GameObject playerModel;
    PlayerController playerScript;
    Rigidbody playerRig;
    Animator playerAnim;

    float jumpingSpeed = 40, jumpHeight = 3;
    float playerPlatformOffset = .7f;

    public GameObject moustacheBoi;
    Animator moustacheAnimator;

    [Header("Platform Settings")]
    public GameObject creatureFlyInPositionObject;
    public GameObject platformsObject;
    public GameObject initialCameraPoint, initialCameraTarget;
    public float platformCreationTime = .5f, platformCreationDistance = 7f;
    List<Transform> platformTransforms = new List<Transform>();
    List<Vector3> platformDefaultPositions = new List<Vector3>();

    [Header("Flying Settings")]
    public Vector3 flyInOutPoint = new Vector3(0, 40, -7);
    public float flyingSpeed = 50, flyInOutRange = 45;
    Vector3 defaultCreaturePos, flyInPosition, flyToPlatformPosition;
    Quaternion defaultCreatureRot;
    bool flyingRoutineRunning = false;

    [Header("Social Events")]
    public GameObject wagglePrefab;
    public GameObject sneezePrefab, welcomeBackPrefab, flopPrefab, superflopPrefab, totZoPrefab;
    bool readyForSequence = false, afterSequenceEventPlayed = false;

    [Header("Other Settings")]
    public float cameraMovementSpeed = 40;
    public const float triggerAbilityRange = 16;

    //UI
    [Header("")]
    public GameObject sequenceCamera;

    //MANAGER
    bool enableSequence, creatureSpawnsPlatforms, sequenceIsRunning, oldPlayerIsJumping, playerIsJumping, creatureIsSpawningPlatforms, readyToStart;
    int activePlatform = 1;

    //NEW OBJECTS
    Vector3 nextCameraPosition;
    public GameObject spawnPlatformParticle;
    GameObject playerCamera;

    private void Awake()
    {
        playerCamera = GameObject.Find("Main Camera");
        player = GameObject.Find("Character");
        playerModel = GameObject.Find("MOD_Draak");
        playerScript = player.GetComponent<PlayerController>();
        playerRig = player.GetComponent<Rigidbody>();
        playerAnim = playerModel.GetComponent<Animator>();
        defaultCreaturePos = moustacheBoi.transform.position;
        defaultCreatureRot = moustacheBoi.transform.rotation;
        moustacheBoi.SetActive(false);
        currentCreatureLocation = 0;
        moustacheAnimator = moustacheBoi.GetComponent<Animator>();

        jumpingSpeed = playerScript.creatureWallJumpSpeed;
        Transform platformsParent;
        platformsParent = transform.GetChild(0);
        for (int i = 0; i < platformsParent.childCount; i++) {
            platformTransforms.Add(platformsParent.GetChild(i));
            platformDefaultPositions.Add(platformTransforms[i].position);
        }
        for (int i = 0; i < platformsParent.childCount - 1; i++) {
            PlatformType platformTypeScript;
            platformTypeScript = platformTransforms[i].GetComponent<PlatformType>();
            if (platformTypeScript.emergeFromTheGround) {
                platformTransforms[i].position += platformTransforms[i].rotation * new Vector3(0, -platformCreationDistance, 0);
            } else {
                platformTransforms[i].position += platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
            }
        }
        flyInPosition = creatureFlyInPositionObject.transform.position;
    }

    private void Update()
    {
        CheckForFlying();
        Debug.Log(playerScript.onPlatform);
    }

    void CheckForFlying()
    {
        if (playerScript.creatureWallsEnabled) {
            if (currentCreatureLocation == 0) {
                if (defaultCreaturePos.SquareDistance(player.transform.position) < flyInOutRange * flyInOutRange) {
                    if (!flyingRoutineRunning) {
                        flyingRoutineRunning = true;
                        StartCoroutine(FlyIn());
                    }
                }
            }
            else if (currentCreatureLocation == gameObject.GetInstanceID()) {
                if ((defaultCreaturePos.SquareDistance(player.transform.position) > ((flyInOutRange * flyInOutRange) * 5)) || !sequenceIsRunning && !readyToStart) {
                    if (!flyingRoutineRunning)
                    {
                        sequenceIsRunning = false;
                        flyingRoutineRunning = true;
                        StartCoroutine(EndSequence());
                        StartCoroutine(FlyOut());
                    }
                }
            }
        }
    }

    public void StartJump()
    {
        sequenceIsRunning = true;
        if (!playerIsJumping && readyToStart) {
            playerIsJumping = true;
            //StartCoroutine(MakeJump(() => { oldPlayerIsJumping = true; }));
        }

        if (!creatureIsSpawningPlatforms) { 
            creatureIsSpawningPlatforms = true;
            StartCoroutine(CreatureFliesToPlatforms());
        }
    }

    IEnumerator MakeJump(System.Action callback)
    {
        platformTransforms[activePlatform].transform.position = platformTransforms[activePlatform].transform.position;
        if (activePlatform == 1) {
            yield return new WaitForSeconds(1f);
            playerCamera.SetActive(true);
        }
        player.transform.LookAt(platformTransforms[activePlatform]);
        //playerAnim.SetBool("IsBouncing", true);
        PlayerAudio.PlayWallJump();

        //Set current and target positions for calculations
        Vector3 currentPos = player.transform.position,
            nextPos = platformTransforms[activePlatform].position + new Vector3(0, playerPlatformOffset, 0);

        //apexModifier moves the apex of the player's jump towards the higher of either the starting or target platform to create a better arc
        float heightDif = nextPos.y - currentPos.y;
        float apexModifier = -.2f;
        if (heightDif > -4)
        {
            apexModifier = -.13f;
            if (heightDif > -2.5)
            {
                apexModifier = -.06f;
                if (heightDif <= -.5)
                {
                    apexModifier = 0;
                    if (heightDif > .5)
                    {
                        apexModifier = .06f;
                        if (heightDif > 2.5)
                        {
                            apexModifier = .13f;
                            if (heightDif > 4)
                                apexModifier = .2f;
                        }
                    }
                }
            }
        }
        //5 points are set at different stages of the jump with a y-axis offset to create an arc
        Vector3[] points = new Vector3[] {
            Vector3.Lerp(currentPos, nextPos, .32f + apexModifier) + Vector3.up * 3.8f,
            Vector3.Lerp(currentPos, nextPos, .38f + apexModifier) + Vector3.up * 4.5f,
            Vector3.Lerp(currentPos,nextPos, .5f + apexModifier) + Vector3.up * 4.8f,
            Vector3.Lerp(currentPos, nextPos, .62f + apexModifier) + Vector3.up * 4.5f,
            Vector3.Lerp(currentPos, nextPos, .68f+ apexModifier) + Vector3.up * 3.8f,
            nextPos
        };
        //Do da move
        int pointIndex = 0;
        while (true) {
            //Camera: look at player
            playerCamera.transform.LookAt(player.transform.position);

            //Camera: calculate the camera's position with an offset from the player
            nextCameraPosition = player.transform.position + player.transform.rotation * new Vector3(0, 3, -10);

            //Camera: move to the next position
            playerCamera.transform.position = new Vector3(Mathf.Lerp(playerCamera.transform.position.x, nextCameraPosition.x, 0.5f),
            Mathf.Lerp(playerCamera.transform.position.y, nextCameraPosition.y, 0.5f),
            Mathf.Lerp(playerCamera.transform.position.z, nextCameraPosition.z, 0.5f));

            player.transform.position = Vector3.MoveTowards(player.transform.position, points[pointIndex], jumpingSpeed * Time.deltaTime);
            if (pointIndex >= points.Length - 1) {
                Quaternion oldRot = player.transform.rotation;
                player.transform.LookAt(points[pointIndex]);
                player.transform.rotation = Quaternion.Lerp(oldRot, player.transform.rotation, 0.18f);
                player.transform.Rotate(-player.transform.eulerAngles.x, 0, 0);
            }
            if (player.transform.position.SquareDistance(points[pointIndex]) < .01f) {
                ++pointIndex;
                if (pointIndex >= points.Length) {
                    break;
                }
            }
            yield return null;
        }
        //Finalize the jump
        playerAnim.SetBool("IsBouncing", false);
        playerRig.velocity = new Vector3(0, 0, 0);
        player.transform.Rotate(new Vector3(-player.transform.eulerAngles.x, 0, -player.transform.eulerAngles.z));
        ++activePlatform;
        playerIsJumping = false;
        if (activePlatform >= platformTransforms.Count && sequenceIsRunning) {
            StartCoroutine(EndSequence());
        } else {
            StartJump();
        }
        callback();
    }

    public void StartEndSequence()
    {
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        sequenceIsRunning = false;
        playerIsJumping = false;
        creatureIsSpawningPlatforms = false;
        readyToStart = false;
        creatureSpawnsPlatforms = false;
        activePlatform = 0;

        for (int i = 0; i < platformTransforms.Count - 1; i++) {
            for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
                PlatformType platformTypeScript;
                platformTypeScript = platformTransforms[i].GetComponent<PlatformType>();
                if (platformTypeScript.emergeFromTheGround) {
                    platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, platformCreationDistance, 0) / platformCreationTime * Time.deltaTime;
                } else {
                    platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, 0, -platformCreationDistance) / platformCreationTime * Time.deltaTime;
                }

                //platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, 0, -1 * platformCreationDistance) / platformCreationTime * Time.deltaTime;
                yield return null;
            }
            for (int j = 0; j < platformTransforms.Count - 1; j++) {
                PlatformType platformTypeScript;
                platformTypeScript = platformTransforms[i].GetComponent<PlatformType>();
                if (platformTypeScript.emergeFromTheGround) {
                    platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, -platformCreationDistance, 0);
                } else {
                    platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
                }
                //platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
                yield return null;
            }
        }
        readyForSequence = true;
    }

    IEnumerator CreatureSpawnsFirstPlatform()
    {
        readyToStart = true;
        //THIS WILL BECOME DIFFERENT!

        //yield return new WaitForSeconds(0.5f);
        MoustacheBoiAudio.PlayRumble();
        //moustacheAnimator.SetBool("isUsingAbility", true);
        GamePad.SetVibration(0, .6f, .6f);
        //yield return new WaitForSeconds(.2f);

        //Spawn platform and particles
        GameObject particle = Instantiate(spawnPlatformParticle, flyToPlatformPosition, Quaternion.Euler(0, 0, 0));
        particle.transform.rotation = platformTransforms[0].transform.rotation;
        particle.transform.Rotate(-90, 0, 0);
        particle.transform.position = platformTransforms[0].position + platformTransforms[0].transform.rotation * new Vector3(0, -2, -5);
        for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
            platformTransforms[0].position -= platformTransforms[0].rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
            yield return null;
        }
        platformTransforms[0].position = platformDefaultPositions[0];

        GamePad.SetVibration(0, .6f, .6f);
        //moustacheAnimator.SetBool("isUsingAbility", false);
        //yield return new WaitForSeconds(.2f);
        GamePad.SetVibration(0, 0, 0);
    }

    IEnumerator CreatureFliesToPlatforms()
    {
        for (int i = 1; i < platformTransforms.Count; i++) {
            flyToPlatformPosition = platformTransforms[i].position + platformTransforms[i].transform.rotation * new Vector3(0, 5, -10);
            //hier gaat het mis
            while (moustacheBoi.transform.position.SquareDistance(flyToPlatformPosition) > .1f && sequenceIsRunning) {
                moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, flyToPlatformPosition, (jumpingSpeed * 1f) * Time.deltaTime);
                yield return null;
            }
            if (i != platformTransforms.Count)
            {
                StartCoroutine(PlatformSpawnsNow(i));
            }
        }
    }

    IEnumerator PlatformSpawnsNow(int currentPlatform)
    {
        GameObject particle = Instantiate(spawnPlatformParticle, flyToPlatformPosition, Quaternion.Euler(0, 0, 0));
        //particle.transform.rotation = platformTransforms[currentPlatform].transform.rotation;
        //particle.transform.Rotate(-90, 0, 0);
        //particle.transform.position = platformTransforms[currentPlatform].position + platformTransforms[currentPlatform].transform.rotation * new Vector3(0, -2, -5);
        particle.transform.position = platformTransforms[currentPlatform].position + platformTransforms[currentPlatform].transform.rotation * new Vector3(0, 5, 5);
        Debug.Log("spawned");
        for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
            PlatformType platformTypeScript;
            platformTypeScript = platformTransforms[currentPlatform].GetComponent<PlatformType>();
            if (platformTypeScript.emergeFromTheGround) {
                platformTransforms[currentPlatform].position -= platformTransforms[currentPlatform].rotation * new Vector3(0, -platformCreationDistance, 0) / platformCreationTime * Time.deltaTime;
            } else {
                platformTransforms[currentPlatform].position -= platformTransforms[currentPlatform].rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
            }
           yield return null;
        }
        platformTransforms[currentPlatform].position = platformDefaultPositions[currentPlatform];
    }

    IEnumerator FlyIn()
    {
        moustacheBoi.transform.position = flyInPosition + defaultCreatureRot * flyInOutPoint;
        moustacheBoi.transform.LookAt(flyInPosition);
        moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));
        moustacheBoi.SetActive(true);

        MoustacheBoiAudio.PlayFlaps();
        moustacheAnimator.SetBool("isFlying", true);
        //added *.1f
        while (moustacheBoi.transform.position.SquareDistance(flyInPosition) > .1f) {
            moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, flyInPosition, flyingSpeed * Time.deltaTime);
            yield return null;
        }
        MoustacheBoiAudio.StopFlaps();
        moustacheAnimator.SetBool("isFlying", false);
        while (Quaternion.Angle(moustacheBoi.transform.rotation, defaultCreatureRot) > .1f) {
            moustacheBoi.transform.rotation = Quaternion.RotateTowards(moustacheBoi.transform.rotation, defaultCreatureRot, 260 * Time.deltaTime);
            yield return null;
        }
        moustacheBoi.transform.rotation = defaultCreatureRot;
        yield return new WaitForSeconds(.8f);
        currentCreatureLocation = gameObject.GetInstanceID();
        flyingRoutineRunning = false;

        StartCoroutine(CreatureSpawnsFirstPlatform());
    }

    IEnumerator FlyOut()
    {
        moustacheBoi.transform.LookAt(defaultCreaturePos + defaultCreatureRot * flyInOutPoint);
        moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));
        MoustacheBoiAudio.PlayFlaps();
        moustacheAnimator.SetBool("isFlying", true);
        while (moustacheBoi.transform.position.SquareDistance(defaultCreaturePos + defaultCreatureRot * flyInOutPoint) > 0.2f) {
            moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, defaultCreaturePos + defaultCreatureRot * flyInOutPoint, flyingSpeed * Time.deltaTime);
            yield return null;
        }
        MoustacheBoiAudio.StopFlaps();
        moustacheAnimator.SetBool("isFlying", false);
        moustacheBoi.gameObject.SetActive(false);
        yield return new WaitForSeconds(.6f);
        currentCreatureLocation = 0;
        flyingRoutineRunning = false;
    }
}