using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlangaMuur : MonoBehaviour
{
    public GameObject spawnPlatformParticle;

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

	private void Awake ()
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
        for (int i = 0; i < platformsParent.childCount - 1; i++)
        {
            platformTransforms[i].position += platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
        }

        flyInPosition = creatureFlyInPositionObject.transform.position;
    }

	private void Update ()
	{
		CheckForFlying();
    }

	void CheckForFlying ()
	{
		if (playerScript.creatureWallsEnabled) {
			if (currentCreatureLocation == 0) {
                if (defaultCreaturePos.SquareDistance(player.transform.position) < flyInOutRange * flyInOutRange) {
                    if (!flyingRoutineRunning) {
						flyingRoutineRunning = true;
						StartCoroutine(FlyIn());
					}
				}
			} else if (currentCreatureLocation == gameObject.GetInstanceID()) {
				if (defaultCreaturePos.SquareDistance(player.transform.position) > flyInOutRange * flyInOutRange) {
					if (!flyingRoutineRunning && !sequenceIsRunning) {
						flyingRoutineRunning = true;
						StartCoroutine(FlyOut());
					}
				}
			}
		}
	}

    public void StartJump()
    {
        sequenceIsRunning = true;
        if (!playerIsJumping && readyToStart)
        {
            sequenceCamera.SetActive(true);
            playerIsJumping = true;
            playerScript.DisablePlayer();
            StartCoroutine(MakeJump(() => { oldPlayerIsJumping = true; }));
        }
        if (!creatureIsSpawningPlatforms)
        {
            creatureIsSpawningPlatforms = true;
            StartCoroutine(CreatureSpawnsPlatforms());
            StartCoroutine(CreatureFliesToPlatforms());
        }
    }

	IEnumerator MakeJump (System.Action callback)
	{
        platformTransforms[activePlatform].transform.position = platformTransforms[activePlatform].transform.position;
        if (activePlatform == 1)
        {
            yield return new WaitForSeconds(1f);
        }
        //pressButtonPopup.SetActive(false);
        player.transform.LookAt(platformTransforms[activePlatform]);
		playerAnim.SetBool("IsBouncing", true);
		PlayerAudio.PlayWallJump();

		//Set current and target positions for calculations
		Vector3 currentPos = player.transform.position,
			nextPos = platformTransforms[activePlatform].position + new Vector3(0, playerPlatformOffset, 0);

        //apexModifier moves the apex of the player's jump towards the higher of either the starting or target platform to create a better arc
        float heightDif = nextPos.y - currentPos.y;
		float apexModifier = -.2f;
		if (heightDif > -4) {
			apexModifier = -.13f;
			if (heightDif > -2.5) {
				apexModifier = -.06f;
				if (heightDif <= -.5) {
					apexModifier = 0;
					if (heightDif > .5) {
						apexModifier = .06f;
						if (heightDif > 2.5) {
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
            //sequenceCamera.transform.position = Vector3.MoveTowards(sequenceCamera.transform.position, platformTransforms[activePlatform].transform.GetChild(0).position, cameraMovementSpeed * Time.deltaTime);
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

			//sequenceCamera.transform.LookAt(player.transform);

			yield return null;
		}
		//Finalize the jump
		playerAnim.SetBool("IsBouncing", false);
		playerRig.velocity = new Vector3(0, 0, 0);
		player.transform.Rotate(new Vector3(-player.transform.eulerAngles.x, 0, -player.transform.eulerAngles.z));
		++activePlatform;
        playerIsJumping = false;
        yield return new WaitForSeconds(1.5f);
        if (activePlatform >= platformTransforms.Count && sequenceIsRunning) {
            StartCoroutine(EndSequence());
		} else {
            StartJump();
        }
        callback();
	}

	IEnumerator EndSequence ()
	{
		sequenceIsRunning = false;
        playerIsJumping = false;
        creatureIsSpawningPlatforms = false;
        readyToStart = false;
        player.transform.rotation = platformTransforms[platformTransforms.Count - 1].rotation;
		player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
		playerRig.velocity = new Vector3(0, 0, 0);
		playerScript.cameraTrans.position = sequenceCamera.transform.position;
		playerScript.EnablePlayer();
		sequenceCamera.SetActive(false);
		creatureSpawnsPlatforms = false;
		activePlatform = 0;

        for (int i = 0; i < platformTransforms.Count - 1; i++)
        {
            for (float t = 0; t < platformCreationTime; t += Time.deltaTime)
            {
                platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, 0, -1 * platformCreationDistance) / platformCreationTime * Time.deltaTime;
                yield return null;
            }
            for (int j = 0; j < platformTransforms.Count - 1; j++)
            {
                platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
                yield return null;
            }
        }
        readyForSequence = true;
	}

	IEnumerator CreatureDoesTrick ()
	{
        yield return new WaitForSeconds(0.5f);

		MoustacheBoiAudio.PlayRumble();
		moustacheAnimator.SetBool("isUsingAbility", true);

		GamePad.SetVibration(0, .6f, .6f);
		yield return new WaitForSeconds(.2f);

        for (float t = 0; t < platformCreationTime; t += Time.deltaTime)
        {
            platformTransforms[0].position -= platformTransforms[0].rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
            yield return null;
        }
        platformTransforms[0].position = platformDefaultPositions[0];

        GamePad.SetVibration(0, .6f, .6f);
		moustacheAnimator.SetBool("isUsingAbility", false);
		yield return new WaitForSeconds(.2f);
		GamePad.SetVibration(0, 0, 0);
        readyToStart = true;
	}

    IEnumerator CreatureSpawnsPlatforms()
    {
        for (int i = 1; i < platformTransforms.Count - 1; i++)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("shader and particles");
            Instantiate(spawnPlatformParticle, flyToPlatformPosition, Quaternion.identity);
            for (float t = 0; t < platformCreationTime; t += Time.deltaTime)
            {
                platformTransforms[i].position -= platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            platformTransforms[i].position = platformDefaultPositions[i];
        }
    }

    IEnumerator CreatureFliesToPlatforms()
    {
        for (int i = 1; i < platformTransforms.Count; i++)
        {
            flyToPlatformPosition = platformTransforms[i].position  + platformTransforms[i].transform.rotation * new Vector3(0, 5, -10);
            while (moustacheBoi.transform.position.SquareDistance(flyToPlatformPosition) > .1f)
            {
                moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, flyToPlatformPosition, (jumpingSpeed * 1.5f) * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
         }
    }

    IEnumerator FlyIn ()
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

        StartCoroutine(CreatureDoesTrick());
	}

	IEnumerator FlyOut ()
	{
        moustacheBoi.transform.LookAt(defaultCreaturePos + defaultCreatureRot * flyInOutPoint);
		moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));

		MoustacheBoiAudio.PlayFlaps();
        moustacheAnimator.SetBool("isFlying", true);
		while (moustacheBoi.transform.position.SquareDistance(defaultCreaturePos + defaultCreatureRot * flyInOutPoint) > 0.01f) {
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