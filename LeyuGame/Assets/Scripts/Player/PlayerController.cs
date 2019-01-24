using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;
	Vector2 leftStickInput = new Vector2(0, 0), rightStickInput = new Vector2(0, 0);
	public float mouseXSensitivity = 1.6f;
	public float mouseYSensitivity = 1.4f;

	//Ability settings per scene
	public const string playerPrefsKey = "LevelSixChoice", playerPrefsNoChoiceMade = "NoChoiceMade", playerPrefsLaunch = "Launch", playerPrefsCreature = "Creature";

	//Animation Settings
	GameObject animationModel;
	Animator animator;
	[HideInInspector]
	public bool isBouncing, isPreLaunching, isAirborne, isBuildingLaunch, isHopping, isLaunchingSuperSaiyan;
	public GameObject dragonModel;
	public LayerMask triggerMask;

	[Header("Camera Settings")]
	public Transform cameraTrans;
	public Vector3 cameraOffset = new Vector3(0, 3, -7.5f), cameraTarget = new Vector3(0, 0, 3);
	public float cameraHorizontalSensitivity = 130, cameraVerticalSensitivity = 90f, cameraXRotationMaxClamp = 50, cameraXRotationMinClamp = -50;
	[Range(0.0f, 1.0f)]
	public float cameraPositionSmooting = .12f, cameraTargetSmoothing = .32f;
	public float cameraVerticalInfluenceThreshold = 14, cameraVerticalInfluenceFactor = .06f;
	public float cameraStationaryYResetSpeed = 30, cameraStationaryXResetSpeed = 15;
	public float cameraSmartYCorrectionBase = 12, cameraSmartYCorrectionRate = 140;
	float cameraVerticalInfluence = 0, cameraXAngle = 0, cameraYAngle = 0;
	Vector3 cameraDesiredPosition, cameraDesiredTarget;
	Quaternion cameraRotation;
	RaycastHit cameraRayHit;
	float cameraRayDistance, cameraObstacleAvoidanceOffset = 0, cameraObstacleAvoidanceMaxOffset = 1.1f, cameraObstacleAvoidanceOffsetLerp = .1f;
	bool cameraObstacleDetected = false;

	[Header("Launch Settings")]
	public bool launchEnabled = true;
	public float launchStageTwoTime = .7f;
	public Vector3 launchStageOneForce = new Vector3(0, 35, 10), launchStageTwoForce = new Vector3(0, 50, 22);
	public Color launchStageOneColor = Color.green, launchStageTwoColor = Color.red;
	public Renderer launchRenderer;
	int[] launchMaterialIndexes = new int[] { 0, 3, 6, 9 };
	Color launchBaseColor = Color.white;
	bool launchRoutineRunning = false;

	[Header("Creature Wall Settings")]
	public bool creatureWallsEnabled = true;
	public float creatureWallJumpSpeed = 40;

	[Header("Model Rotation Settings")]
	public float modelXRotationSpeed = 45;
	public float modelRotationMaximumXAngle = 40, modelRotationMinimumXAngle = -40;
	float modelXRotation = 0, modelYRotation = 0;
	Vector3 modelLateralVelocity;

	[Header("Movement Settings")]
	public GameObject snowLandingParticlePrefab;
	GameObject[] snowLandingParticlePool;
	public Vector3 leapingVelocity = new Vector3(0, 12.5f, 18);
	public Vector3 snowLeapingVelocity = new Vector3(0, 8, 14);
	public float airborneMovementSpeed = 25, snowAirborneMovementSpeed = 14, airborneMovementAcceleration = 50, airborneDecceleration = 56;
	[Range(0.0f, 1.0f)]
	public float walkingBouncingThreshold = .8f;
	bool inSnow = false;
	public float groundType, jumpHeight;
	bool checkCurrentHeight, waitingForNextBounce = false, waitForBounceRoutineRunning = false;
	public bool enableLaunchOnly;

	[Header("Hop Settings")]
	public bool canHop = true;
	public float hopVelocity = 9;
	bool disableGravity = false;

	[Header("Gravity Settings")]
	public float gravityStrength = 48;
	public float maximumFallingSpeed = 112;

	//Boundary Settings
	[HideInInspector]
	public bool playerIsAirborne, enablePlayerPushBack;
	[HideInInspector]
	public Vector3 boundaryPushingDirection;

	//SETUP
	void Start ()
	{
		rig = GetComponent<Rigidbody>();

		snowLandingParticlePool = new GameObject[6];
		for (int i = 0; i < snowLandingParticlePool.Length; i++) {
			snowLandingParticlePool[i] = Instantiate(snowLandingParticlePrefab);
			snowLandingParticlePool[i].hideFlags = HideFlags.HideInHierarchy;
			snowLandingParticlePool[i].SetActive(false);
		}

		cameraRayDistance = cameraOffset.magnitude;
		cameraYAngle = transform.rotation.eulerAngles.y;
		cameraDesiredPosition = transform.position + transform.rotation * cameraOffset;
		cameraTrans.position = cameraDesiredPosition;
		cameraDesiredTarget = transform.position + transform.rotation * cameraTarget;
		cameraTrans.LookAt(cameraDesiredTarget);

		animationModel = GameObject.Find("MOD_Draak");
		animator = animationModel.GetComponent<Animator>();
		launchBaseColor = launchRenderer.materials[launchMaterialIndexes[0]].GetColor("_baseColor");

		GamePad.SetVibration(0, 0, 0);
		Cursor.visible = false;
	}

	//START
	void SetAbilities ()
	{
		int currentSceneIndex;
		currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

		switch (currentSceneIndex) {
			default:
				PlayerPrefs.SetString(playerPrefsKey, playerPrefsNoChoiceMade);
				break;
			case 1:
				PlayerPrefs.SetString(playerPrefsKey, playerPrefsNoChoiceMade);
				creatureWallsEnabled = false;
				launchEnabled = false;
				break;
			case 2:
				PlayerPrefs.SetString(playerPrefsKey, playerPrefsNoChoiceMade);
				creatureWallsEnabled = false;
				launchEnabled = false;
				break;
			case 3:
				PlayerPrefs.SetString(playerPrefsKey, playerPrefsNoChoiceMade);
				creatureWallsEnabled = false;
				launchEnabled = true;
				break;
			case 4:
				PlayerPrefs.SetString(playerPrefsKey, playerPrefsNoChoiceMade);
				creatureWallsEnabled = true;
				launchEnabled = true;
				break;
			case 5:
				PlayerPrefs.SetString(playerPrefsKey, playerPrefsNoChoiceMade);
				creatureWallsEnabled = true;
				launchEnabled = true;
				break;
			case 6:
				switch (PlayerPrefs.GetString(playerPrefsKey)) {
					case playerPrefsNoChoiceMade:
						launchEnabled = true;
						creatureWallsEnabled = true;
						break;
					case playerPrefsLaunch:
						launchEnabled = true;
						creatureWallsEnabled = false;
						break;
					case playerPrefsCreature:
						launchEnabled = false;
						creatureWallsEnabled = true;
						break;
				}
				break;
		}
	}

	//UPDATES
	void Update ()
	{
		creatureWallsEnabled = true;
		ProcessInputs();

		CameraControl();
		Launch();
		Hop();
		ModelRotation();
		if (Grounded())
			animator.SetBool("airborne", false);
		else
			animator.SetBool("airborne", true);
	}

	private void FixedUpdate ()
	{
		Gravity();
		Movement();
		CheckHeight();

		rig.velocity = transform.rotation * velocity;
		//APPLY BOUNDARY PUSHBACK FORCE
		if (enablePlayerPushBack) {
			rig.velocity += boundaryPushingDirection;
		}
	}

	private void OnApplicationQuit ()
	{
		GamePad.SetVibration(PlayerIndex.One, 0, 0);
	}


	//UPDATE FUNCTIONS
	void ProcessInputs ()
	{
		if (!enableLaunchOnly) {
			leftStickInput = new Vector2(Mathf.Clamp(Input.GetAxis("Left Stick X") + Input.GetAxis("Keyboard AD"), -1, 1), Mathf.Clamp(Input.GetAxis("Left Stick Y") + Input.GetAxis("Keyboard WS"), -1, 1));
			rightStickInput = new Vector2(Mathf.Clamp(Input.GetAxis("Right Stick X") + Input.GetAxis("Mouse X") * mouseXSensitivity, -1, 1), Mathf.Clamp(Input.GetAxis("Right Stick Y") + Input.GetAxis("Mouse Y") * mouseYSensitivity, -1, 1));
		}
	}

	void CameraControl ()
	{
		cameraYAngle += rightStickInput.x * cameraHorizontalSensitivity * Time.deltaTime;
		cameraXAngle = Mathf.Clamp(cameraXAngle - rightStickInput.y * cameraVerticalSensitivity * Time.deltaTime, cameraXRotationMinClamp, cameraXRotationMaxClamp);

		//CAMERA RESET ZN ROTATIE WANNEER SPELER STIL STAAT. NIET TEVREDEN OVER.
		//if (velocity.sqrMagnitude <= 1) {
		//	if (cameraYAngle != modelYRotation) {
		//		cameraYAngle = Mathf.MoveTowards(cameraYAngle, transform.eulerAngles.y + modelYRotation, cameraStationaryYResetSpeed * Time.deltaTime);
		//		modelYRotation = Mathf.MoveTowards(modelYRotation, 0, cameraStationaryYResetSpeed * Time.deltaTime);
		//	}
		//	cameraXAngle = Mathf.MoveTowards(cameraXAngle, 0, cameraStationaryXResetSpeed * Time.deltaTime);
		//}

		//Smart Y rot
		Vector3 lateralVelocity = new Vector3(velocity.x, 0, velocity.z);
		if (lateralVelocity.sqrMagnitude > 64 && rightStickInput.x < .2f) {
			Vector3 cameraOrientation = Quaternion.Euler(0, cameraTrans.eulerAngles.y, 0) * Vector3.forward;
			float angle = Vector3.SignedAngle(cameraOrientation, transform.rotation * lateralVelocity, Vector3.up);
			if (angle < 0) {
				cameraYAngle -= (cameraSmartYCorrectionRate * Mathf.Abs(angle) / 180) * Time.deltaTime;
			} else {
				cameraYAngle += (cameraSmartYCorrectionRate * Mathf.Abs(angle) / 180) * Time.deltaTime;
			}
		}
		cameraRotation = Quaternion.Euler(cameraXAngle, cameraYAngle, 0);

		if (Grounded() && leftStickInput.SqrMagnitude() == 0) {
			transform.rotation = Quaternion.Euler(new Vector3(0, cameraYAngle, 0));
		}

		cameraDesiredPosition = Vector3.Lerp(cameraTrans.position, transform.position + cameraRotation * cameraOffset, cameraPositionSmooting);

		//Camera Obstacle Avoidance
		Quaternion cameraRayRot = Quaternion.Euler(cameraXAngle, cameraYAngle, 0);
		if (cameraObstacleDetected)
			cameraObstacleAvoidanceOffset = Mathf.Lerp(cameraObstacleAvoidanceOffset, cameraObstacleAvoidanceMaxOffset, cameraObstacleAvoidanceOffsetLerp);
		else
			cameraObstacleAvoidanceOffset = Mathf.Lerp(cameraObstacleAvoidanceOffset, 0, cameraObstacleAvoidanceOffsetLerp);
		if (Physics.Raycast(transform.position, cameraRayRot * cameraOffset, out cameraRayHit, cameraRayDistance, triggerMask)) {
			cameraObstacleDetected = true;
			cameraTrans.position = cameraRayHit.point + cameraTrans.forward * (cameraObstacleAvoidanceOffset);
		} else {
			cameraObstacleDetected = false;
			cameraTrans.position = cameraDesiredPosition;
		}

		if (velocity.y < -cameraVerticalInfluenceThreshold || velocity.y > cameraVerticalInfluenceThreshold) {
			if (velocity.y < -cameraVerticalInfluenceThreshold)
				cameraVerticalInfluence = (velocity.y + cameraVerticalInfluenceThreshold) * cameraVerticalInfluenceFactor;
			else
				cameraVerticalInfluence = (velocity.y - cameraVerticalInfluenceThreshold) * cameraVerticalInfluenceFactor;
		} else {
			cameraVerticalInfluence = 0;
		}
		cameraDesiredTarget = Vector3.Lerp(cameraDesiredTarget, transform.position + cameraRotation * (cameraTarget + new Vector3(0, cameraVerticalInfluence, 0)), cameraTargetSmoothing);
		cameraTrans.LookAt(cameraDesiredTarget);
	}

	void Launch ()
	{
		if (launchEnabled && (Input.GetAxis("Right Trigger") != 0 || Input.GetButtonDown("Keyboard Space"))) {
			if (!launchRoutineRunning) {
				launchRoutineRunning = true;
				animator.SetBool("preLaunching", true);
				StartCoroutine(LaunchRoutine());
			}
		}
	}

	void ModelRotation ()
	{
		if (velocity.y > 0)
			modelXRotation += -modelXRotationSpeed * Time.deltaTime;
		else
			modelXRotation += modelXRotationSpeed * Time.deltaTime;
		modelXRotation = Mathf.Clamp(modelXRotation, modelRotationMinimumXAngle, modelRotationMaximumXAngle);

		modelLateralVelocity = new Vector3(velocity.x, 0, velocity.z);
		if (modelLateralVelocity.magnitude > .1f) {
			modelYRotation = Vector3.Angle(Vector3.forward, modelLateralVelocity);
			if (velocity.x < 0)
				modelYRotation *= -1;
		}
		if (Grounded()) {
			modelXRotation = Mathf.MoveTowards(modelXRotation, 0, modelXRotationSpeed * 2 * Time.deltaTime);
			modelYRotation -= rightStickInput.x * Time.deltaTime * cameraHorizontalSensitivity;
		}
		
		dragonModel.transform.rotation = transform.rotation * Quaternion.Euler(modelXRotation, modelYRotation, 0);
	}

	void Hop ()
	{
		if (canHop && !enableLaunchOnly) {
			if (Input.GetButtonDown("A Button") || Input.GetButton("Left Mouse Button")) {
				canHop = false;
				isHopping = true;
				if (velocity.y < 0)
					velocity.y = 0;
				velocity.y += hopVelocity;
				GamePad.SetVibration(0, .2f, .2f);
				KillVibration();
				StartCoroutine(SuspendGroundedCheck());
			}
		} else {
			if (Grounded()) {
				canHop = true;
			}
		}
	}

	//FIXED UPDATE FUNCTIONS
	void Movement ()
	{
		if (Grounded()) {
			if (waitingForNextBounce) {
				if (groundType == 1.5f) {
					if (!waitForBounceRoutineRunning) {
						waitForBounceRoutineRunning = true;
						foreach (GameObject g in snowLandingParticlePool) {
							if (!g.activeSelf) {
								g.transform.position = transform.position - Vector3.up * .5f;
								g.SetActive(true);
								StartCoroutine(DisableSnowLandingParticle(g));
								break;
							}
						}
						StartCoroutine(BouncePause());
					}
				} else {
					waitingForNextBounce = false;
				}
			}

			if (leftStickInput.magnitude == 0 || waitingForNextBounce) {
				velocity.x = velocity.z = 0;
			} else {
				Vector3 velocityDirection = Quaternion.Inverse(transform.rotation) * Quaternion.Euler(0, cameraYAngle, 0) * new Vector3(leftStickInput.x, 0, leftStickInput.y);
				velocity = velocityDirection.normalized * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
				StartCoroutine(SuspendGroundedCheck());
			}
		} else {
			//ELSE IF NOT GROUNDED
			Vector2 lateralSpeed = new Vector2(velocity.x, velocity.z);

			if (leftStickInput.magnitude == 0) {
				//AIR MOVEMENT WHEN NOT GIVING INPUT
				if (lateralSpeed.SqrMagnitude() < 1)
					lateralSpeed = Vector2.zero;
				else
					lateralSpeed += lateralSpeed.normalized * -airborneDecceleration * Time.fixedDeltaTime;
			} else {
				//AIR MOVEMENT WHEN GIVING INPUT
				Vector2 lateralSpeedGain = leftStickInput.Rotate(Quaternion.Inverse(transform.rotation) * Quaternion.Euler(0, cameraYAngle, 0)) * airborneMovementAcceleration;
				lateralSpeed += lateralSpeedGain * Time.fixedDeltaTime;
				if (!inSnow) {
					if (lateralSpeed.magnitude > airborneMovementSpeed)
						lateralSpeed = lateralSpeed.normalized * airborneMovementSpeed;
				} else {
					if (lateralSpeed.magnitude > snowAirborneMovementSpeed)
						lateralSpeed = lateralSpeed.normalized * snowAirborneMovementSpeed;
				}
			}

			waitingForNextBounce = true;

			velocity.x = lateralSpeed.x;
			velocity.z = lateralSpeed.y;
		}
	}

	public void Gravity ()
	{
		if (!Grounded()) {
			if (velocity.y > -maximumFallingSpeed)
				velocity.y -= gravityStrength * Time.fixedDeltaTime;

			Ray ceilingDetectRay = new Ray(transform.position, transform.up);
			if (Physics.SphereCast(ceilingDetectRay, .2f, .35f, triggerMask)) {
				if (velocity.y > 0)
					velocity.y = 0;
			}
		} else {
			velocity.y = 0;
		}
	}

	//RETURN FUNCTIONS
	bool Grounded ()
	{
		if (groundedSuspended) {
			return false;
		}

		Ray groundedRay = new Ray(transform.position, Vector3.up * -1);
		RaycastHit groundedRayHit;
		if (Physics.SphereCast(groundedRay, .42f, out groundedRayHit, .1f, triggerMask)) {
			playerIsAirborne = false;

			//CHECK GROUND TYPE
			if (groundedRayHit.transform.tag == "Snow") {
				inSnow = true;
				groundType = 1.5f;
			} else {
				inSnow = false;
			}
			if (groundedRayHit.transform.tag == "Rock") {
				groundType = 0;
			}
			if (groundedRayHit.transform.tag == "Amethyst") {
				groundType = 3;
			}

			//beetje lelijk dit
			canHop = true;
			if (!isBuildingLaunch) {
				for (int i = 0; i < launchMaterialIndexes.Length; i++) {
					launchRenderer.materials[launchMaterialIndexes[i]].SetColor("_baseColor", launchBaseColor);
				}
			}

			return true;
		} else {
			playerIsAirborne = true;
			return false;
		}
	}

	void CheckHeight ()
	{
		if (velocity.y < 0) {
			if (!checkCurrentHeight) {
				Ray checkHeightRay = new Ray(transform.position, Vector3.up * -1);
				if (Physics.Raycast(checkHeightRay, 20f)) {
				} else {
					jumpHeight = 1;
				}
				checkCurrentHeight = true;
			}
		}
		checkCurrentHeight = false;
	}

	void KillVibration (float timeBeforeKill = .1f)
	{
		StopCoroutine(KillVibrationRoutine());
		StartCoroutine(KillVibrationRoutine(timeBeforeKill));
	}

	public void DisablePlayer (bool disableCamera = false)
	{
		//DISABLE PLAYER ANIMATION
		animator.SetTrigger("reset");
		animator.SetBool("preLaunching", false);
		animator.SetBool("airborne", false);

		//DISABLE PLAYER MOVEMENT
		leftStickInput = Vector2.zero;
		rightStickInput = Vector2.zero;
		velocity = new Vector3(0, 0, 0);
		StopCoroutine(LaunchRoutine());
		launchRoutineRunning = false;
		for (int i = 0; i < launchMaterialIndexes.Length; i++) {
			launchRenderer.materials[launchMaterialIndexes[i]].SetColor("_baseColor", launchBaseColor);
		}
		transform.rotation = Quaternion.Euler(0, 0, 0);
		dragonModel.transform.rotation = Quaternion.identity;
		modelYRotation = 0;
		modelXRotation = 0;
		rig.velocity = Vector3.zero;
		cameraTrans.gameObject.SetActive(!disableCamera);
		enabled = false;
	}

	public void EnablePlayer ()
	{
		enabled = true;
		cameraYAngle = transform.eulerAngles.y;
		cameraDesiredTarget = transform.position + transform.rotation * cameraTarget;
		cameraTrans.LookAt(cameraDesiredTarget);
		cameraTrans.gameObject.SetActive(true);
		modelYRotation = 0;
	}

	//COROUTINES
	IEnumerator BouncePause ()
	{
		yield return new WaitForSeconds(.05f);
		waitingForNextBounce = waitForBounceRoutineRunning = false;
	}

	IEnumerator DisableSnowLandingParticle (GameObject go)
	{
		yield return new WaitForSeconds(go.GetComponent<ParticleSystem>().main.duration);
		go.SetActive(false);
	}

	IEnumerator LaunchRoutine ()
	{
		float timeLapsed = 0;
		bool stageTwoReached = false;

		GamePad.SetVibration(PlayerIndex.One, .1f, .1f);

		for (int i = 0; i < launchMaterialIndexes.Length; i++) {
			launchRenderer.materials[launchMaterialIndexes[i]].SetColor("_baseColor", launchStageOneColor);
		}

		while (Input.GetAxis("Right Trigger") != 0 || Input.GetButton("Keyboard Space")) {
			isBuildingLaunch = true;
			timeLapsed += Time.deltaTime;

			if (timeLapsed > launchStageTwoTime) {
				stageTwoReached = true;
				GamePad.SetVibration(PlayerIndex.One, .3f, .3f);
				for (int i = 0; i < launchMaterialIndexes.Length; i++) {
					launchRenderer.materials[launchMaterialIndexes[i]].SetColor("_baseColor", launchStageTwoColor);
				}
			}
			yield return null;
		}

		GamePad.SetVibration(PlayerIndex.One, 0.8f, 0.8f);
		KillVibration(.15f);
		animator.SetBool("preLaunching", false);

		if (velocity.y < 0)
			velocity.y = 0;
		isBuildingLaunch = false;

		if (!stageTwoReached) {
			//stage 1
			isLaunchingSuperSaiyan = true;
			velocity = new Vector3(velocity.x, 0, velocity.z).normalized * launchStageOneForce.z;
			velocity.y = launchStageOneForce.y;
		} else {
			//stage 2
			isLaunchingSuperSaiyan = false;
			velocity = new Vector3(velocity.x, 0, velocity.z).normalized * launchStageTwoForce.z;
			velocity.y = launchStageTwoForce.y;
		}
		canHop = true;

		StartCoroutine(PreLaunchRoutine());
		StopCoroutine(SuspendGroundedCheck());
		StartCoroutine(SuspendGroundedCheck());

		while (!Grounded()) {
			yield return null;
		}

		for (int i = 0; i < launchMaterialIndexes.Length; i++) {
			launchRenderer.materials[launchMaterialIndexes[i]].SetColor("_baseColor", launchBaseColor);
		}
		launchRoutineRunning = false;
	}

	IEnumerator PreLaunchRoutine ()
	{
		isPreLaunching = true;
		yield return new WaitForSeconds(0.2F);
		isPreLaunching = false;
	}

	IEnumerator SuspendGroundedCheck (float suspensionTime = .1f)
	{
		groundedSuspended = true;
		yield return new WaitForSeconds(suspensionTime);
		groundedSuspended = false;
	}

	IEnumerator KillVibrationRoutine (float timeBeforeKill = 0.1f)
	{
		yield return new WaitForSeconds(timeBeforeKill);
		GamePad.SetVibration(0, 0, 0);
	}
}