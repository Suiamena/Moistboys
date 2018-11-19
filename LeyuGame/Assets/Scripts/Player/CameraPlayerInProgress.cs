using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerInProgress : MonoBehaviour, ISnowTornado
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;
	Vector2 leftStickInput = new Vector2(0, 0);

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
	public Vector3 minLaunchVelocity = new Vector3(0, 12, 4), maxLaunchVelocity = new Vector3(0, 72, 10);
	public float launchChargeSpeed = 1.5f;
	float launchCharge, launchChargeDisplayMaxWidth, launchChargeDisplayHeight;
	bool launchRoutineRunning = false;

	[Header("Movement Settings")]
	public float walkingMovementSpeed = 8;
	public Vector3 leapingVelocity = new Vector3(0, 11, 20);
	public float airborneMovementSpeed = 22, airborneMovementAcceleration = 26, airborneDecceleration = .92f;
	[Range(0.0f, 1.0f)]
	public float walkingBouncingThreshold = .85f;

	[Header("Hop Settings")]
	public bool canHop = true;
	public float hopVelocity = 9;
	bool disableGravity = false;

	[Header("Wall Bounce Settings")]
	public bool wallBounceEnabled = false;
	public Vector3 wallBounceVelocityModifier = new Vector3(0, 1, 1);
	RaycastHit wallBounceCastHit;
	bool canWallBounce;

	[Header("Gravity Settings")]
	public float gravityStrength = 38;
	public float maximumFallingSpeed = 96;

	//[Header("SnowTornado Settings")]
	bool inTornado = false, canBeisSpinning = true;
	Vector3 snowTornadoDesiredPlayerPosition;

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
	}



	//UPDATES
	void Update ()
	{
		ProcessInputs();

		CameraControl();
		Launch();

		Hop();
	}
	private void FixedUpdate ()
	{
		if (!inTornado) {
			Gravity();

			Movement();
			if (canWallBounce) {
				WallBounce();
			}

			//RESOLVE VELOCITY
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
	void Movement ()
	{
		if (Grounded()) {
			if (leftStickInput.magnitude == 0) {
				velocity.x = velocity.z = 0;
			} else if (leftStickInput.magnitude < walkingBouncingThreshold) {
				float stuff = leftStickInput.x + leftStickInput.y;
				Vector2 actualInput = leftStickInput.normalized * leftStickInput.magnitude / Mathf.Abs(stuff);
				velocity.x = actualInput.x;
				velocity.z = actualInput.y;
			} else {
				velocity = new Vector3(leftStickInput.x, 0, leftStickInput.y).normalized * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
				StartCoroutine(SuspendGroundedCheck());
			}
		}

		//switch (groundMovementMode) {
		//	case GroundMovementMode.Walking:
		//		if (Grounded()) {
		//			velocity.x = Input.GetAxis("Left Stick X") * walkingMovementSpeed;
		//			velocity.z = Input.GetAxis("Left Stick Y") * walkingMovementSpeed;
		//		} else {
		//			if (Input.GetAxis("Left Stick X") != 0)
		//				velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Left Stick X") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
		//			else
		//				velocity.x *= airborneDecceleration;
		//			if (Input.GetAxis("Left Stick Y") != 0)
		//				velocity.z = Mathf.Clamp(velocity.z + Input.GetAxis("Left Stick Y") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
		//			else
		//				velocity.z *= airborneDecceleration;

		//		}
		//		break;
		//	case GroundMovementMode.Leaping:
		//		if (Grounded()) {
		//			if (Mathf.Abs(Input.GetAxis("Left Stick X")) >= walkingBouncingThreshold || Mathf.Abs(Input.GetAxis("Left Stick Y")) >= walkingBouncingThreshold) {
		//				velocity = new Vector3(Input.GetAxis("Left Stick X"), 0, Input.GetAxis("Left Stick Y")).normalized * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
		//				StartCoroutine(SuspendGroundedCheck());
		//			} else {
		//				velocity.x = velocity.z = 0;
		//			}
		//		} else {
		//			if (Input.GetAxis("Left Stick X") != 0)
		//				velocity.x = Mathf.Clamp(velocity.x + Input.GetAxis("Left Stick X") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
		//			else
		//				velocity.x *= airborneDecceleration;
		//			if (Input.GetAxis("Left Stick Y") != 0)
		//				velocity.z = Mathf.Clamp(velocity.z + Input.GetAxis("Left Stick Y") * airborneMovementAcceleration * Time.fixedDeltaTime, -airborneMovementSpeed, airborneMovementSpeed);
		//			else
		//				velocity.z *= airborneDecceleration;

		//		}
		//		break;
		//}
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
	//DOOD
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
		if (!Grounded())
			if (velocity.y > -maximumFallingSpeed)
				velocity.y -= gravityStrength * Time.fixedDeltaTime;
	}



	//RETURN FUNCTIONS
	bool Grounded ()
	{
		if (groundedSuspended) {
			return false;
		}

		Ray groundedRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.SphereCast(groundedRay, .42f, .1f)) {
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
	}
	IEnumerator SuspendGroundedCheck (float suspensionTime = .1f)
	{
		groundedSuspended = true;
		yield return new WaitForSeconds(suspensionTime);
		groundedSuspended = false;
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