using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour
{
	public static int currentCreatureLocation = 0;

	GameObject player;
	GameObject playerModel;
	PlayerController playerScript;
	Rigidbody playerRig;
	Animator playerAnim;

	float jumpingSpeed = 40;
	float playerPlatformOffset = .7f;

	public GameObject moustacheBoi;

	[Header("Platform Settings")]
	public GameObject platformsObject;
	public GameObject initialCameraPoint, initialCameraTarget;
	List<Transform> platformTransforms = new List<Transform>();

	[Header("Flying Settings")]
	public Vector3 flyInOutPoint = new Vector3(0, 40, -7);
	public float flyingSpeed = 50, flyInOutRange = 25;
	Vector3 defaultCreaturePos;
	Quaternion defaultCreatureRot;
	bool flyingRoutineRunning = false;

	[Header("Social Events")]
	public GameObject beforeSequenceSocialPrefab;
	public GameObject afterSequenceSocialPrefab;
	bool beforeSequenceEventPlayed = false, afterSequenceEventPlayed = false;

	[Header("Other Settings")]
	public const float triggerAbilityRange = 10;
	public float cameraMovementSpeed = 40;

	//CREATURE
	Animator moustacheAnim;

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

		moustacheAnim = moustacheBoi.GetComponent<Animator>();

		jumpingSpeed = playerScript.creatureWallJumpSpeed;
		Transform platformsParent;
		platformsParent = transform.parent.GetChild(1);
		for (int i = 0; i < platformsParent.childCount; i++)
			platformTransforms.Add(platformsParent.GetChild(i));

		if (beforeSequenceSocialPrefab != null && currentCreatureLocation == gameObject.GetInstanceID()) {
			beforeSequenceSocialPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
				beforeSequenceSocialPrefab.GetComponent<ISocialEncounter>().Execute(() => {
					beforeSequenceSocialPrefab.GetComponent<ISocialEncounter>().End(() => { beforeSequenceEventPlayed = true; });
				});
			});
		} else {
			beforeSequenceEventPlayed = true;
		}
		if (afterSequenceSocialPrefab == null) {
			afterSequenceEventPlayed = true;
		}
	}

	private void Update ()
	{
		CheckForFlying();
		TriggerSequence();
		StartSequence();
		StartJump();
		MakeJump();
	}

	void CheckForFlying ()
	{
		Debug.Log(currentCreatureLocation);
		if (currentCreatureLocation == 0) {
			if (Vector3.Distance(defaultCreaturePos, player.transform.position) < flyInOutRange) {
				if (!flyingRoutineRunning) {
					flyingRoutineRunning = true;
					StartCoroutine(FlyIn());
				}
			}
		} else if (currentCreatureLocation == gameObject.GetInstanceID() && afterSequenceEventPlayed) {
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
		if (beforeSequenceEventPlayed && currentCreatureLocation == gameObject.GetInstanceID()) {
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
				playerScript.enabled = false;
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
		}
	}

	void MakeJump ()
	{
		if (playerIsJumping) {
			sequenceCamera.transform.position = Vector3.MoveTowards(sequenceCamera.transform.position, platformTransforms[activePlatform].transform.GetChild(0).position, cameraMovementSpeed * Time.deltaTime);
			sequenceCamera.transform.LookAt(player.transform);

			Vector3 targetPosition = platformTransforms[activePlatform].position + new Vector3(0, playerPlatformOffset, 0);
			player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, jumpingSpeed * Time.deltaTime);

			if (Vector3.Distance(player.transform.position, targetPosition) < .1f) {
				playerRig.velocity = new Vector3(0, 0, 0);
				++activePlatform;
				if (activePlatform == platformTransforms.Count) {
					EndSequence();
				}
				playerIsJumping = false;
			}
		}
	}

	void EndSequence ()
	{
		player.transform.rotation = platformTransforms[platformTransforms.Count - 1].rotation;
		player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
		playerScript.enabled = true;
		playerRig.velocity = new Vector3(0, 0, 0);

		platformsObject.SetActive(false);
		sequenceCamera.SetActive(false);
		creatureSpawnsPlatforms = false;
		sequenceIsRunning = false;
		activePlatform = 0;

		if (!afterSequenceEventPlayed) {
			afterSequenceSocialPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
				afterSequenceSocialPrefab.GetComponent<ISocialEncounter>().Execute(() => {
					afterSequenceSocialPrefab.GetComponent<ISocialEncounter>().End(() => { afterSequenceEventPlayed = true; });
				});
			});
		}
	}

	IEnumerator CreatureDoesTrick ()
	{
        MoustacheBoiAudio.PlayRumble();
		moustacheAnim.SetBool("UseAbility", true);
		pressButtonPopup.SetActive(false);
		yield return new WaitForSeconds(1F);
		moustacheAnim.SetBool("UseAbility", false);
		platformsObject.SetActive(true);
		yield return new WaitForSeconds(0.5F);
		pressButtonPopup.SetActive(true);
		sequenceIsRunning = true;
	}

	IEnumerator FlyIn ()
	{
		moustacheBoi.transform.position = defaultCreaturePos + defaultCreatureRot * flyInOutPoint;
		moustacheBoi.transform.LookAt(defaultCreaturePos);
		moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));
		moustacheBoi.SetActive(true);
        MoustacheBoiAudio.PlayFlaps();

        while (Vector3.Distance(moustacheBoi.transform.position, defaultCreaturePos) > .1f) {
            moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, defaultCreaturePos, flyingSpeed * Time.deltaTime);
			yield return null;
		}

		while (Quaternion.Angle(moustacheBoi.transform.rotation, defaultCreatureRot) > .1f) {
			moustacheBoi.transform.rotation = Quaternion.RotateTowards(moustacheBoi.transform.rotation, defaultCreatureRot, 260 * Time.deltaTime);
			yield return null;
		}
		moustacheBoi.transform.rotation = defaultCreatureRot;
		currentCreatureLocation = gameObject.GetInstanceID();
        flyingRoutineRunning = false;
	}

	IEnumerator FlyOut ()
	{
		moustacheBoi.transform.LookAt(defaultCreaturePos + defaultCreatureRot * flyInOutPoint);
		moustacheBoi.transform.Rotate(new Vector3(-moustacheBoi.transform.eulerAngles.x, 0, -moustacheBoi.transform.eulerAngles.z));

		MoustacheBoiAudio.PlayFlaps();
		while (Vector3.Distance(moustacheBoi.transform.position, defaultCreaturePos + defaultCreatureRot * flyInOutPoint) > 0.1f) {
			moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, defaultCreaturePos + defaultCreatureRot * flyInOutPoint, flyingSpeed * Time.deltaTime);
			yield return null;
		}

		MoustacheBoiAudio.StopFlaps();
		moustacheBoi.gameObject.SetActive(false);
		currentCreatureLocation = 0;
		flyingRoutineRunning = false;
	}
}