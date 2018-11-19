using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerInProgress : MonoBehaviour, ISnowTornado
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;
	public bool disableRotation;

	[Header("Camera Settings")]
	public Transform cameraTrans;
	public Vector3 cameraOffset = new Vector3(0, 3, -14), cameraTarget = new Vector3(0, 0, 3);
	public float cameraHorizontalSensitivity = 130, cameraVerticalSensitivity = 90f, cameraXRotationMaxClamp = 50, cameraXRotationMinClamp = -50;
	float cameraXAngle = 0, cameraYAngle = 0;
	Vector3 cameraDesiredPosition;
	Quaternion cameraRotation;
	RaycastHit cameraRayHit;

	[Header("Launch Settings")]
	public bool canLaunch = true;
	public RectTransform launchChargeDisplay;
	public Vector3 minLaunchVelocity = new Vector3(0, 12, 4), maxLaunchVelocity = new Vector3(0, 50, 10);
	public float launchChargeSpeed = .5f;
	float launchCharge, launchChargeDisplayMaxWidth, launchChargeDisplayHeight;
	bool launchRoutineRunning = false;

	public enum GroundMovementMode { Walking, Leaping };
	[Header("Movement Settings")]
	public GroundMovementMode groundMovementMode = GroundMovementMode.Walking;
	public float walkingMovementSpeed = 8;
	public Vector3 leapingVelocity = new Vector3(0, 10, 16);
	public float airborneMovementSpeed = 16, airborneMovementAcceleration = 18, airborneDecceleration = .85f, inputDeadzone = .5f;

	[Header("Hop Settings")]
	public bool canHop = true;
	public float hopVelocity = 15;
	bool disableGravity = false;

	[Header("Wall Bounce Settings")]
	public bool wallBounceEnabled = true;
	public Vector3 wallBounceVelocityModifier = new Vector3(0, 1, 1);
	RaycastHit wallBounceCastHit;

	[Header("Gravity Settings")]
	public float gravityStrength = 2;
	public float maximumFallingSpeed = 15;

	[Header("Timer Settings")]
	public UnityEngine.UI.Text timerDisplay;
	public bool useTimer;
	public float timeBeforeAbilityDisabled = 300;
	public enum Abilities { Jump, Launch };
	public Abilities abilityTimerEnables = Abilities.Launch;

	[Header("Dash Settings")]
	bool useDash = false;
	public float dashSpeed = 30, dashHeigth = 10, timingInterval = 1.5f;
	Vector3 timingRayOrigin;
	bool disableAirControl;

	[Header("Mechanic Settings (ON = HOP/OFF = DASH)")]
	public bool toggleDashHop;
	float dashHopCoroutineDelay;

	//[Header("SnowTornado Settings")]
	bool isSpinning = false, canBeisSpinning = true;
	Vector3 snowTornadoDesiredPlayerPosition;

	[Header("New Camera Variables")]
	//public bool isLaunching;
	bool canBounce, stopDeactivateBounce;

	[Header("Twirl Settings")]
	public GameObject model;
	public bool enableTwirl = true;
	public float twirlTime = .26f;


	//SETUP
	void Start ()
	{
		rig = GetComponent<Rigidbody>();

		launchChargeDisplayHeight = launchChargeDisplay.sizeDelta.y;
		launchChargeDisplayMaxWidth = launchChargeDisplay.sizeDelta.x;
		launchChargeDisplay.sizeDelta = new Vector2(0, launchChargeDisplayHeight);

		if (useTimer)
			StartCoroutine(TimedAbility(timeBeforeAbilityDisabled, abilityTimerEnables));
		else
			timerDisplay.enabled = false;
	}



	//UPDATES
	void Update ()
	{
		CameraControl();
		Launch();

		Dash();
		Hop();
	}
	private void FixedUpdate ()
	{
		CheckRotation();
		if (!isSpinning) {
			Gravity();

			Movement();
			WalkSlowly();
			if (canBounce) {
				WallBounce();
			}

			////RESOLVE VELOCITY
			rig.velocity = transform.rotation * velocity;
		}
	}



	//UPDATE FUNCTIONS
	void CameraControl ()
	{
		cameraYAngle += Input.GetAxis("Right Stick X") * cameraHorizontalSensitivity * Time.deltaTime;
		cameraXAngle = Mathf.Clamp(cameraXAngle - Input.GetAxis("Right Stick Y") * cameraVerticalSensitivity * Time.deltaTime, cameraXRotationMinClamp, cameraXRotationMaxClamp);

		if (Grounded()) {
			transform.rotation = Quaternion.Euler(new Vector3(0, cameraYAngle, 0));
		}

		cameraTrans.position = transform.position + Quaternion.Euler(new Vector3(cameraXAngle, cameraYAngle)) * cameraOffset;
		cameraTrans.LookAt(transform.position + Quaternion.Euler(new Vector3(cameraXAngle, cameraYAngle)) * cameraTarget);
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
	void CheckRotation ()
	{
		Ray RotationRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.Raycast(RotationRay, 1, 1)) {
			disableRotation = false;
		} else {
			disableRotation = true;
		}
	}

	void WalkSlowly ()
	{
		if (!disableAirControl) {
			if (Input.GetAxis("Left Stick Y") >= -0.5 && Input.GetAxis("Left Stick Y") <= 0.5) {
				if (Input.GetAxis("Left Stick X") >= -0.5 && Input.GetAxis("Left Stick X") <= 0.5) {
					groundMovementMode = GroundMovementMode.Walking;
				} else {
					groundMovementMode = GroundMovementMode.Leaping;
				}
			} else {
				groundMovementMode = GroundMovementMode.Leaping;
			}
		}
	}
	void Movement ()
	{
		switch (groundMovementMode) {
			case GroundMovementMode.Walking:
				if (Grounded()) {
					velocity.x = Input.GetAxis("Left Stick X") * walkingMovementSpeed;
					velocity.z = Input.GetAxis("Left Stick Y") * walkingMovementSpeed;
				} else {
					if (Input.GetAxis("Left Stick X") != 0)
						velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Left Stick X") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
					else
						velocity.x *= airborneDecceleration;
					if (Input.GetAxis("Left Stick Y") != 0)
						velocity.z = Mathf.Clamp(velocity.z + Input.GetAxis("Left Stick Y") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
					else
						velocity.z *= airborneDecceleration;

				}
				break;
			case GroundMovementMode.Leaping:
				if (Grounded()) {
					if (Mathf.Abs(Input.GetAxis("Left Stick X")) >= inputDeadzone || Mathf.Abs(Input.GetAxis("Left Stick Y")) >= inputDeadzone) {
						velocity = new Vector3(Input.GetAxis("Left Stick X"), 0, Input.GetAxis("Left Stick Y")).normalized * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
						StartCoroutine(SuspendGroundedCheck());
					} else {
						velocity.x = velocity.z = 0;
					}
				} else {
					if (!disableAirControl) {
						if (Input.GetAxis("Left Stick X") != 0)
							velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Left Stick X") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
						else
							velocity.x *= airborneDecceleration;
						if (Input.GetAxis("Left Stick Y") != 0)
							velocity.z = Mathf.Clamp(velocity.z + Input.GetAxis("Left Stick Y") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
						else
							velocity.z *= airborneDecceleration;
					}
				}
				break;
		}
	}
	void Dash ()
	{
		Ray TimingRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.Raycast(TimingRay, timingInterval, 1)) {
			if (Input.GetButtonDown("B Button")) {
				disableAirControl = true;
				velocity.z = Mathf.Clamp(Mathf.Abs(Input.GetAxis("Left Stick Y")), 0.5f, 1) * dashSpeed;
				if (Input.GetAxis("Left Stick Y") > 0) {
					velocity.x = Mathf.Clamp(Input.GetAxis("Left Stick X"), -0.5f, 0.5f) * dashSpeed;
				}
				velocity.y = dashHeigth;
				dashHopCoroutineDelay = 1f;
				StartCoroutine(DashCooldown());
			}
		}
	}
	void Hop ()
	{
		if (canHop) {
			if (Input.GetButtonDown("A Button")) {
				canHop = false;
				velocity.y = hopVelocity;
				StartCoroutine(SuspendGroundedCheck());
			}
		} else {
			if (Grounded())
				canHop = true;
		}
	}
	void WallBounce ()
	{
		if (wallBounceEnabled) {
			if (!Grounded()) {
				if (Physics.CapsuleCast(transform.position + new Vector3(0, .2f, 0), transform.position + new Vector3(0, 0, 0), .45f, transform.forward, out wallBounceCastHit, .1f)) {

					if (wallBounceCastHit.transform.tag != "Tornado") {
						transform.forward = Vector3.Reflect(transform.forward, wallBounceCastHit.normal);
						transform.forward -= new Vector3(0, transform.forward.y, 0);
					}
				}
			}
		}
	}
	void Gravity ()
	{
		if (Grounded()) {
			if (!disableGravity) {
				velocity.y = 0;
			}
		} else {
			if (velocity.y > -maximumFallingSpeed)
				velocity.y -= gravityStrength * Time.fixedDeltaTime;
		}
	}



	//RETURN FUNCTIONS
	bool Grounded ()
	{
		if (groundedSuspended) {
			return false;
		}

		Ray GroundedRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.SphereCast(GroundedRay, .42f, .1f)) {
			if (!stopDeactivateBounce) {
				canBounce = false;
			}
			canHop = true;
			return true;
		} else {
			return false;
		}
	}
	string CorrectTimerString (string timeUnit)
	{
		if (timeUnit.Length < 2)
			timeUnit = "0" + timeUnit;
		return timeUnit;
	}



	//COROUTINES
	IEnumerator LaunchRoutine ()
	{
		launchCharge = 0;
		launchChargeDisplay.sizeDelta = new Vector2(0, launchChargeDisplayHeight);

		while (Input.GetAxis("Right Trigger") != 0) {
			launchChargeDisplay.sizeDelta = new Vector2(launchChargeDisplayMaxWidth * launchCharge, launchChargeDisplayHeight);
			launchCharge = Mathf.Clamp(launchCharge + launchChargeSpeed * Time.deltaTime, 0, 1);
			yield return null;
		}

		while (!Grounded()) {
			yield return null;
		}

		launchChargeDisplay.sizeDelta = new Vector2(0, launchChargeDisplayHeight);
		velocity = minLaunchVelocity + (maxLaunchVelocity - minLaunchVelocity) * launchCharge;
		StopCoroutine(SuspendGroundedCheck());
		StartCoroutine(SuspendGroundedCheck());
		launchRoutineRunning = false;

		StopCoroutine(Twirl());
		StartCoroutine(Twirl());

		stopDeactivateBounce = true;
		StartCoroutine(DeactivateBounce());
	}
	IEnumerator SuspendGroundedCheck (float suspensionTime = .1f)
	{
		groundedSuspended = true;
		yield return new WaitForSeconds(suspensionTime);
		groundedSuspended = false;
	}
	IEnumerator TimedAbility (float time, Abilities ability)
	{
		switch (ability) {
			case Abilities.Jump:
				canHop = true;
				break;
			case Abilities.Launch:
				canLaunch = true;
				break;
		}

		string hours, minutes, seconds;
		for (float t = time; t > 0; t -= Time.deltaTime) {
			hours = CorrectTimerString((Mathf.FloorToInt(t / 3600) % 99).ToString());
			minutes = CorrectTimerString((Mathf.FloorToInt(t / 60) % 60).ToString());
			seconds = CorrectTimerString((Mathf.FloorToInt(t) % 60).ToString());

			timerDisplay.text = hours.ToString() + ":" + minutes.ToString() + ":" + seconds.ToString();
			yield return null;
		}

		switch (ability) {
			case Abilities.Jump:
				canHop = false;
				break;
			case Abilities.Launch:
				canLaunch = false;
				break;
		}
	}
	IEnumerator Twirl ()
	{
		if (model == null)
			yield break;
		while (!Grounded()) {
			model.transform.Rotate(new Vector3(0, 0, 360 / twirlTime) * Time.deltaTime);
			yield return null;
		}
		model.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}



	//SNOW MECHANICS FUNCTIONS
	IEnumerator ISnowTornado.HitBySnowTornado (Transform tornadoTrans, Vector3 playerOffsetFromCenter, float spinSpeed, float playerLerpFactor, Vector3 releaseVelocity)
	{
		if (isSpinning && !canBeisSpinning)
			yield break;

		isSpinning = true;
		canBeisSpinning = false;

		tornadoTrans.forward = -transform.right;

		while (true) {
			snowTornadoDesiredPlayerPosition = tornadoTrans.forward + playerOffsetFromCenter;
			transform.position = Vector3.Lerp(transform.position, tornadoTrans.position + tornadoTrans.rotation * playerOffsetFromCenter, playerLerpFactor);
			transform.rotation *= Quaternion.Euler(new Vector3(0, spinSpeed * Time.deltaTime, 0));

			if (Input.GetButtonDown("A Button")) {
				velocity = releaseVelocity;
				isSpinning = false;
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		canBeisSpinning = true;
	}

	IEnumerator DeactivateBounce ()
	{
		canBounce = true;
		yield return new WaitForSeconds(0.5F);
		stopDeactivateBounce = false;
	}

	IEnumerator DashCooldown ()
	{
		yield return new WaitForSeconds(dashHopCoroutineDelay);
		disableAirControl = false;
		disableGravity = false;
	}
}
