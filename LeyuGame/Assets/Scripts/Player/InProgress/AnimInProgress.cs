using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimInProgress : MonoBehaviour, ISnowTornado
{
    Rigidbody rig;
    Vector3 velocity;
    bool groundedSuspended = false;
    Vector2 leftStickInput = new Vector2(0, 0);

    //Animation Settings
    GameObject animationModel;
    Animator animator;
    bool isBouncing, isLaunching;

    [Header("Camera Settings")]
    public Transform cameraTrans;
    public Vector3 cameraOffset = new Vector3(0, 3, -14), cameraTarget = new Vector3(0, 0, 3);
    [Range(0.0f, 1.0f)]
    public float cameraPositionSmoothing = .2f;
    public float cameraHorizontalSensitivity = 130, cameraVerticalSensitivity = 90f, cameraXRotationMaxClamp = 50, cameraXRotationMinClamp = -50;
    float cameraXAngle = 0, cameraYAngle = 0;
    Vector3 cameraDesiredPosition;
    Quaternion cameraRotation;
    RaycastHit cameraRayHit;

    [Header("Launch Settings")]
    public bool canLaunch = true;
    public RectTransform launchChargeDisplay;
    public Vector3 minLaunchVelocity = new Vector3(0, 12, 4), maxLaunchVelocity = new Vector3(0, 72, 10);
    public float launchChargeSpeed = 1f;
    float launchCharge, launchChargeDisplayMaxWidth, launchChargeDisplayHeight;
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

    [Header("Twirl Settings")]
    public GameObject model;
    public bool enableTwirl = true;
    public float twirlTime = .26f;

    [Header("Landing Indicator Settings")]
    public Transform landingIndicatorTrans;
    public float landingIndicatorMaxDistance = 100;
    public bool useLandingIndicator = true, useLandingIndicatorOnlyWhenAirborne = false;
    Vector3 landingIndicatorPosition;
    float landingIndicatorYRotation;
    Ray landingIndicatorRay;
    RaycastHit landingIndicatorRayHit;


    //SETUP
    void Start()
    {
        rig = GetComponent<Rigidbody>();

        launchChargeDisplayHeight = launchChargeDisplay.sizeDelta.y;
        launchChargeDisplayMaxWidth = launchChargeDisplay.sizeDelta.x;
        launchChargeDisplay.sizeDelta = new Vector2(0, launchChargeDisplayHeight);

        animationModel = GameObject.Find("MOD_Draak");
        animator = animationModel.GetComponent<Animator>();
    }



    //UPDATES
    void Update()
    {
        ProcessInputs();

        CameraControl();
        if (useLandingIndicator)
            LandingIndicator();
        Launch();
        Hop();
    }
    private void FixedUpdate()
    {
        if (!inTornado)
        {
            Gravity();

            RunAnimation();
            Movement();

            //RESOLVE VELOCITY
            rig.velocity = transform.rotation * velocity;
        }
    }



    //UPDATE FUNCTIONS
    void ProcessInputs()
    {
        leftStickInput = new Vector2(Input.GetAxis("Left Stick X"), Input.GetAxis("Left Stick Y"));
    }
    void CameraControl()
    {
        cameraYAngle += Input.GetAxis("Right Stick X") * cameraHorizontalSensitivity * Time.deltaTime;
        cameraXAngle = Mathf.Clamp(cameraXAngle - Input.GetAxis("Right Stick Y") * cameraVerticalSensitivity * Time.deltaTime, cameraXRotationMinClamp, cameraXRotationMaxClamp);
        cameraRotation = Quaternion.Euler(cameraXAngle, cameraYAngle, 0);

        if (Grounded())
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, cameraYAngle, 0));
        }
        cameraDesiredPosition = Vector3.Lerp(cameraTrans.position, transform.position + cameraRotation * cameraOffset, cameraPositionSmoothing);

        if (Physics.Raycast(transform.position, cameraDesiredPosition - transform.position, out cameraRayHit, Vector3.Distance(transform.position, cameraDesiredPosition)))
        {
            cameraTrans.position = Vector3.Lerp(cameraTrans.position, cameraRayHit.point, .45f);
        }
        else
        {
            cameraTrans.position = cameraDesiredPosition;
        }

        cameraTrans.LookAt(transform.position + cameraRotation * cameraTarget);
    }
    void LandingIndicator()
    {
        landingIndicatorPosition = transform.position;

        landingIndicatorRay = new Ray(transform.position, Vector3.up * -1);
        if (Physics.Raycast(landingIndicatorRay, out landingIndicatorRayHit, landingIndicatorMaxDistance))
        {
            landingIndicatorPosition.y = landingIndicatorRayHit.point.y;
        }

        landingIndicatorYRotation = transform.eulerAngles.y;
        landingIndicatorTrans.eulerAngles = new Vector3(0, landingIndicatorYRotation, 0);

        if (useLandingIndicatorOnlyWhenAirborne && Grounded())
        {
            landingIndicatorTrans.gameObject.SetActive(false);
        }
        else
        {
            landingIndicatorTrans.gameObject.SetActive(true);
        }

        landingIndicatorTrans.position = landingIndicatorPosition;
    }
    void Launch()
    {
        if (canLaunch && Input.GetAxis("Right Trigger") != 0)
        {
            if (!launchRoutineRunning)
            {
                launchRoutineRunning = true;
                StartCoroutine(LaunchRoutine());
            }
        }
    }



    //FIXED UPDATE FUNCTIONS
    void Movement()
    {
        if (Grounded())
        {
            if (leftStickInput.magnitude == 0)
            {
                velocity.x = velocity.z = 0;
            }
            else if (leftStickInput.magnitude < walkingBouncingThreshold)
            {
                velocity.x = leftStickInput.x * walkingSpeed;
                velocity.z = leftStickInput.y * walkingSpeed;
            }
            else
            {
                velocity = new Vector3(leftStickInput.x, 0, leftStickInput.y).normalized * leapingVelocity.z + new Vector3(0, leapingVelocity.y, 0);
                StartCoroutine(SuspendGroundedCheck());
            }
        }
        else
        {
            Vector2 lateralSpeed = new Vector2(velocity.x, velocity.z);

            if (leftStickInput.magnitude == 0)
            {
                if (lateralSpeed.magnitude > 0.1f)
                {
                    lateralSpeed += lateralSpeed.normalized * airborneDecceleration * Time.fixedDeltaTime * -1;
                }
                else
                {
                    lateralSpeed = Vector2.zero;
                }
            }
            else
            {
                Vector2 lateralSpeedGain = Vector2.zero;

                lateralSpeedGain = (leftStickInput.normalized * airborneMovementAcceleration * Time.fixedDeltaTime).Rotate(Quaternion.Inverse(transform.rotation) * Quaternion.Euler(0, cameraYAngle, 0));
                lateralSpeed += lateralSpeedGain;
                if (lateralSpeed.magnitude > airborneMovementSpeed)
                {
                    lateralSpeed = lateralSpeed.normalized * airborneMovementSpeed;
                }
            }

            velocity.x = lateralSpeed.x;
            velocity.z = lateralSpeed.y;
        }
    }
    void Hop()
    {
        if (canHop)
        {
            if (Input.GetButtonDown("A Button"))
            {
                canHop = false;
                if (velocity.y < 0)
                    velocity.y = 0;
                velocity.y += hopVelocity;
                StartCoroutine(SuspendGroundedCheck());
            }
        }
        else
        {
            if (Grounded())
                canHop = true;
        }
    }
    void Gravity()
    {
        if (!Grounded())
        {
            if (velocity.y > -maximumFallingSpeed)
                velocity.y -= gravityStrength * Time.fixedDeltaTime;

            Ray ceilingDetectRay = new Ray(transform.position, transform.up);
            if (Physics.SphereCast(ceilingDetectRay, .5f, .1f))
            {
                if (velocity.y > 0)
                    velocity.y = 0;
            }
        }
    }

    void RunAnimation()
    {
        //Vieze animatie code

        //Set Animation States
        if (Grounded())
        {
            if (leftStickInput.magnitude == 0)
            {
                isBouncing = false;
            }
            else if (leftStickInput.magnitude < walkingBouncingThreshold)
            {
                if (!isLaunching)
                {
                    isBouncing = true;
                }
                else
                {
                    isBouncing = false;
                }
            }
            else
            {
                if (!isLaunching)
                {
                    isBouncing = true;
                }
                else
                {
                    isBouncing = false;
                }
            }
        }

        //Play Launch Animation
        if (isLaunching)
        {
            animator.SetBool("IsLaunching", true);
        }
        else
        {
            animator.SetBool("IsLaunching", false);
        }

        //Play Bounce Animation
        if (isBouncing)
        {
            animator.SetBool("IsBouncing", true);
        }
        else
        {
            animator.SetBool("IsBouncing", false);
        }
    }


    //RETURN FUNCTIONS
    bool Grounded()
    {
        if (groundedSuspended)
        {
            return false;
        }

        Ray groundedRay = new Ray(transform.position, Vector3.up * -1);
        if (Physics.SphereCast(groundedRay, .42f, .1f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    //COROUTINES
    IEnumerator LaunchRoutine()
    {
        launchCharge = 0;
        launchChargeDisplay.sizeDelta = new Vector2(0, launchChargeDisplayHeight);

        while (Input.GetAxis("Right Trigger") != 0)
        {
            launchChargeDisplay.sizeDelta = new Vector2(launchChargeDisplayMaxWidth * launchCharge, launchChargeDisplayHeight);
            launchCharge = Mathf.Clamp(launchCharge + launchChargeSpeed * Time.deltaTime, 0, 1);
            yield return null;
        }

        while (!Grounded())
        {
            yield return null;
        }

        //COPY PASTE THIS!
        StartCoroutine(SetLaunchAnimation());
    }
    IEnumerator SuspendGroundedCheck(float suspensionTime = .1f)
    {
        groundedSuspended = true;
        yield return new WaitForSeconds(suspensionTime);
        groundedSuspended = false;
    }

    //COPY PASTE THIS!!!
    IEnumerator SetLaunchAnimation()
    {
        isLaunching = true;
        yield return new WaitForSeconds(0.45f);
        isLaunching = false;
        if (!isLaunching)
        {
            launchChargeDisplay.sizeDelta = new Vector2(0, launchChargeDisplayHeight);
            velocity = minLaunchVelocity + (maxLaunchVelocity - minLaunchVelocity) * launchCharge;
            StopCoroutine(SuspendGroundedCheck());
            StartCoroutine(SuspendGroundedCheck());
            launchRoutineRunning = false;
        }
    }

    //SNOW MECHANICS FUNCTIONS
    IEnumerator ISnowTornado.HitBySnowTornado(Transform tornadoTrans, Vector3 playerOffsetFromCenter, float spinSpeed, float playerLerpFactor, Vector3 releaseVelocity)
    {
        if (inTornado && !canBeisSpinning)
            yield break;

        inTornado = true;
        canBeisSpinning = false;

        tornadoTrans.forward = -transform.right;

        while (true)
        {
            snowTornadoDesiredPlayerPosition = tornadoTrans.forward + playerOffsetFromCenter;
            transform.position = Vector3.Lerp(transform.position, tornadoTrans.position + tornadoTrans.rotation * playerOffsetFromCenter, playerLerpFactor);
            transform.rotation *= Quaternion.Euler(new Vector3(0, spinSpeed * Time.deltaTime, 0));

            if (Input.GetButtonDown("A Button"))
            {
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