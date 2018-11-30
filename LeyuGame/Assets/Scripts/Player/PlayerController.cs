using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour, ISnowTornado
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;
	Vector2 leftStickInput = new Vector2(0, 0);


	//Animation Settings
	GameObject animationModel;
	Animator animator;
	public bool isBouncing, isPreLaunching, isAirborne, isBuildingLaunch;

	[Header("Camera Settings")]
	public Transform cameraTrans;
	public Vector3 cameraOffset = new Vector3(0, 3, -14), cameraTarget = new Vector3(0, 0, 3);
	public float cameraHorizontalSensitivity = 130, cameraVerticalSensitivity = 90f, cameraXRotationMaxClamp = 50, cameraXRotationMinClamp = -50;
	[Range(0.0f, 1.0f)]
	public float cameraPositionSmooting = .2f;
	float cameraXAngle = 0, cameraYAngle = 0;
	Vector3 cameraDesiredPosition;
	Quaternion cameraRotation;
	RaycastHit cameraRayHit;

	[Header("Launch Settings")]
	public bool canLaunch = true;
	public float launchStageTwoTime = .7f;
	public Vector3 launchStageOneForce = new Vector3(0, 18, 10), launchStageTwoForce = new Vector3(0, 38, 18);
	public Color launchStageOneColor = Color.yellow, launchStageTwoColor = Color.red;
	public Renderer launchRenderer;
	int launchMaterialIndex = 1;
	Color launchBaseColor;
	bool launchRoutineRunning = false;

	[Header("Movement Settings")]
	public float walkingSpeed = 14;
	public Vector3 leapingVelocity = new Vector3(0, 11, 20);
	public float airborneMovementSpeed = 22, airborneMovementAcceleration = 26, airborneDecceleration = 21;
	[Range(0.0f, 1.0f)]
	public float walkingBouncingThreshold = .72f;

	[Header("Hop Settings")]
	public bool canHop = true;
	public float hopVelocity = 9;
	bool disableGravity = false;

	[Header("Gravity Settings")]
	public float gravityStrength = 38;
	public float maximumFallingSpeed = 96;

	//[Header("SnowTornado Settings")]
	bool inTornado = false, canBeisSpinning = true;
	Vector3 snowTornadoDesiredPlayerPosition;

	//Boundary Settings
	[HideInInspector]
	public bool playerIsAirborne, enablePlayerPushBack;
	[HideInInspector]
	public Vector3 boundaryPushingDirection;

	[Header("Twirl Settings")]
	public GameObject twirlModel;
	public bool enableTwirl = true;
	public float twirlTime = .26f;

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

		if (launchRenderer != null)
			launchBaseColor = launchRenderer.materials[launchMaterialIndex].color;
		else
			launchBaseColor = Color.white;

		cameraYAngle = transform.rotation.eulerAngles.y;

		animationModel = GameObject.Find("MOD_Draak");
		animator = animationModel.GetComponent<Animator>();

		foreach (Material m in launchRenderer.materials) {
			m.color = Color.black;
		}
		launchBaseColor = Color.black;
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
		Handheld.Vibrate();
	}

	private void FixedUpdate ()
	{
		if (!inTornado) {
			Gravity();
			RunAnimation();
			Movement();

			//APPLY BOUNDARY PUSHBACK FORCE
			if (enablePlayerPushBack) {
				velocity += boundaryPushingDirection;
				//rig.velocity = velocity;
			} else {
				//RESOLVE VELOCITY
				//rig.velocity = transform.rotation * velocity;
			}
			rig.velocity = transform.rotation * velocity;
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

		cameraTrans.LookAt(transform.position + cameraRotation * cameraTarget);
	}

	void LandingIndicator ()
	{
		landingIndicatorPosition = transform.position;

		landingIndicatorRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.Raycast(landingIndicatorRay, out landingIndicatorRayHit)) {
			landingIndicatorPosition.y = landingIndicatorRayHit.point.y;
		}

		landingIndicatorYRotation = transform.eulerAngles.y;
		landingIndicatorTrans.eulerAngles = new Vector3(0, landingIndicatorYRotation, 0);

		if (useLandingIndicatorOnlyWhenAirborne && Grounded()) {
			landingIndicatorTrans.gameObject.SetActive(false);
		} else {
			landingIndicatorTrans.gameObject.SetActive(true);
		}

		landingIndicatorTrans.position = landingIndicatorPosition;
	}

	void Launch ()
	{
		if (canLaunch && Input.GetAxis("Right Trigger") != 0) {
			if (!launchRoutineRunning) {
				launchRoutineRunning = true;
				StartCoroutine(LaunchRoutine());
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
				//BOUNCE VELOCITY
				velocity = new Vector3(leftStickInput.x, 0, leftStickInput.y) * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
				StartCoroutine(SuspendGroundedCheck());
			}
		} else {
			Vector2 lateralSpeed = new Vector2(velocity.x, velocity.z);

			if (leftStickInput.magnitude == 0) {
				//AIR MOVEMENT WHEN NOT GIVING INPUT
				//airborneDecceleration 21 is too low. 42 seems ok. Tweak this in the inspector.
				if (lateralSpeed.magnitude < .2f)
					lateralSpeed = Vector2.zero;
				else
					lateralSpeed += lateralSpeed.normalized * -airborneDecceleration * Time.fixedDeltaTime;
			} else {
				//AIR MOVEMENT WHEN GIVING INPUT
				Vector2 lateralSpeedGain = leftStickInput.Rotate(Quaternion.Inverse(transform.rotation) * Quaternion.Euler(0, cameraYAngle, 0)) * airborneMovementAcceleration;

				lateralSpeed += lateralSpeedGain * Time.fixedDeltaTime;
				if (lateralSpeed.magnitude > airborneMovementSpeed)
					lateralSpeed = lateralSpeed.normalized * airborneMovementSpeed;
			}

			velocity.x = lateralSpeed.x;
			velocity.z = lateralSpeed.y;
		}
	}

	void Hop ()
	{
		if (canHop) {
			if (Input.GetButtonDown("A Button")) {
				canHop = false;
				if (velocity.y < 0)
					velocity.y = 0;
				velocity.y += hopVelocity;
				GamePad.SetVibration((PlayerIndex) 0, .2f, .2f);
				KillVibration();
				StartCoroutine(SuspendGroundedCheck());
			}
		} else {
			if (Grounded())
				canHop = true;
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
		if (Physics.SphereCast(groundedRay, .42f, .1f)) {
			playerIsAirborne = false;
			return true;
		} else {
			playerIsAirborne = true;
			return false;
		}
	}

	void KillVibration (float timeBeforeKill = .1f)
	{
		StopCoroutine(KillVibrationRoutine());
		StartCoroutine(KillVibrationRoutine(timeBeforeKill));
	}

	void RunAnimation ()
	{
		//Vieze animatie code

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

	//COROUTINES
	IEnumerator LaunchRoutine ()
	{
		float timeLapsed = 0;
		bool stageTwoReached = false;
		
		GamePad.SetVibration(PlayerIndex.One, .1f, .1f);

		launchRenderer.materials[launchMaterialIndex].color = launchStageOneColor;

		while (Input.GetAxis("Right Trigger") != 0) {
			isBuildingLaunch = true;
			timeLapsed += Time.deltaTime;

			if (timeLapsed > launchStageTwoTime) {
				stageTwoReached = true;
				GamePad.SetVibration(PlayerIndex.One, .4f, .4f);
				launchRenderer.materials[launchMaterialIndex].color = launchStageTwoColor;
			}
			yield return null;
		}

		GamePad.SetVibration(PlayerIndex.One, 0.9f, 0.9f);
		KillVibration(.15f);

		if (velocity.y < 0)
			velocity.y = 0;
		isBuildingLaunch = false;

		if (!stageTwoReached)
			velocity = launchStageOneForce;
		else
			velocity = launchStageTwoForce;

		StartCoroutine(PreLaunchRoutine());
		StopCoroutine(SuspendGroundedCheck());
		StartCoroutine(SuspendGroundedCheck());
		StopCoroutine(Twirl());
		if (enableTwirl)
			StartCoroutine(Twirl());

		while (!Grounded()) {
			yield return null;
		}

		StopCoroutine(Twirl());
		launchRenderer.materials[launchMaterialIndex].color = launchBaseColor;
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

	IEnumerator Twirl ()
	{
		if (twirlModel == null)
			yield break;
		while (!Grounded()) {
			twirlModel.transform.Rotate(new Vector3(360 / twirlTime, 0, 0) * Time.deltaTime);
			yield return null;
		}
		twirlModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}



	//SNOW MECHANICS FUNCTIONS
	IEnumerator ISnowTornado.HitBySnowTornado (Transform tornadoTrans, Vector3 playerOffsetFromCenter, float spinSpeed, float playerLerpFactor, Vector3 releaseVelocity)
	{
		if (inTornado && !canBeisSpinning)
			yield break;

		inTornado = true;
		canBeisSpinning = false;

		tornadoTrans.forward = -transform.right;

		while (true) {
			snowTornadoDesiredPlayerPosition = tornadoTrans.forward + playerOffsetFromCenter;
			transform.position = Vector3.Lerp(transform.position, tornadoTrans.position + tornadoTrans.rotation * playerOffsetFromCenter, playerLerpFactor);
			transform.rotation *= Quaternion.Euler(new Vector3(0, spinSpeed * Time.deltaTime, 0));

			if (Input.GetButtonDown("A Button")) {
				velocity = releaseVelocity;
				inTornado = false;
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		canBeisSpinning = true;
	}
}