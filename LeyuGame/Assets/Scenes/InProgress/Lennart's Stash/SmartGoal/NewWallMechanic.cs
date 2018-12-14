using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class NewWallMechanic : MonoBehaviour
{

	//DEBUG STUFF
	float distanceToNextPlatform;

	public static int currentCreatureLocation = 0;

	public enum PreSequenceActivities { Waggle, Sneeze, WelcomeBack, None };
	public PreSequenceActivities preSequenceActivity = PreSequenceActivities.None;
	public enum SequenceActivities { Flop, Superflop, None };
	public SequenceActivities sequenceActivity = SequenceActivities.None;
	public enum PostSequenceActivities { TotZo, None };
	public PostSequenceActivities postSequenceActivity = PostSequenceActivities.None;

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
	public GameObject platformsObject;
	public GameObject initialCameraPoint, initialCameraTarget;
	public float platformCreationTime = .5f, platformCreationDistance = 5f;
	List<Transform> platformTransforms = new List<Transform>();
	List<Vector3> platformDefaultPositions = new List<Vector3>();

	[Header("Flying Settings")]
	public Vector3 flyInOutPoint = new Vector3(0, 40, -7);
	public float flyingSpeed = 50, flyInOutRange = 45;
	Vector3 defaultCreaturePos, flyInPosition;
	Quaternion defaultCreatureRot;
	bool flyingRoutineRunning = false;

	[Header("Social Events")]
	public GameObject wagglePrefab;
	public GameObject sneezePrefab, welcomeBackPrefab, flopPrefab, superflopPrefab, totZoPrefab;
	bool readyForSequence = false, afterSequenceEventPlayed = false;

	[Header("Other Settings")]
	public float cameraMovementSpeed = 40;
	public const float triggerAbilityRange = 10;

	//UI
	[Header("")]
	public GameObject pressButtonPopup;
	public GameObject sequenceCamera;

	//MANAGER
	bool enableSequence, creatureSpawnsPlatforms, sequenceIsRunning, playerIsJumping;
	int activePlatform;

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

		moustacheAnimator = moustacheBoi.GetComponent<Animator>();

		jumpingSpeed = playerScript.creatureWallJumpSpeed;
		Transform platformsParent;
		platformsParent = transform.parent.GetChild(1);
		for (int i = 0; i < platformsParent.childCount; i++) {
			platformTransforms.Add(platformsParent.GetChild(i));
			platformDefaultPositions.Add(platformTransforms[i].position);
			platformTransforms[i].position += platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
		}

		if (preSequenceActivity == PreSequenceActivities.Waggle) {
			flyInPosition = wagglePrefab.transform.GetChild(0).position;
		} else {
			flyInPosition = defaultCreaturePos;
		}
	}

	private void Update ()
	{
		CheckForFlying();
		TriggerSequence();
		StartSequence();
		StartJump();
	}

	void CheckForFlying ()
	{
		if (currentCreatureLocation == 0) {
			if (Vector3.Distance(defaultCreaturePos, player.transform.position) < flyInOutRange) {
				if (!flyingRoutineRunning) {
					flyingRoutineRunning = true;
					StartCoroutine(FlyIn());
				}
			}
		} else if (currentCreatureLocation == gameObject.GetInstanceID()) {
			if (Vector3.Distance(defaultCreaturePos, player.transform.position) > flyInOutRange) {
				if (!flyingRoutineRunning) {
					flyingRoutineRunning = true;
					StartCoroutine(FlyOut());
				}
			}
		}
	}

	void TriggerSequence ()
	{
		if (readyForSequence && currentCreatureLocation == gameObject.GetInstanceID()) {
			if (Vector3.Distance(defaultCreaturePos, player.transform.position) < triggerAbilityRange) {
				if (!creatureSpawnsPlatforms) {
					pressButtonPopup.SetActive(true);
				}
				enableSequence = true;
			} else {
				pressButtonPopup.SetActive(false);
				enableSequence = false;
			}
		}
	}

	void StartSequence ()
	{
		if (enableSequence) {
			if (Input.GetButtonDown("A Button") && !creatureSpawnsPlatforms) {
				//DISABLE PLAYER ANIMATION
				playerAnim.SetBool("IsBouncing", false);
				playerAnim.SetBool("IsLaunching", false);
				playerAnim.SetBool("IsAirborne", false);

				//DISABLE PLAYER MOVEMENT
				player.transform.position = new Vector3(player.transform.position.x, moustacheBoi.transform.position.y, player.transform.position.z);
				playerScript.DisablePlayer();
				playerRig.velocity = Vector3.zero;

				//SHOW CAMERA
				sequenceCamera.transform.position = initialCameraPoint.transform.position;
				sequenceCamera.transform.LookAt(initialCameraTarget.transform);
				sequenceCamera.SetActive(true);

				//SPAWN OBJECTS
				creatureSpawnsPlatforms = true;
				StartCoroutine(CreatureDoesTrick());
			}
		}
	}

	void StartJump ()
	{
		if (Input.GetButtonDown("A Button") && sequenceIsRunning && !playerIsJumping) {
			PlayerAudio.PlayWallJump();
			playerIsJumping = true;
			StartCoroutine(MakeJump(() => { playerIsJumping = false; }));
		}
	}

	IEnumerator MakeJump (System.Action callback)
	{
		player.transform.LookAt(platformTransforms[activePlatform]);
		playerAnim.SetBool("IsBouncing", true);

		Vector3 currentPos = player.transform.position,
			nextPos = platformTransforms[activePlatform].position + new Vector3(0, playerPlatformOffset, 0);

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

		Vector3[] points = new Vector3[] {
			Vector3.Lerp(currentPos, nextPos, .32f + apexModifier) + Vector3.up * 3.8f,
			Vector3.Lerp(currentPos, nextPos, .38f + apexModifier) + Vector3.up * 4.5f,
			Vector3.Lerp(currentPos,nextPos, .5f + apexModifier) + Vector3.up * 4.8f,
			Vector3.Lerp(currentPos, nextPos, .62f + apexModifier) + Vector3.up * 4.5f,
			Vector3.Lerp(currentPos, nextPos, .68f+ apexModifier) + Vector3.up * 3.8f,
			nextPos
		};

		int pointIndex = 0;

		while (true) {
			sequenceCamera.transform.position = Vector3.MoveTowards(sequenceCamera.transform.position, platformTransforms[activePlatform].transform.GetChild(0).position, cameraMovementSpeed * Time.deltaTime);

			player.transform.position = Vector3.MoveTowards(player.transform.position, points[pointIndex], jumpingSpeed * Time.deltaTime);
			Quaternion oldRot = player.transform.rotation;
			player.transform.LookAt(points[pointIndex]);
			player.transform.rotation = Quaternion.Lerp(oldRot, player.transform.rotation, 0.18f);
			player.transform.Rotate(-player.transform.eulerAngles.x, 0, 0);

			if (Vector3.Distance(player.transform.position, points[pointIndex]) < .1f) {
				++pointIndex;
				if (pointIndex >= points.Length) {
					break;
				}
			}

			sequenceCamera.transform.LookAt(player.transform);

			yield return null;
		}

		playerAnim.SetBool("IsBouncing", false);
		playerRig.velocity = new Vector3(0, 0, 0);
		player.transform.Rotate(new Vector3(-player.transform.eulerAngles.x, 0, -player.transform.eulerAngles.z));
		++activePlatform;
		if (activePlatform >= platformTransforms.Count) {
			StartCoroutine(EndSequence());
		}
		callback();
	}

	IEnumerator EndSequence ()
	{
		player.transform.rotation = platformTransforms[platformTransforms.Count - 1].rotation;
		player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
		playerRig.velocity = new Vector3(0, 0, 0);
		playerScript.EnablePlayer();

		sequenceCamera.SetActive(false);
		creatureSpawnsPlatforms = false;
		activePlatform = 0;

		if (postSequenceActivity == PostSequenceActivities.TotZo) {
			totZoPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
				totZoPrefab.GetComponent<ISocialEncounter>().Execute(() => {
					totZoPrefab.GetComponent<ISocialEncounter>().End(() => { afterSequenceEventPlayed = true; });
				});
			});
		} else {
			afterSequenceEventPlayed = true;
		}

		for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
			foreach (Transform trans in platformTransforms) {
				trans.position += trans.rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
			}
			yield return null;
		}
		for (int i = 0; i < platformTransforms.Count; i++) {
			platformTransforms[i].position = platformDefaultPositions[i] + platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
		}

		sequenceIsRunning = false;
	}

	IEnumerator CreatureDoesTrick ()
	{
		bool readyToAdvance = false;
		switch (sequenceActivity) {
			case SequenceActivities.None:
				readyToAdvance = true;
				break;
			case SequenceActivities.Flop:
				flopPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
					flopPrefab.GetComponent<ISocialEncounter>().Execute(() => {
						flopPrefab.GetComponent<ISocialEncounter>().End(() => {
							readyToAdvance = true;
						});
					});
				});
				break;
			case SequenceActivities.Superflop:
				superflopPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
					superflopPrefab.GetComponent<ISocialEncounter>().Execute(() => {
						superflopPrefab.GetComponent<ISocialEncounter>().End(() => {
							readyToAdvance = true;
						});
					});
				});
				break;
		}
		while (!readyToAdvance)
			yield return null;

		MoustacheBoiAudio.PlayRumble();
		moustacheAnimator.SetBool("isUsingAbility", true);
		pressButtonPopup.SetActive(false);

		GamePad.SetVibration(0, .6f, .6f);
		yield return new WaitForSeconds(.2f);

		for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
			foreach (Transform trans in platformTransforms) {
				trans.position -= trans.rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
			}
			yield return null;
		}
		for (int i = 0; i < platformTransforms.Count; i++) {
			platformTransforms[i].position = platformDefaultPositions[i];
		}
		GamePad.SetVibration(0, .6f, .6f);
		moustacheAnimator.SetBool("isUsingAbility", false);
		pressButtonPopup.SetActive(true);
		sequenceIsRunning = true;
		yield return new WaitForSeconds(.2f);
		GamePad.SetVibration(0, 0, 0);
	}

	IEnumerator FlyIn ()
	{
		moustacheBoi.transform.position = flyInPosition + defaultCreatureRot * flyInOutPoint;
		moustacheBoi.transform.LookAt(flyInPosition);
		moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));
		moustacheBoi.SetActive(true);
		MoustacheBoiAudio.PlayFlaps();
		moustacheAnimator.SetBool("isFlying", true);

		while (Vector3.Distance(moustacheBoi.transform.position, flyInPosition) > .1f) {
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

		switch (preSequenceActivity) {
			case PreSequenceActivities.None:
				//SET IDLE ANIM TO PLAY
				readyForSequence = true;
				break;
			case PreSequenceActivities.Waggle:
				wagglePrefab.GetComponent<ISocialEncounter>().Initialize(() => {
					wagglePrefab.GetComponent<ISocialEncounter>().Execute(() => {
						wagglePrefab.GetComponent<ISocialEncounter>().End(() => {
							readyForSequence = true;
						});
					});
				});
				break;
			case PreSequenceActivities.Sneeze:
				sneezePrefab.GetComponent<ISocialEncounter>().Initialize(() => {
					sneezePrefab.GetComponent<ISocialEncounter>().Execute(() => {
						sneezePrefab.GetComponent<ISocialEncounter>().End(() => {
							readyForSequence = true;
						});
					});
				});
				break;
			case PreSequenceActivities.WelcomeBack:
				welcomeBackPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
					welcomeBackPrefab.GetComponent<ISocialEncounter>().Execute(() => {
						welcomeBackPrefab.GetComponent<ISocialEncounter>().End(() => {
							readyForSequence = true;
						});
					});
				});
				break;
		}
	}

	IEnumerator FlyOut ()
	{
		moustacheBoi.transform.LookAt(defaultCreaturePos + defaultCreatureRot * flyInOutPoint);
		moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));

		MoustacheBoiAudio.PlayFlaps();
		moustacheAnimator.SetBool("isFlying", true);
		while (Vector3.Distance(moustacheBoi.transform.position, defaultCreaturePos + defaultCreatureRot * flyInOutPoint) > 0.1f) {
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