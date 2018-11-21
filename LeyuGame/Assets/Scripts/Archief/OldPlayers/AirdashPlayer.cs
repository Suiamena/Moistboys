using System.Collections;
using UnityEngine;

public class AirdashPlayer : MonoBehaviour, IPulsingSnow, ISnowTornado, ISnowball
{
	Rigidbody rig;
	Vector3 velocity;
	bool groundedSuspended = false;

	[Header("Camera Settings")]
	public new Transform camera;
	public float horizontalCameraSensitivity = 1.5f, verticalCameraSensitivity = 1.2f, cameraXRotationMaxClamp = 60, cameraXRotationMinClamp = -60;
	public Vector3 cameraOffset = new Vector3(0, 3, -6), cameraTarget = new Vector3(0, 0, 4);
	float cameraYAngle = 0, cameraXAngle = 0;

	[Header("Landing Indicator Settings")]
	public Transform landingIndicatorTrans;
	public Sprite landingIndicatorSprite;
	public float landingIndicatorMaxDistance = 100;
	public bool useLandingIndicator = true, useLandingIndicatorOnlyWhenAirborne = false;
	Vector3 landingIndicatorPosition;
	float landingIndicatorYRotation;
	Ray landingIndicatorRay;
	RaycastHit landingIndicatorRayHit;

	[Header("Movement Settings")]
	public float walkingSpeed = 3;
	public float runningSpeed = 9, timeBetweenWalkingAndRunning = 0.8f, backwardsSpeed = 4, airborneSpeed = 9, airborneAcceleration = 2;
	bool running = false, runningRoutineRunning = false;

	[Header("Jumping Settings")]
	public float jumpVelocity = 15;
	public float gravityStrength = 1.5f;

	[Header("Dash Settings")]
	public GameObject dashTrailPrefab;
	public float dashVelocity = 24, dashTime = 1, dashTrailLifetime = 2.5f, consecutiveDashTimeframe = 1, dashTriggerDeadzone = 0.8f;
	public int maximumConsecutiveDashes = 5;
	bool dashing = false, canDash = true, dashTriggerReleased = false;
	Vector3 dashTrajectory;
	int consecutiveDashesPerformed = 0;
	enum DashTriggers { LeftTrigger = -1, RightTrigger = 1 };

	[Header("Heat Settings")]
	public UnityEngine.UI.Text heatDisplay;
	public bool useOverheating = true;
	public float cooldownAfterOverheat = 3, maxHeat = 100, runningHeatPerSecond = 3, jumpingHeat = 5, dashHeat = 5, heatLossPerSnow = 20;
	float heat = 0;
	bool overheated = false;

	//[Header("SnowPulse Settings")]
	bool affectedBySnowPulse = false;
	Vector3 snowPulseOrigin;
	float snowPulseVelocity;

	//[Header("SnowTornado Settings")]
	bool affectedBySnowTornado = false, canBeAffectedBySnowTornado = true;
	Vector3 snowTornadoDesiredPlayerPosition;

	//[Header("Snowball Settings")]
	bool affectedBySnowball = false, snowballRoutineRunning = false;
	Vector3 snowballVelocity;



	//SETUP
	void Start ()
	{
		rig = GetComponent<Rigidbody>();

		landingIndicatorTrans.GetChild(0).GetComponent<SpriteRenderer>().sprite = landingIndicatorSprite;
	}



	//UPDATES
	void Update ()
	{
		CameraMovement();

		if (!affectedBySnowTornado && !affectedBySnowball) {
			if (useLandingIndicator)
				LandingIndicator();
			Heat();
		}
	}
	private void FixedUpdate ()
	{
		if (!affectedBySnowTornado && !affectedBySnowball) {
			if (!overheated) {
				Dash();

				if (!dashing) {
					Move();
					Jump();
				}
			}

			rig.velocity = transform.rotation * velocity;
		}
	}



