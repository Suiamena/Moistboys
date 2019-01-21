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
    //public const float triggerAbilityRange = 16;

    //MANAGER
    int activePlatform = 0;
    public GameObject spawnPlatformParticle;
    bool creatureBecamePiccolo, sequenceIsRunning;

    private void Awake()
    {
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
                if ((defaultCreaturePos.SquareDistance(player.transform.position) > ((flyInOutRange * flyInOutRange) * 5))) {
                    if (!flyingRoutineRunning) {
                        flyingRoutineRunning = true;
                        StartCoroutine(EndSequence());
                        StartCoroutine(FlyOut());
                    }
                }
            }
        }
    }

    IEnumerator FlyIn()
    {
        moustacheBoi.transform.position = flyInPosition + defaultCreatureRot * flyInOutPoint;
        moustacheBoi.transform.LookAt(flyInPosition);
        moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));
        moustacheBoi.SetActive(true);
        MoustacheBoiAudio.PlayFlaps();
        moustacheAnimator.SetBool("isFlying", true);
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
            moustacheBoi.transform.LookAt(player.transform.position);
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

    IEnumerator CreatureSpawnsFirstPlatform()
    {
        MoustacheBoiAudio.PlayRumble();
        GamePad.SetVibration(0, .6f, .6f);
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
        GamePad.SetVibration(0, 0, 0);
        moustacheAnimator.SetBool("isFlying", true);
        flyToPlatformPosition = platformTransforms[1].position + platformTransforms[1].transform.rotation * new Vector3(0, -2, -12);
        while (moustacheBoi.transform.position.SquareDistance(flyToPlatformPosition) > .1f) {
            moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, flyToPlatformPosition, (jumpingSpeed * 2f) * Time.deltaTime);
            yield return null;
        }
        sequenceIsRunning = true;
    }

    public void NewPlatform(bool playerOnPlatform)
    {
        activePlatform += 1;
        if (activePlatform < platformTransforms.Count - 1) {
            StartCoroutine(CreatureSpawnsPlatform(activePlatform));
        } else {
            moustacheAnimator.SetBool("isFlying", false);
        }
    }

    IEnumerator CreatureFliesToPlatform()
    {
        if (creatureBecamePiccolo) {
        } else {
            if (activePlatform < platformTransforms.Count - 1) {
                flyToPlatformPosition = platformTransforms[activePlatform + 1].position + platformTransforms[activePlatform + 1].transform.rotation * new Vector3(0, -2, -12);
            } else {
                flyToPlatformPosition = platformTransforms[activePlatform].position + platformTransforms[activePlatform].transform.rotation * new Vector3(0, 0, 0);
                moustacheAnimator.SetBool("isFlying", false);
            }
            while (moustacheBoi.transform.position.SquareDistance(flyToPlatformPosition) > .1f) {
                moustacheBoi.transform.LookAt(player.transform.position);
                moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, flyToPlatformPosition, (jumpingSpeed * 2f) * Time.deltaTime);
                yield return null;
            }
        }
    }

    IEnumerator CreatureSpawnsPlatform(int currentPlatform)
    {
        PlatformType platformTypeScript;
        platformTypeScript = platformTransforms[currentPlatform].GetComponent<PlatformType>();
        if (platformTypeScript.platformIsElevator) {
            creatureBecamePiccolo = true;
        }
        //FLY TO NEXT PLATFORM
        if (!creatureBecamePiccolo) {
            StartCoroutine(CreatureFliesToPlatform());
        }

        GameObject particle = Instantiate(spawnPlatformParticle, flyToPlatformPosition, Quaternion.Euler(0, 5, 5));
        for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
            if (platformTypeScript.emergeFromTheGround) {
                platformTransforms[currentPlatform].position -= platformTransforms[currentPlatform].rotation * new Vector3(0, -platformCreationDistance, 0) / platformCreationTime * Time.deltaTime;
            } else {
                platformTransforms[currentPlatform].position -= platformTransforms[currentPlatform].rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
            }
            yield return null;
        }
        platformTransforms[currentPlatform].position = platformDefaultPositions[currentPlatform];
        //PICCOLO
        if (creatureBecamePiccolo) {
            while (creatureBecamePiccolo) {
                moustacheAnimator.SetBool("isFlying", false);

                //dit naar de elevator!
                moustacheBoi.transform.position = new Vector3(moustacheBoi.transform.position.x, player.transform.position.y, moustacheBoi.transform.position.z);
                yield return null;
            }
        }
    }

    //PICCOLO FUNCTIE

    public void DisablePiccolo()
    {
        if (sequenceIsRunning) {
            creatureBecamePiccolo = false;
            moustacheAnimator.SetBool("isFlying", true);
            StartCoroutine(CreatureFliesToPlatform());
        }
    }

    IEnumerator EndSequence()
    {
        sequenceIsRunning = false;
        creatureBecamePiccolo = false;
        activePlatform = 0;
        for (int i = 0; i < platformTransforms.Count - 1; i++)
        {
            PlatformType platformTypeScript;
            platformTypeScript = platformTransforms[i].GetComponent<PlatformType>();
            DetectPlayerOnPlatform detectPlayerScript;
            detectPlayerScript = platformTransforms[i].GetComponentInChildren<DetectPlayerOnPlatform>();
            detectPlayerScript.playerOnPlatform = false;
            for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
                if (platformTypeScript.emergeFromTheGround) {
                    platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, platformCreationDistance, 0) / platformCreationTime * Time.deltaTime;
                } else {
                    platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, 0, -platformCreationDistance) / platformCreationTime * Time.deltaTime;
                }
                yield return null;
            }
            if (platformTypeScript.emergeFromTheGround) {
                platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, -platformCreationDistance, 0);
            } else {
                platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
            }
        }
    }
}