using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour
{
	public static NewWallMechanic currentCreatureLocation = null;

	GameObject player;
	GameObject playerModel;
	PlayerController playerScript;
	Rigidbody playerRig;
	Animator playerAnim;

	float jumpingSpeed = 40;
	float playerPlatformOffset = .7f;

	[Header("Platform Settings")]
	public GameObject platformsObject;
	List<Transform> platformTransforms = new List<Transform>();

	[Header("Other Settings")]
	public const float triggerAbilityRange = 10;
	public float cameraMovementSpeed = 40;

	[Header("Social Events")]
	public GameObject beforeSequenceSocialPrefab;
	public GameObject duringSequenceSocialPrefab, afterSequenceSocialPrefab;
	bool beforeSequenceEventPlayed = false, duringSequenceEventPlayed = false, afterSequenceEventPlayed = false;

	[Header("")]
	//CREATURE
	public GameObject moustacheBoi;
	Animator moustacheAnim;

	//UI
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

		moustacheAnim = moustacheBoi.GetComponent<Animator>();

		jumpingSpeed = playerScript.creatureWallJumpSpeed;
		Transform platformsParent;
		platformsParent = transform.parent.GetChild(1);
		for (int i = 0; i < platformsParent.childCount; i++)
			platformTransforms.Add(platformsParent.GetChild(i));

		if (beforeSequenceSocialPrefab != null) {
			beforeSequenceSocialPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
				beforeSequenceSocialPrefab.GetComponent<ISocialEncounter>().Execute(() => {
					beforeSequenceSocialPrefab.GetComponent<ISocialEncounter>().End(() => { beforeSequenceEventPlayed = true; });
				});
			});
		} else {
			beforeSequenceEventPlayed = true;
		}
	}

	private void FixedUpdate ()
	{
		TriggerSequence();
		StartSequence();
		StartJump();
		MakeJump();
	}


	void TriggerSequence ()
	{
		if (beforeSequenceEventPlayed) {
			if (Vector3.Distance(moustacheBoi.transform.position, player.transform.position) < triggerAbilityRange) {
				if (!creatureSpawnsPlatforms) {
					pressButtonPopup.SetActive(true);
				}
				enableSequence = true;

				if (duringSequenceSocialPrefab != null) {
					duringSequenceSocialPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
						duringSequenceSocialPrefab.GetComponent<ISocialEncounter>().Execute(() => {
							duringSequenceSocialPrefab.GetComponent<ISocialEncounter>().End(() => { duringSequenceEventPlayed = true; });
						});
					});
				} else {
					duringSequenceEventPlayed = true;
				}
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
				sequenceCamera.SetActive(true);
				sequenceCamera.transform.position = platformTransforms[0].GetChild(0).position;
				sequenceCamera.transform.LookAt(player.transform.position);

				//SPAWN OBJECTS
				creatureSpawnsPlatforms = true;
				StartCoroutine(CreatureDoesTrick());
			}
		}
	}

	void StartJump ()
	{
		if (Input.GetButtonDown("A Button") && sequenceIsRunning && !playerIsJumping) {
			playerIsJumping = true;
		}
	}

	void MakeJump ()
	{
		if (playerIsJumping) {
			sequenceCamera.transform.position = Vector3.MoveTowards(sequenceCamera.transform.position, platformTransforms[activePlatform].transform.GetChild(0).position, cameraMovementSpeed * Time.deltaTime);
			sequenceCamera.transform.LookAt(player.transform);

			//HIER GING IETS FOUT
			//playerMovementTarget = platforms[activePlatform].transform.position;

			//playerPositionLerp = new Vector3(player.transform.position.x,
			//	Mathf.Lerp(player.transform.position.y, playerMovementTarget.y, playerLerpSpeed * Time.deltaTime),
			//	player.transform.position.z);
			//player.transform.position = Vector3.MoveTowards(playerPositionLerp, playerMovementTarget, playerJumpSpeed * Time.deltaTime);

			//playerDistanceToPlatform = player.transform.position - playerMovementTarget;
			//playerDistanceToPlatform = new Vector3(Mathf.Abs(playerDistanceToPlatform.x), playerDistanceToPlatform.y, playerDistanceToPlatform.z);

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

		if (afterSequenceSocialPrefab != null && duringSequenceEventPlayed) {
			afterSequenceSocialPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
				afterSequenceSocialPrefab.GetComponent<ISocialEncounter>().Execute(() => {
					afterSequenceSocialPrefab.GetComponent<ISocialEncounter>().End(() => { afterSequenceEventPlayed = true; });
				});
			});
		} else {
			duringSequenceEventPlayed = true;
		}
	}

	IEnumerator CreatureDoesTrick ()
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