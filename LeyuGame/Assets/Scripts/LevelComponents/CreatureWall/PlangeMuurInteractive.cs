using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

namespace Creature
{
	public class PlangeMuurInteractive : MonoBehaviour
	{
		public enum SequenceActivities { Sneeze, WelcomeBack, Flop, Superflop, None };
		public SequenceActivities sequenceActivity = SequenceActivities.None;

		[HideInInspector]
		public bool startEvent;

		public static int currentCreatureLocation = 0;

		GameObject player;
		GameObject playerModel;
		PlayerController playerScript;
		Rigidbody playerRig;
		Animator playerAnim;

		float jumpingSpeed = 40, jumpHeight = 3;
		float playerPlatformOffset = .7f;

		[HideInInspector]
		public GameObject moustacheBoi;
		Animator moustacheAnimator;

		[HideInInspector]
		public GameObject creatureFlyInPositionObject;
		[HideInInspector]
		public GameObject platformsObject;

		//CREATURE MATERIALS
		[HideInInspector]
		public Material defaultMaterial, glowingMaterial;
		Renderer creatureRenderer;

		[Header("Platform Settings")]
		public float platformCreationTime = .5f;
		public float platformCreationDistance = 7f;
		List<Transform> platformTransforms = new List<Transform>();
		List<Vector3> platformDefaultPositions = new List<Vector3>();
        public GameObject finalCreatureLocation;

		[Header("Flying Settings")]
		public float flyingSpeed = 50;
		public float flyInOutRange = 45;
		[HideInInspector]
		public Vector3 flyInOutPoint = new Vector3(0, 40, -7);
		Vector3 defaultCreaturePos, flyInPosition, flyToPlatformPosition;
		Quaternion defaultCreatureRot;
		bool flyingRoutineRunning = false;

		[Header("Social Events")]
		[HideInInspector]
		public GameObject sneezePrefab, welcomeBackPrefab, flopPrefab, superflopPrefab;
		bool readyForSequence = false, afterSequenceEventPlayed = false, readyToAdvance = false;

		//MANAGER
		int activePlatform = 0;
		[HideInInspector]
		public GameObject spawnPlatformParticle;
		[HideInInspector]
		public bool sequenceIsRunning, creatureBecamePiccolo;

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
			creatureRenderer = moustacheBoi.GetComponentInChildren<Renderer>();
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

		private void Update ()
		{
			CheckForFlying();
			if (startEvent) {
				StartCoroutine(PlayEvent());
			}
		}

