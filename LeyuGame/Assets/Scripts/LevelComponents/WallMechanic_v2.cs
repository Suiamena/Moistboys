using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMechanic_v2 : MonoBehaviour
{

	[Header("Camera Settings")]
	public GameObject camAnchor;
	public float cameraSpeed, cameraRotationSpeed;

	[Header("Player")]
	public int playerJumpSpeed;
	public int playerLerpSpeed;
	GameObject player, playerCam;
	Vector3 playerPositionLerp;
	bool playerIsJumping;

	PlayerController playerScript;
	Rigidbody playerRig;

	[Header("Platform Settings")]
	public List<GameObject> platforms = new List<GameObject>();
	GameObject platformsParent;
	Vector3 playerMovementTarget, platformPlayerOffset = new Vector3(0, 0.62f, 0);
	int platformsJumped;

	//Sequence Manager
	bool enableSequence, sequenceIsRunning, fuckingBoolean, coroutineRunning, creatureSpawnedPlatforms;

	public GameObject pressAObject;

	GameObject moustacheBoi;
	Animator moustacheAnim;

	private void Awake ()
	{
		player = GameObject.Find("Character");
		playerCam = GameObject.Find("Main Camera");
		moustacheBoi = GameObject.Find("MOD_MoustacheBoi");
		moustacheAnim = moustacheBoi.GetComponent<Animator>();
		playerScript = player.GetComponent<PlayerController>();
		playerRig = player.GetComponent<Rigidbody>();
		playerJumpSpeed = 50;
		playerLerpSpeed = 5;
		cameraSpeed = 10f;
		platformsParent = platforms[0].transform.parent.gameObject;
	}

	private void Update ()
	{
		JumpInput();
		SeparateCamera();
		JumpExecution();

		if (!enableSequence) {
			if (!sequenceIsRunning) {
				platformsParent.SetActive(false);
				pressAObject.SetActive(false);
				enableSequence = false;
			}
		}
		if (creatureSpawnedPlatforms && !sequenceIsRunning) {
			ShowPlatformsCutscene();
		}

	}


	//Detect wether or not the player can start the sequence
	void OnTriggerStay (Collider collider)
	{
		if (collider.tag == "Player") {
			pressAObject.SetActive(true);
			enableSequence = true;
		}
	}
	void OnTriggerExit (Collider collider)
	{
		if (collider.tag == "Player") {
			if (!sequenceIsRunning) {
				enableSequence = false;
			}
		}
	}


	//Start the sequence by pressing A and the player performs jumps from platform to platform
	void JumpInput ()
	{
		if (Input.GetButtonDown("A Button")) {
			if (enableSequence && !playerIsJumping) {
				if (creatureSpawnedPlatforms) {
					playerIsJumping = true;
					if (platformsJumped == 0) {
						StartSequence();
					}
				} else {
					//Start Spawning Platforms Cutscene
					playerRig.velocity = new Vector3(0, 0, 0);
					StartCoroutine(CreatureDoesTrick());
					creatureSpawnedPlatforms = true;
					platformsParent.SetActive(true);
					playerScript.enabled = false;
					camAnchor.SetActive(true);
					playerCam.SetActive(false);
				}
			}
		}
	}

	//Sequence setup
	void ShowPlatformsCutscene ()
	{
		camAnchor.transform.position = new Vector3(Mathf.Lerp(camAnchor.transform.position.x, player.transform.position.x, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.y, player.transform.position.y + 3, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.z, player.transform.position.z, cameraSpeed * Time.deltaTime));
		camAnchor.transform.LookAt(platforms[0].transform);
	}

	void StartSequence ()
	{
		sequenceIsRunning = true;
	}

	//Actual jumping
	void JumpExecution ()
	{
		if (playerIsJumping) {
			player.transform.rotation = transform.rotation;
			playerMovementTarget = platforms[platformsJumped].transform.position + platformPlayerOffset;
			playerPositionLerp = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, playerMovementTarget.y, playerLerpSpeed * Time.deltaTime), player.transform.position.z);
			player.transform.position = Vector3.MoveTowards(playerPositionLerp, playerMovementTarget, playerJumpSpeed * Time.deltaTime);
			//if (player.transform.position == playerMovementTarget)
			if (!coroutineRunning) {
				StartCoroutine(WaitForNextJump());
				coroutineRunning = true;
			}
			if (fuckingBoolean) {
				playerRig.velocity = new Vector3(0, 0, 0);
				platformsJumped += 1;
				playerIsJumping = false;
				fuckingBoolean = false;
				coroutineRunning = false;
				if (platformsJumped == platforms.Count) {
					EndSequence();
				}
			}
		}
	}

	//End the sequence, allows the player to move freely again
	void EndSequence ()
	{
		playerScript.enabled = true;
		playerCam.SetActive(true);
		playerCam.transform.rotation = player.transform.rotation;
		camAnchor.SetActive(false);
		sequenceIsRunning = false;
		enableSequence = false;
		creatureSpawnedPlatforms = false;
		platformsJumped = 0;
	}

	//Make the camera follow the player
	void SeparateCamera ()
	{
		if (sequenceIsRunning) {
			camAnchor.transform.position = new Vector3(Mathf.Lerp(camAnchor.transform.position.x, player.transform.position.x, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.y, player.transform.position.y, cameraSpeed * Time.deltaTime), Mathf.Lerp(camAnchor.transform.position.z, player.transform.position.z, cameraSpeed * Time.deltaTime));
			camAnchor.transform.rotation = player.transform.rotation;
		}
	}

	IEnumerator WaitForNextJump ()
	{
		yield return new WaitForSeconds(0.5f);
		fuckingBoolean = true;
	}

	IEnumerator CreatureDoesTrick ()
	{
		moustacheAnim.SetBool("UseAbility", true);
		yield return new WaitForSeconds(1F);
		moustacheAnim.SetBool("UseAbility", false);
	}
}
