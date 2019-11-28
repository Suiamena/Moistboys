using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaselinePlayer : MonoBehaviour
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;

	//INPUTS
	Vector2 leftStickInput = Vector2.zero, rightStickInput = Vector2.zero;
	float rightTriggerInput = 0;
	bool aButtonInput = false;

	[Header("Camera Settings")]
	public Transform cameraTrans;
	public Vector3 cameraOffset = new Vector3(0, 3, -14), cameraTarget = new Vector3(0, 0, 3);
	public float cameraHorizontalSensitivity = 130, cameraVaselineHorizontalSensitivity = 65, cameraVertialSensitivity = 90f, maxCameraXAngle = 50, minCameraXAngle = -50;
	float cameraXAngle = 0;
	Vector3 cameraDesiredPosition;
	Quaternion cameraRotation;
	RaycastHit cameraRayHit;

	[Header("Movement Settings")]
	public float groundedMaxSpeed = 8;
	public float groundedMovementAcceleration = 4, groundedMovementDeceleration = 6, airborneMovementSpeed = 8, airborneMovementAcceleration = 3, airborneDecceleration = 3, inputDeadzone = .5f;
	Vector2 lateralVelocity;

	[Header("Jump Settings")]
	public bool canJump = true;
	public float jumpVelocity = 12;

	[Header("Vaseline Settings")]
	public bool canUseVaseline = true;
	public float vaselineAcceleration = 4, vaselineMaxSpeed = 22;
	bool vaselineActive = false;

	[Header("Wall Slide Settings")]
	public Vector3 wallJumpForce = new Vector3(0, 5, 7);
	RaycastHit wallSlideCastHit;
	Ray wallSlideRoutineRay;
	bool wallSliding = false;

	[Header("Gravity Settings")]
	public float gravityStrength = 8;
	public float maximumFallingSpeed = 30;

	[Header("Timer Settings")]
	public UnityEngine.UI.Text timerDisplay;
	public float timeBeforeAbilityDisabled = 300;
	public enum Abilities { Jump, Vaseline };
	public Abilities abilityTimerEnables = Abilities.Vaseline;



	//SETUP
	void Start ()
	{
		rig = GetComponent<Rigidbody>();

		StartCoroutine(TimedAbility(timeBeforeAbilityDisabled, abilityTimerEnables));
	}



	//UPDATES
	void Update ()
	{
		GetInputs();

		CameraControl();
		Vaseline();
	}
	private void FixedUpdate ()
	{
		lateralVelocity = new Vector2(velocity.x, velocity.z);

		if (!wallSliding) {
			Gravity();

			if (vaselineActive)
				VaselineMovement();
			else
				Movement();
			Jump();
			WallSlide();
		}

		//RESOLVE VELOCITY
		rig.velocity = transform.rotation * velocity;
	}



	//UPDATE FUNCTIONS
	void GetInputs ()
	{
		leftStickInput = new Vector2(Input.GetAxis("Left Stick X"), Input.GetAxis("Left Stick Y"));
		rightStickInput = new Vector2(Input.GetAxis("Right Stick X"), Input.GetAxis("Right Stick Y"));
		rightTriggerInput = Input.GetAxis("Right Trigger");
		aButtonInput = Input.GetButton("A Button");
	}
	void CameraControl ()
	{
		if (!wallSliding) {
			cameraXAngle = Mathf.Clamp(cameraXAngle + rightStickInput.y * cameraVertialSensitivity * Time.deltaTime, minCameraXAngle, maxCameraXAngle);
			if (new Vector3(velocity.x, 0, velocity.z).magnitude > groundedMaxSpeed) {
				float currentHorizontalSensitivity;
				currentHorizontalSensitivity = (vaselineMaxSpeed - lateralVelocity.magnitude) / (vaselineMaxSpeed - groundedMaxSpeed) * (cameraHorizontalSensitivity - cameraVaselineHorizontalSensitivity) + cameraVaselineHorizontalSensitivity;
				transform.Rotate(0, rightStickInput.x * currentHorizontalSensitivity * Time.deltaTime, 0);
			} else
				transform.Rotate(0, rightStickInput.x * cameraHorizontalSensitivity * Time.deltaTime, 0);
			cameraRotation = Quaternion.Euler(cameraXAngle, transform.eulerAngles.y, 0);
		}

		if (Grounded())
			cameraDesiredPosition = Vector3.Lerp(cameraTrans.position, transform.position + cameraRotation * cameraOffset, 0.3f);
		else
			cameraDesiredPosition = Vector3.Lerp(cameraTrans.position, transform.position + cameraRotation * cameraOffset, 0.15f);

		if (Physics.Raycast(transform.position, cameraDesiredPosition - transform.position, out cameraRayHit, Vector3.Distance(transform.position, cameraDesiredPosition))) {
			cameraTrans.position = Vector3.Lerp(cameraTrans.position, cameraRayHit.point, .45f);
		} else
			cameraTrans.position = cameraDesiredPosition;

		cameraTrans.LookAt(transform.position + cameraRotation * cameraTarget);
	}
	void Vaseline ()
	{
		if (Grounded() && rightTriggerInput != 0)
			vaselineActive = true;
		else
			vaselineActive = false;
	}



	//FIXED UPDATE FUNCTIONS
	void VaselineMovement ()
	{
		if (leftStickInput.magnitude != 0) {
			lateralVelocity += new Vector2(leftStickInput.x, leftStickInput.y).normalized * vaselineAcceleration * Time.fixedDeltaTime;
			lateralVelocity = Vector2.ClampMagnitude(lateralVelocity, vaselineMaxSpeed);
		}
		velocity = new Vector3(lateralVelocity.x, velocity.y, lateralVelocity.y);
	}
	void Movement ()
	{
		if (Grounded()) {
			if (lateralVelocity.magnitude <= groundedMaxSpeed) {
				lateralVelocity = new Vector2(leftStickInput.x * groundedMaxSpeed, leftStickInput.y * groundedMaxSpeed);
				lateralVelocity = Vector2.ClampMagnitude(lateralVelocity, groundedMaxSpeed);
			} else {
				lateralVelocity += -lateralVelocity.normalized * groundedMovementAcceleration * Time.fixedDeltaTime + leftStickInput.normalized * groundedMovementAcceleration * Time.fixedDeltaTime;
			}
		} else {
			if (leftStickInput.magnitude == 0) {
				lateralVelocity += -lateralVelocity.normalized * airborneDecceleration * Time.fixedDeltaTime;
			} else {
				if (lateralVelocity.magnitude <= airborneMovementSpeed) {
					lateralVelocity += leftStickInput.normalized * airborneMovementAcceleration * Time.fixedDeltaTime;
					lateralVelocity = Vector2.ClampMagnitude(lateralVelocity, airborneMovementSpeed);
				} else {
					lateralVelocity += -lateralVelocity.normalized * airborneMovementAcceleration * Time.fixedDeltaTime + leftStickInput.normalized * airborneMovementAcceleration * Time.fixedDeltaTime;
				}
			}
		}
		velocity = new Vector3(lateralVelocity.x, velocity.y, lateralVelocity.y);
	}
	void Jump ()
	{
		if (Grounded() && canJump)
			if (aButtonInput) {
				velocity.y += jumpVelocity;
				StartCoroutine(SuspendGroundedCheck());
			}
	}
	void WallSlide ()
	{
		if (!Grounded()) {
			if (Physics.CapsuleCast(transform.position + new Vector3(0, .2f, 0), transform.position + new Vector3(0, 0, 0), .48f, transform.forward, out wallSlideCastHit, .1f)) {
				Vector3 oldVelocity = transform.rotation * new Vector3(velocity.x, 0, velocity.z), normal = new Vector3(wallSlideCastHit.normal.x, 0, wallSlideCastHit.normal.z);
				float angle = Vector3.Angle(normal, oldVelocity);
				float newSpeed = Mathf.Sin(Mathf.Deg2Rad * angle) * oldVelocity.magnitude;
				velocity = new Vector3(0, velocity.y, newSpeed);

				float distanceOne = Vector3.Distance(transform.position + oldVelocity.normalized * 5, transform.position + transform.forward + Quaternion.Euler(0, 90, 0) * normal * 5);
				if (Vector3.Distance(transform.position + oldVelocity, wallSlideCastHit.point + Quaternion.Euler(0, 270, 0) * normal * 5) < distanceOne)
					transform.forward = Quaternion.Euler(0, 270, 0) * normal;
				else
					transform.forward = Quaternion.Euler(0, 90, 0) * normal;

				if (!wallSliding) {
					wallSliding = true;
					StartCoroutine(WallSlideRoutine(normal));
				}
			}
		}
	}
	void Gravity ()
	{
		if (Grounded()) {
			velocity.y = 0;
		} else {
			if (velocity.y > -maximumFallingSpeed)
				velocity.y -= gravityStrength * Time.fixedDeltaTime;
		}
	}



	//RETURN FUNCTIONS
	bool Grounded ()
	{
		if (groundedSuspended)
			return false;

		Ray GroundedRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.SphereCast(GroundedRay, .42f, .1f))
			return true;
		else
			return false;
	}
	string CorrectTimerString (string timeUnit)
	{
		if (timeUnit.Length < 2)
			timeUnit = "0" + timeUnit;
		return timeUnit;
	}



	//COROUTINES
	IEnumerator WallSlideRoutine (Vector3 wallNormal)
	{
		while (true) {
			velocity.y -= gravityStrength * .5f * Time.deltaTime;

			if (aButtonInput) {
				transform.forward = (transform.rotation * velocity).normalized + wallNormal;
				transform.forward -= new Vector3(0, transform.forward.y, 0);
				velocity = wallJumpForce;
				break;
			}

			wallSlideRoutineRay = new Ray(transform.position, -wallNormal);
			if (Grounded() || !Physics.SphereCast(wallSlideRoutineRay, .5f, .8f)) {
				Debug.Log("Wall Lost");
				break;
			}
			yield return null;
		}
		wallSliding = false;
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
				canJump = true;
				break;
			case Abilities.Vaseline:
				canUseVaseline = true;
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
				canJump = false;
				break;
			case Abilities.Vaseline:
				canUseVaseline = false;
				break;
		}
	}
}