		IEnumerator PlayEvent ()
		{
			//EVENT
			switch (sequenceActivity) {
				case SequenceActivities.None:
					readyForSequence = true;
					break;
				case SequenceActivities.Sneeze:
					sneezePrefab.GetComponent<ISocialEncounter>().Initialize(() => {
						sneezePrefab.GetComponent<ISocialEncounter>().Execute(() => {
							sneezePrefab.GetComponent<ISocialEncounter>().End(() => {
								readyForSequence = true;
							});
						});
					});
					break;
				case SequenceActivities.WelcomeBack:
					welcomeBackPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
						welcomeBackPrefab.GetComponent<ISocialEncounter>().Execute(() => {
							welcomeBackPrefab.GetComponent<ISocialEncounter>().End(() => {
								readyForSequence = true;
							});
						});
					});
					break;
				case SequenceActivities.Flop:
					flopPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
						flopPrefab.GetComponent<ISocialEncounter>().Execute(() => {
							flopPrefab.GetComponent<ISocialEncounter>().End(() => {
								readyForSequence = true;
							});
						});
					});
					break;
				case SequenceActivities.Superflop:
					superflopPrefab.GetComponent<ISocialEncounter>().Initialize(() => {
						superflopPrefab.GetComponent<ISocialEncounter>().Execute(() => {
							superflopPrefab.GetComponent<ISocialEncounter>().End(() => {
								readyForSequence = true;
							});
						});
					});
					break;
			}
			while (!readyForSequence) {
				yield return null;
			}
			startEvent = false;
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

		IEnumerator FlyIn ()
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

		IEnumerator FlyOut ()
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

		IEnumerator CreatureSpawnsFirstPlatform ()
		{
			//SPAWN
			creatureRenderer.material = glowingMaterial;
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
			creatureRenderer.material = defaultMaterial;
			sequenceIsRunning = true;
		}

		public void NewPlatform (bool playerOnPlatform)
		{
			activePlatform += 1;
            if (!creatureBecamePiccolo)
            {
                if (activePlatform < platformTransforms.Count - 1)
                {
                    StartCoroutine(CreatureSpawnsPlatform(activePlatform));
                }
                else
                {
                    StartCoroutine(CreatureFliesToPlatform());
                }
            }
        }

		IEnumerator CreatureFliesToPlatform ()
		{
			if (creatureBecamePiccolo) {
			} else {
				if (activePlatform < platformTransforms.Count - 1) {
                    flyToPlatformPosition = platformTransforms[activePlatform + 1].position + platformTransforms[activePlatform + 1].transform.rotation * new Vector3(0, -2, -12);
				} else {
                    flyToPlatformPosition = finalCreatureLocation.transform.position;
				}
				while (moustacheBoi.transform.position.SquareDistance(flyToPlatformPosition) > .1f) {
					moustacheBoi.transform.LookAt(player.transform.position);
					moustacheBoi.transform.position = Vector3.MoveTowards(moustacheBoi.transform.position, flyToPlatformPosition, (jumpingSpeed * 2f) * Time.deltaTime);
					yield return null;
				}
                if (activePlatform >= platformTransforms.Count - 1)
                {
                    moustacheAnimator.SetBool("isFlying", false);
                }
			}
		}

		IEnumerator CreatureSpawnsPlatform (int currentPlatform)
		{
            Debug.Log(activePlatform);
			creatureRenderer.material = glowingMaterial;
			PlatformType platformTypeScript;
			platformTypeScript = platformTransforms[currentPlatform].GetComponent<PlatformType>();
			if (platformTypeScript.platformIsElevator) {
				creatureBecamePiccolo = true;
				moustacheAnimator.SetBool("isFlying", false);
			}
			//FLY TO NEXT PLATFORM
			if (!creatureBecamePiccolo) {
				StartCoroutine(CreatureFliesToPlatform());
			}
			MoustacheBoiAudio.PlayRumble();
            GamePad.SetVibration(0, .6f, .6f);
			GameObject particle = Instantiate(spawnPlatformParticle, new Vector3(flyToPlatformPosition.x, flyToPlatformPosition.y - 5, flyToPlatformPosition.z), Quaternion.Euler(0, 5, 5));
			for (float t = 0; t < platformCreationTime; t += Time.deltaTime) {
				if (platformTypeScript.emergeFromTheGround) {
					platformTransforms[currentPlatform].position -= platformTransforms[currentPlatform].rotation * new Vector3(0, -platformCreationDistance, 0) / platformCreationTime * Time.deltaTime;
				} else {
					platformTransforms[currentPlatform].position -= platformTransforms[currentPlatform].rotation * new Vector3(0, 0, platformCreationDistance) / platformCreationTime * Time.deltaTime;
				}
				yield return null;
			}
			platformTransforms[currentPlatform].position = platformDefaultPositions[currentPlatform];
			GamePad.SetVibration(0, .6f, .6f);
			GamePad.SetVibration(0, 0, 0);
			creatureRenderer.material = defaultMaterial;
		}

		public void DisablePiccolo ()
		{
			if (sequenceIsRunning) {
				creatureBecamePiccolo = false;
                moustacheAnimator.SetBool("isFlying", true);
                Debug.Log(platformTransforms[activePlatform]);
                flyToPlatformPosition = platformTransforms[activePlatform].position + platformTransforms[activePlatform].transform.rotation * new Vector3(0, -2, -12);
                StartCoroutine(CreatureSpawnsPlatform(activePlatform));
			}
		}

		IEnumerator EndSequence ()
		{
			sequenceIsRunning = false;
			creatureBecamePiccolo = false;
			activePlatform = 0;
			for (int i = 0; i < platformTransforms.Count - 1; i++) {
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
}