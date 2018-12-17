using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;
	Vector2 leftStickInput = new Vector2(0, 0);


	//Animation Settings
	GameObject animationModel;
	Animator animator;
	[HideInInspector]
	public bool isBouncing, isPreLaunching, isAirborne, isBuildingLaunch, isHopping, isLaunchingSuperSaiyan;
	public GameObject dragonModel;

	[Header("Camera Settings")]
	public Transform cameraTrans;
	public Vector3 cameraOffset = new Vector3(0, 3, -7.5f), cameraTarget = new Vector3(0, 0, 3);
	public float cameraHorizontalSensitivity = 130, cameraVerticalSensitivity = 90f, cameraXRotationMaxClamp = 50, cameraXRotationMinClamp = -50;
	[Range(0.0f, 1.0f)]
	public float cameraPositionSmooting = .12f, cameraTargetSmoothing = .32f;
	public float cameraVerticalInfluenceThreshold = 14, cameraVerticalInfluenceFactor = .06f;
	float cameraVerticalInfluence = 0, cameraXAngle = 0, cameraYAngle = 0;
	Vector3 cameraDesiredPosition, cameraDesiredTarget;
	Quaternion cameraRotation;
	RaycastHit cameraRayHit;

	[Header("Launch Settings")]
	public bool launchEnabled = true;
	public float launchStageTwoTime = .7f;
	public Vector3 launchStageOneForce = new Vector3(0, 35, 10), launchStageTwoForce = new Vector3(0, 50, 22);
	public Color launchStageOneColor = Color.green, launchStageTwoColor = Color.red;
	public Renderer launchRenderer;
	int[] launchMaterialIndexes = new int[] { 1, 3, 4 };
	Color launchBaseColor = Color.white;
	bool launchRoutineRunning = false;

	[Header("Creature Wall Settings")]
	public bool creatureWallsEnabled = true;
	public float creatureWallJumpSpeed = 40;

	[Header("Model Rotation Settings")]
	public float modelRotationLerpFactor = .24f;
	public float modelRotationMaximumXAngle = 40, modelRotationMinimumXAngle = -40;
	Quaternion modelRotationDesiredRotation;
	float modelXRotation, modelYRotation, modelZRotation;

	[Header("Movement Settings")]
	public Vector3 leapingVelocity = new Vector3(0, 12.5f, 18);
	public Vector3 snowLeapingVelocity = new Vector3(0, 8, 14);
	public float airborneMovementSpeed = 25, snowAirborneMovementSpeed = 14, airborneMovementAcceleration = 50, airborneDecceleration = 56;
	[Range(0.0f, 1.0f)]
	public float walkingBouncingThreshold = .8f;
	bool inSnow = false;
	public float groundType, jumpHeight;
	bool checkCurrentHeight;

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

	[Header("Landing Indicator Settings")]
	public Transform landingIndicatorTrans;
	public bool useLandingIndicator = true, useLandingIndicatorOnlyWhenAirborne = false;
	Vector3 landingIndicatorPosition;
	float landingIndicatorYRotation;
	Ray landingIndicatorRay;
	RaycastHit landingIndicatorRayHit;

	//SETUP
	void Start ()
	{
		rig = GetComponent<Rigidbody>();

		cameraYAngle = transform.rotation.eulerAngles.y;
		cameraDesiredPosition = transform.position + transform.rotation * cameraOffset;
		cameraTrans.position = cameraDesiredPosition;
		cameraDesiredTarget = transform.position + transform.rotation * cameraTarget;
		cameraTrans.LookAt(cameraDesiredTarget);

		animationModel = GameObject.Find("MOD_Draak");
		animator = animationModel.GetComponent<Animator>();
		launchBaseColor = launchRenderer.materials[launchMaterialIndexes[0]].color;

		GamePad.SetVibration(0, 0, 0);
	}

	//UPDATES
	void Update ()
	{
		ProcessInputs();

		CameraControl();
		if (useLandingIndicator)
			LandingIndicator();
		Launch();
		Hop();
		ModelRotation();
	}

	private void FixedUpdate ()
	{
		Gravity();
		RunAnimation();
		Movement();
		CheckHeight();

		rig.velocity = transform.rotation * velocity;
		//APPLY BOUNDARY PUSHBACK FORCE
		if (enablePlayerPushBack) {
			rig.velocity += boundaryPushingDirection;
		}
	}


	//UPDATE FUNCTIONS
	void ProcessInputs ()
	{
		leftStickInput = new Vector2(Input.GetAxis("Left Stick X"), Input.GetAxis("Left Stick Y"));
	}

	void CameraControl ()
	{
		cameraYAngle += Input.GetAxis("Right Stick X") * cameraHorizontalSensitivity * Time.deltaTime;
		cameraXAngle = Mathf.Clamp(cameraXAngle - Input.GetAxis("Right Stick Y") * cameraVerticalSensitivity * Time.deltaTime, cameraXRotationMinClamp, cameraXRotationMaxClamp);
		cameraRotation = Quaternion.Euler(cameraXAngle, cameraYAngle, 0);

		if (Grounded()) {
			transform.rotation = Quaternion.Euler(new Vector3(0, cameraYAngle, 0));
		}

		cameraDesiredPosition = Vector3.Lerp(cameraTrans.position, transform.position + cameraRotation * cameraOffset, cameraPositionSmooting);

		if (Physics.Raycast(transform.position, cameraDesiredPosition - transform.position, out cameraRayHit, Vector3.Distance(transform.position, cameraDesiredPosition))) {
			cameraTrans.position = Vector3.Lerp(cameraTrans.position, cameraRayHit.point, .55f);
		} else {
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

	void LandingIndicator ()
	{
		landingIndicatorPosition = transform.position;

		landingIndicatorRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.Raycast(landingIndicatorRay, out landingIndicatorRayHit)) {
			landingIndicatorPosition.y = landingIndicatorRayHit.point.y;
			landingIndicatorTrans.up = landingIndicatorRayHit.normal;
		}

		if (useLandingIndicatorOnlyWhenAirborne && Grounded()) {
			landingIndicatorTrans.gameObject.SetActive(false);
		} else {
			landingIndicatorTrans.gameObject.SetActive(true);
		}

		landingIndicatorTrans.position = landingIndicatorPosition;
	}

	void Launch ()
	{
		if (launchEnabled && Input.GetAxis("Right Trigger") != 0) {
			if (!launchRoutineRunning) {
				launchRoutineRunning = true;
				StartCoroutine(LaunchRoutine());
			}
		}
	}
	
	void ModelRotation ()
	{
		modelXRotation = Vector3.Angle(Vector3.forward, new Vector3(0, velocity.y, velocity.z));
		if (velocity.y > 0)
			modelXRotation = Mathf.Abs(modelXRotation) * -1;
		modelXRotation = Mathf.Clamp(modelXRotation, modelRotationMinimumXAngle, modelRotationMaximumXAngle);

		Vector3 lateralVelocity = new Vector3(velocity.x, 0, velocity.z);
		if (lateralVelocity.magnitude > .1f) {
			modelYRotation = Vector3.Angle(Vector3.forward, lateralVelocity);
			if (velocity.x < 0)
				modelYRotation = Mathf.Abs(modelYRotation) * -1;
		}
		if (Grounded()) {
			modelYRotation -= Input.GetAxis("Right Stick X") * Time.deltaTime * cameraHorizontalSensitivity;
		}



		modelRotationDesiredRotation = transform.rotation * Quaternion.Euler(modelXRotation, modelYRotation, 0);
		dragonModel.transform.rotation = Quaternion.Lerp(dragonModel.transform.rotation, modelRotationDesiredRotation, modelRotationLerpFactor);
		if (transform.eulerAngles.z < -10 || transform.eulerAngles.z > 10)
			Debug.Log("Transform");
		if (dragonModel.transform.eulerAngles.z < -10 || dragonModel.transform.eulerAngles.z > 10)
			Debug.Log("Dragon");
	}

	void Hop ()
	{
		if (canHop) {
			if (Input.GetButtonDown("A Button")) {
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
			if (leftStickInput.magnitude == 0) {
				velocity.x = velocity.z = 0;
			} else {
				velocity = new Vector3(leftStickInput.x, 0, leftStickInput.y) * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
				StartCoroutine(SuspendGroundedCheck());
			}
		} else {
			Vector2 lateralSpeed = new Vector2(velocity.x, velocity.z);

			if (leftStickInput.magnitude == 0) {
				//AIR MOVEMENT WHEN NOT GIVING INPUT
				if (lateralSpeed.magnitude < 1)
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

			velocity.x = lateralSpeed.x;
			velocity.z = lateralSpeed.y;
		}
	}

	void Gravity ()
	{
		if (!Grounded()) {
			if (velocity.y > -maximumFallingSpeed)
				velocity.y -= gravityStrength * Time.fixedDeltaTime;

			Ray ceilingDetectRay = new Ray(transform.position, transform.up);
			if (Physics.SphereCast(ceilingDetectRay, .4f, .15f)) {
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
		if (Physics.SphereCast(groundedRay, .42f, out groundedRayHit, .1f)) {
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
					launchRenderer.materials[launchMaterialIndexes[i]].color = launchBaseColor;
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
				if (Physics.Raycast(checkHeightRay, 10f)) {
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

	void RunAnimation ()
	{
		//Set Animation States
		if (Grounded()) {
			if (leftStickInput.magnitude == 0) {
				isBouncing = false;
			} else if (leftStickInput.magnitude < walkingBouncingThreshold) {
				if (!isPreLaunching) {
					isBouncing = true;
				} else {
					isBouncing = false;
				}
			} else {
				if (!isPreLaunching) {
					isBouncing = true;
				} else {
					isBouncing = false;
				}
			}
		}

		//Play Bounce Animation
		if (isBouncing) {
			animator.SetBool("IsBouncing", true);
		} else {
			animator.SetBool("IsBouncing", false);
		}
		//Play prelaunch
		if (isPreLaunching) {
			animator.SetBool("IsLaunching", true);
		} else {
			animator.SetBool("IsLaunching", false);
		}
		//Play Airborne
		if (Grounded()) {
			animator.SetBool("IsAirborne", false);
		} else {
			animator.SetBool("IsAirborne", true);
		}
	}

	public void DisablePlayer ()
	{
		//DISABLE PLAYER ANIMATION
		animator.SetBool("IsBouncing", false);
		animator.SetBool("IsLaunching", false);
		animator.SetBool("IsAirborne", false);

		//DISABLE PLAYER MOVEMENT
		leftStickInput = Vector3.zero;
		velocity = new Vector3(0, 0, 0);
		transform.rotation = Quaternion.Euler(0, 0, 0);
		rig.velocity = Vector3.zero;
		enabled = false;
	}

	public void EnablePlayer ()
	{
		enabled = true;
		cameraYAngle = transform.eulerAngles.y;
		cameraDesiredTarget = transform.position + transform.rotation * cameraTarget;
		cameraTrans.LookAt(cameraDesiredTarget);
		modelYRotation = 0;
	}

	//COROUTINES
	IEnumerator LaunchRoutine ()
	{
		float timeLapsed = 0;
		bool stageTwoReached = false;

		GamePad.SetVibration(PlayerIndex.One, .1f, .1f);

		for (int i = 0; i < launchMaterialIndexes.Length; i++) {
			launchRenderer.materials[launchMaterialIndexes[i]].color = launchStageOneColor;
		}

		while (Input.GetAxis("Right Trigger") != 0) {
			isBuildingLaunch = true;
			timeLapsed += Time.deltaTime;

			if (timeLapsed > launchStageTwoTime) {
				stageTwoReached = true;
				GamePad.SetVibration(PlayerIndex.One, .3f, .3f);
				for (int i = 0; i < launchMaterialIndexes.Length; i++) {
					launchRenderer.materials[launchMaterialIndexes[i]].color = launchStageTwoColor;
				}
			}
			yield return null;
		}

		GamePad.SetVibration(PlayerIndex.One, 0.8f, 0.8f);
		KillVibration(.15f);

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
			launchRenderer.materials[launchMaterialIndexes[i]].color = launchBaseColor;
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
		GamePad.SetVibration((PlayerIndex) 0, 0, 0);
	}
}