	//UPDATE
	void CameraMovement ()
	{
		if (!overheated) {
			cameraYAngle += Input.GetAxis("Right Stick X") * horizontalCameraSensitivity;
			cameraXAngle = Mathf.Clamp(cameraXAngle - Input.GetAxis("Right Stick Y") * verticalCameraSensitivity, cameraXRotationMinClamp, cameraXRotationMaxClamp);
		}
        if (!dashing)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, cameraYAngle, 0));
        }

		camera.transform.position = transform.position + Quaternion.Euler(new Vector3(cameraXAngle, cameraYAngle)) * cameraOffset;
		camera.transform.LookAt(transform.position + Quaternion.Euler(new Vector3(cameraXAngle, cameraYAngle)) * cameraTarget);
	}
	void LandingIndicator ()
	{
		landingIndicatorPosition = transform.position;

		landingIndicatorRay = new Ray(transform.position, Vector3.up * -1);
		if (Physics.Raycast(landingIndicatorRay, out landingIndicatorRayHit, landingIndicatorMaxDistance)) {
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
	void Heat ()
	{
		if (running && !overheated)
			heat += runningHeatPerSecond * Time.deltaTime;

		if (heat >= maxHeat && !overheated && useOverheating) {
			overheated = true;
			StartCoroutine(Overheat());
		}

		heatDisplay.text = "Heat: " + Mathf.RoundToInt(heat).ToString();
	}



	//FIXED UPDATE
	void Dash ()
	{
		if (Input.GetAxis("Right Trigger") > dashTriggerDeadzone || Input.GetAxis("Left Trigger") > dashTriggerDeadzone) {
			if (!dashing && canDash && dashTriggerReleased) {
				dashing = true;
				canDash = false;
				if (Input.GetAxis("Right Trigger") != 0)
					StartCoroutine(DashRoutine(DashTriggers.RightTrigger));
				else
					StartCoroutine(DashRoutine(DashTriggers.LeftTrigger));
			}
		} else {
			dashTriggerReleased = true;
		}
	}
	void Move ()
	{
		if (Grounded()) {
			velocity.x = Input.GetAxisRaw("Left Stick X") * walkingSpeed;

			if (Input.GetAxis("Left Stick Y") > 0) {
				if (!runningRoutineRunning && !running) {
					runningRoutineRunning = true;
					StartCoroutine(WalkingToRunning());
				}
				if (running)
					velocity.z = runningSpeed;
				else
					velocity.z = walkingSpeed;
			} else if (Input.GetAxis("Left Stick Y") < 0) {
				velocity.z = backwardsSpeed * -1;
				running = false;
			} else {
				velocity.z = 0;
				running = false;
			}
		} else {
			velocity += new Vector3(Input.GetAxis("Left Stick X") * airborneAcceleration, 0, Input.GetAxis("Left Stick Y") * airborneAcceleration) * Time.fixedDeltaTime;

			Vector2 lateralVelocityCheck = new Vector2(velocity.x, velocity.z);
			if (lateralVelocityCheck.magnitude > airborneSpeed) {
				lateralVelocityCheck = lateralVelocityCheck.normalized * airborneSpeed;
				velocity.x = lateralVelocityCheck.x;
				velocity.z = lateralVelocityCheck.y;
			}
		}

		//SNOWPULSE IMPLEMENTATION
		if (affectedBySnowPulse) {
			Vector3 snowPulseDirection = Quaternion.Inverse(transform.rotation) * (transform.position - snowPulseOrigin);
			velocity += new Vector3(snowPulseDirection.x, 0, snowPulseDirection.z).normalized * snowPulseVelocity;

			affectedBySnowPulse = false;
		}
		//SNOWBALL IMPLEMENTATION
		//if (affectedBySnowball) {
		//	velocity += Quaternion.Inverse(transform.rotation) * snowballVelocity;
		//	affectedBySnowball = false;
		//}
	}
	void Jump ()
	{
		if (Grounded()) {
			if (Input.GetButtonDown("A Button")) {
				velocity.y += jumpVelocity;
				SuspendGroundedCheck();
				if (running)
					heat += jumpingHeat;
			} else {
				velocity.y = 0;
			}
		} else {
			velocity.y -= gravityStrength;
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



	//COROUTINES
	IEnumerator Overheat ()
	{
		StopCoroutine(WalkingToRunning());
		runningRoutineRunning = false;
		StopCoroutine(DashRoutine(DashTriggers.LeftTrigger));
		running = dashing = false;
		velocity = Vector3.zero;

		for (float t = 0; t < cooldownAfterOverheat; t += Time.deltaTime) {
			velocity.x = 0;
			velocity.z = 0;
			if (!Grounded())
				velocity.y -= gravityStrength;
			else
				velocity.y = 0;

			heat -= 100 / cooldownAfterOverheat * Time.deltaTime;
			yield return null;
		}

		heat = 0;
		overheated = false;
		canDash = true;
	}
	IEnumerator DashRoutine (DashTriggers dashTrigger = DashTriggers.RightTrigger)
	{
		heat += dashHeat;
		canDash = dashTriggerReleased = false;
		++consecutiveDashesPerformed;
		Quaternion dashTrajectoryRotation = Quaternion.Euler(Input.GetAxis("Left Stick Y") * 90, 0, Input.GetAxis("Left Stick X") * -90);
		dashTrajectory = dashTrajectoryRotation * Vector3.up * dashVelocity;
		dashTrajectory.y *= (int) dashTrigger;
		GameObject trailGO = Instantiate(dashTrailPrefab, transform.position, Quaternion.identity);
		trailGO.GetComponent<TrailRenderer>().time = dashTrailLifetime;

		for (float t = 0; t < dashTime; t += Time.deltaTime) {
			if (Grounded() && t > 0.1f)
				dashTrajectory.y = 0;
			trailGO.transform.position = transform.position;
			velocity = dashTrajectory;

			//SNOWPULSE IMPLEMENTATION
			if (affectedBySnowPulse) {
				Vector3 snowPulseDirection = Quaternion.Inverse(transform.rotation) * (transform.position - snowPulseOrigin);
				velocity += new Vector3(snowPulseDirection.x, 0, snowPulseDirection.z).normalized * snowPulseVelocity;

				affectedBySnowPulse = false;
			}
			yield return null;
		}
		dashing = false;
		Destroy(trailGO, dashTrailLifetime);

		for (float t = 0; t < consecutiveDashTimeframe; t += Time.deltaTime) {
			if (Grounded())
				break;

			if (Input.GetAxis("Right Trigger") > dashTriggerDeadzone || Input.GetAxis("Left Trigger") > dashTriggerDeadzone) {
				if (dashTriggerReleased && consecutiveDashesPerformed < maximumConsecutiveDashes) {
					dashing = true;
					if (Input.GetAxis("Right Trigger") != 0)
						StartCoroutine(DashRoutine(DashTriggers.RightTrigger));
					else
						StartCoroutine(DashRoutine(DashTriggers.LeftTrigger));
					yield break;
				}
			}
			yield return null;
		}

		while (true) {
			if (dashTriggerReleased && Grounded()) {
				canDash = true;
				consecutiveDashesPerformed = 0;
				break;
			}
			yield return null;
		}
		Debug.Log("Done Dashing");
	}
	IEnumerator WalkingToRunning ()
	{
		yield return new WaitForSeconds(timeBetweenWalkingAndRunning);
		if (!overheated)
			running = true;
		runningRoutineRunning = false;
	}
	IEnumerator SuspendGroundedCheck (float suspensionTime = .1f)
	{
		groundedSuspended = true;
		yield return new WaitForSeconds(suspensionTime);
		groundedSuspended = false;
	}



	//SNOW MECHANICS FUNCTIONS
	void IPulsingSnow.HitByPulsingSnow (Vector3 origin, float pushVelocity)
	{
		snowPulseOrigin = origin;
		snowPulseVelocity = pushVelocity;
		affectedBySnowPulse = true;
	}
	IEnumerator ISnowTornado.HitBySnowTornado (Transform tornadoTrans, Vector3 playerOffsetFromCenter, float spinSpeed, float playerLerpFactor, Vector3 releaseVelocity)
	{
		if (affectedBySnowTornado && !canBeAffectedBySnowTornado)
			yield break;

		affectedBySnowTornado = true;
		StopCoroutine(DashRoutine());
		StopCoroutine(WalkingToRunning());
		dashing = running = runningRoutineRunning = affectedBySnowPulse = canBeAffectedBySnowTornado = false;

		tornadoTrans.forward = -transform.right;

		while (true) {
			snowTornadoDesiredPlayerPosition = tornadoTrans.forward + playerOffsetFromCenter;
			transform.position = Vector3.Lerp(transform.position, tornadoTrans.position + tornadoTrans.rotation * playerOffsetFromCenter, playerLerpFactor);
			cameraYAngle += spinSpeed * Time.deltaTime;

			if (Input.GetButtonDown("A Button")) {
				velocity = releaseVelocity;
				transform.forward = Quaternion.Euler(new Vector3(0, cameraYAngle, 0)) * Vector3.forward;
				affectedBySnowTornado = false;
				break;
			}
			yield return null;
		}
		yield return new WaitForSeconds(2f);
		canBeAffectedBySnowTornado = true;
	}
	void ISnowball.HitBySnowball (float pushForce, float pushTime, Vector3 snowballOrigin)
	{
		affectedBySnowball = true;
		StopCoroutine(DashRoutine());
		dashing = false;
		snowballVelocity = (transform.position - snowballOrigin).normalized * pushForce;

		if (!snowballRoutineRunning) {
			snowballRoutineRunning = true;
			StartCoroutine(ResolveSnowballHit(pushTime));
		}
	}
	IEnumerator ResolveSnowballHit (float pushTime)
	{
		for (float t = 0; t < pushTime; t += Time.deltaTime) {
			transform.position += snowballVelocity * Time.deltaTime;
			yield return null;
		}
		affectedBySnowball = snowballRoutineRunning = false;
	}
}