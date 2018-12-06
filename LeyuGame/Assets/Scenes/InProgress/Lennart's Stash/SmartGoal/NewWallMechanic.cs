using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour
{

	[Header("Player Settings")]
	public int playerLerpSpeed = 5;
	public int playerJumpSpeed = 40;

    GameObject player;
	GameObject playerModel;
	PlayerController playerScript;
	Rigidbody playerRig;
	Animator playerAnim;

	Vector3 playerPositionLerp;
	Vector3 playerMovementTarget;
	Vector3 playerDistanceToPlatform;

	[Header("Platform Settings")]
	public GameObject platformsObject;
	public List<GameObject> platforms = new List<GameObject>();

	[Header("Other Settings")]
	public int triggerAbilityRange = 10;

	[Header("Social Events")]
	public GameObject beforeSequenceSocialPrefab;
	public GameObject duringSequenceSocialPrefab, afterSequenceSocialPrefab;

	//CREATURE
	public GameObject moustacheBoi;
	Animator moustacheAnim;

	//UI
	public GameObject pressButtonPopup;
	public GameObject sequenceCamera;

    //MANAGER
    public GameObject playerNose;
	bool enableSequence, creatureSpawnsPlatforms, sequenceIsRunning, playerIsJumping;
	int platformsReached;

	private void Awake ()
	{
		player = GameObject.Find("Character");
		playerModel = GameObject.Find("MOD_Draak");
		playerScript = player.GetComponent<PlayerController>();
		playerRig = player.GetComponent<Rigidbody>();
		playerAnim = playerModel.GetComponent<Animator>();

		moustacheAnim = moustacheBoi.GetComponent<Animator>();

		if (beforeSequenceSocialPrefab != null) {

		}
	}

	private void FixedUpdate ()
	{
		TriggerSequence();
		StartSequence();
		StartJump();
		MakeJump();
	}

    //INSPECTOR HELP
    Transform platformsParent;

    private void OnDrawGizmosSelected()
    {
        platformsParent = transform.parent.GetChild(1);

        platforms = new List<GameObject>();
        for (int i = 0; i < platformsParent.childCount; i++)
        {
            platforms.Add(platformsParent.GetChild(i).gameObject);
        }
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

    void StartSequence ()
	{
		if (enableSequence) {
			if (Input.GetButtonDown("A Button") && !creatureSpawnsPlatforms) {
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
				sequenceCamera.transform.position = player.transform.position;
				sequenceCamera.transform.position += sequenceCamera.transform.rotation * new Vector3(0, 8, -10);
				sequenceCamera.transform.LookAt(platforms[0].transform);

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
			sequenceCamera.transform.LookAt(player.transform);

			playerMovementTarget = platforms[platformsReached].transform.position;
			playerPositionLerp = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, playerMovementTarget.y, playerLerpSpeed * Time.deltaTime), player.transform.position.z);
            player.transform.position = Vector3.MoveTowards(playerPositionLerp, playerMovementTarget, playerJumpSpeed * Time.deltaTime);
            playerDistanceToPlatform = player.transform.position - playerMovementTarget;
            playerDistanceToPlatform = new Vector3(Mathf.Abs(playerDistanceToPlatform.x), playerDistanceToPlatform.y, playerDistanceToPlatform.z);
            if (playerDistanceToPlatform.x < 0.5f) { 
                playerRig.velocity = new Vector3(0, 0, 0);
				platformsReached += 1;
                if (platformsReached == platforms.Count) {
					EndSequence();
				}
				playerIsJumping = false;
			}
		}
	}

	void EndSequence ()
	{
        player.transform.rotation = playerNose.transform.rotation;
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
        playerScript.enabled = true;
        playerRig.velocity = new Vector3(0, 0, 0);

        platformsObject.SetActive(false);
		sequenceCamera.SetActive(false);
		creatureSpawnsPlatforms = false;
		sequenceIsRunning = false;
		platformsReached = 0;
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