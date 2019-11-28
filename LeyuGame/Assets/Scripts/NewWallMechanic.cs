using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class NewWallMechanic : MonoBehaviour
{
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
	public const float triggerAbilityRange = 16;

	//UI
	[Header("")]
	public GameObject pressButtonPopup;
	public GameObject sequenceCamera;

	//MANAGER
	bool enableSequence, creatureSpawnsPlatforms, sequenceIsRunning, oldPlayerIsJumping;
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
		currentCreatureLocation = 0;

		moustacheAnimator = moustacheBoi.GetComponent<Animator>();

		jumpingSpeed = playerScript.creatureWallJumpSpeed;
		Transform platformsParent;
		platformsParent = transform.parent.GetChild(1);
		for (int i = 0; i < platformsParent.childCount; i++) {
			platformTransforms.Add(platformsParent.GetChild(i));
			platformDefaultPositions.Add(platformTransforms[i].position);
			platformTransforms[i].position += platformTransforms[i].rotation * new Vector3(0, 0, platformCreationDistance);
		}
		platformTransforms[platformTransforms.Count - 1].gameObject.SetActive(false);

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
					if (!flyingRoutineRunning) {
						flyingRoutineRunning = true;
						StartCoroutine(FlyOut());
					}
				}
			}
		}
	}

	void TriggerSequence ()
	{
		if (readyForSequence && currentCreatureLocation == gameObject.GetInstanceID()) {
			if (defaultCreaturePos.SquareDistance(player.transform.position) < triggerAbilityRange * triggerAbilityRange) {
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
			if ((Input.GetButtonDown("A Button") || Input.GetButtonDown("Keyboard Space")) && !creatureSpawnsPlatforms) {
				//DISABLE PLAYER ANIMATION

				//DISABLE PLAYER MOVEMENT
				player.transform.position = new Vector3(player.transform.position.x, moustacheBoi.transform.position.y, player.transform.position.z);
				playerScript.DisablePlayer();
				playerRig.velocity = Vector3.zero;

				//SHOW CAMERA
				sequenceCamera.transform.position = initialCameraPoint.transform.position;
				sequenceCamera.transform.LookAt(initialCameraTarget.transform);
				sequenceCamera.SetActive(true);
				Camera.main.gameObject.SetActive(false);

				//SPAWN OBJECTS
				creatureSpawnsPlatforms = true;
				readyForSequence = false;
				enableSequence = false;
				StartCoroutine(CreatureDoesTrick());
			}
		}
	}

	void StartJump ()
	{
		if (sequenceIsRunning && !oldPlayerIsJumping) {
			oldPlayerIsJumping = true;
			StartCoroutine(MakeJump(() => { oldPlayerIsJumping = false; }));
		}
		//if ((Input.GetButtonDown("A Button") || Input.GetButtonDown("Keyboard Space")) && sequenceIsRunning && !oldPlayerIsJumping) {
		//	oldPlayerIsJumping = true;
		//	StartCoroutine(MakeJump(() => { oldPlayerIsJumping = false; }));
		//}
	}

	IEnumerator MakeJump (System.Action callback)
	{
		pressButtonPopup.SetActive(false);
		player.transform.LookAt(platformTransforms[activePlatform]);
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
			sequenceCamera.transform.position = Vector3.MoveTowards(sequenceCamera.transform.position, platformTransforms[activePlatform].transform.GetChild(0).position, cameraMovementSpeed * Time.deltaTime);

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

			sequenceCamera.transform.LookAt(player.transform);

			yield return null;
		}
		//Finalize the jump
		playerAnim.SetBool("IsBouncing", false);
		playerRig.velocity = new Vector3(0, 0, 0);
		player.transform.Rotate(new Vector3(-player.transform.eulerAngles.x, 0, -player.transform.eulerAngles.z));
		++activePlatform;
		if (activePlatform >= platformTransforms.Count) {
			StartCoroutine(EndSequence());
		} else {
			pressButtonPopup.SetActive(true);
		}
		callback();
	}

	IEnumerator EndSequence ()
	{
		sequenceIsRunning = false;
		player.transform.rotation = platformTransforms[platformTransforms.Count - 1].rotation;
		player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
		playerRig.velocity = new Vector3(0, 0, 0);
		playerScript.cameraTrans.position = sequenceCamera.transform.position;
		playerScript.EnablePlayer();

		Camera.main.gameObject.SetActive(true);
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

		readyForSequence = true;
	}

	IEnumerator CreatureDoesTrick ()
	{
		bool readyToAdvance = false;

		player.transform.LookAt(moustacheBoi.transform);
		player.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));
		moustacheBoi.transform.LookAt(player.transform);
		moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));

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

		while (moustacheBoi.transform.position.SquareDistance(flyInPosition) > .1f * .1f) {
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