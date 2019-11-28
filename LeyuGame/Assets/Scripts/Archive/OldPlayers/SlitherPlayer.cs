using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlitherPlayer : MonoBehaviour, IPulsingSnow, ISnowball, ISnowTornado
{

    [Header("Movement stats")]
    public float rotationSensitivity = 3;
    public int accelerationSpeed = 3;
    public int maxAcceleratedSpeed = 60;
    public int deAccelerationSpeed = 5;
    public int normalSpeed = 10;
    public float slitherSpeed = 10;

    float acceleratedSpeed;
    float countMovementOne;
    float countMovementTwo;
    float cameraYAngle;

    //bools for states
    bool isMoving, isMovingForward, startCounting, startCountingWhenStrafing, isSlithering, isSurfing, isPushed;

    //bools for acceleration
    bool isMovingRight, isMovingLeft, hasChangedDirection;

    Vector3 _movementVector;
    Rigidbody myRig;

    //SNOWPULSE
    Vector3 pulseDirection;
    float pulseForce;

    //SNOWBALL
    float playerPushForce;
	float playerPushTime;
    Vector3 playerPushDirection;
    Vector3 newDirection;
    bool lockMovement;

    //SNOWTORNADO
    //[Header("SnowTornado Settings")]
    bool isSpinning = false, canBeisSpinning = true;
    Vector3 snowTornadoDesiredPlayerPosition;

    void Awake()
    {
        myRig = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        //SET MOVEMENTVECTOR.Y (OTHERWISE THE PLAYER WON'T FALL)
        _movementVector.y = myRig.velocity.y;

        MoveForward();
        RotatePlayer();

        //CHOOSE MOVEMENT MODE
        if (!isSpinning)
        {
            if (!isPushed)
            {
                if (isSlithering)
                {
                    MoveLeftRight();
                    AccelerateMovement();
                }
                else
                {
                    NormalMovement();
                }
            }
            else
            {
                StartPushing();
            }
        }


        //EXECUTE MOVEMENT
        myRig.velocity = transform.rotation * _movementVector;
    }

    void NormalMovement()
    {
        _movementVector.z = Input.GetAxis("Left Stick Y") * normalSpeed;
        _movementVector.x = Input.GetAxis("Left Stick X") * normalSpeed;

        if (isSurfing)
        {
            AddPulseForce();
        }
    }

    void MoveForward()
    {
        if (Input.GetAxis("Right Trigger") > 0) {
            isSlithering = true;
            _movementVector.z = slitherSpeed + acceleratedSpeed;
            isMovingForward = true;
        }
        else {
            isSlithering = false;

            //_movementVector.z = 0;
            acceleratedSpeed = 0;
            isMovingForward = false;
        }
    }

    void MoveLeftRight()
    {
        if (isMovingForward) {
            //MOVE RIGHT
            if (Input.GetAxis("Left Stick X") > 0) {
                _movementVector.x = Input.GetAxis("Left Stick X") * slitherSpeed + acceleratedSpeed;
                isMoving = true;

                if (!startCountingWhenStrafing) {
                    StartCoroutine(DeAccelerateWhenStrafing());
                }
                if (isMovingLeft) {
                    isMovingLeft = false;
                    hasChangedDirection = true;
                }
                isMovingRight = true;
            }
            //MOVE LEFT
            if (Input.GetAxis("Left Stick X") < 0) {
                _movementVector.x = Input.GetAxis("Left Stick X") * slitherSpeed + (-1 * acceleratedSpeed);
                isMoving = true;

                if (!startCountingWhenStrafing) {
                    StartCoroutine(DeAccelerateWhenStrafing());
                }
                if (isMovingRight) {
                    isMovingRight = false;
                    hasChangedDirection = true;
                }
                isMovingLeft = true;
            }
            //STOP WHEN NOT STEERING
            if (Input.GetAxis("Left Stick X") == 0) {
                _movementVector.x = 0;
                if (!startCounting) {
                    StartCoroutine(DeAccelerate());
                }
            }
        }
        else {
            _movementVector.x = 0;
        }

        if (isSurfing) {
            AddPulseForce();
        }
    }

    void AccelerateMovement()
    {
        //MOVE LEFT
        if (Input.GetAxis("Left Stick X") > 0) {
            countMovementOne += 1 * Time.deltaTime;
            if (countMovementOne > countMovementTwo && countMovementTwo != 0) {
                countMovementOne = 0;
                countMovementTwo = 0;
                acceleratedSpeed += accelerationSpeed;
            }
        }
        //MOVE RIGHT
        if (Input.GetAxis("Left Stick X") < 0) {
            countMovementTwo += 1 * Time.deltaTime;
            if (countMovementTwo > countMovementOne && countMovementOne != 0) {
                countMovementOne = 0;
                countMovementTwo = 0;
                acceleratedSpeed += accelerationSpeed;
            }
        }
        acceleratedSpeed = Mathf.Clamp(acceleratedSpeed, 0, maxAcceleratedSpeed);
    }

    void RotatePlayer()
    {
        cameraYAngle = Input.GetAxis("Right Stick X") * rotationSensitivity;
        transform.eulerAngles = transform.eulerAngles - new Vector3(0, cameraYAngle, 0);
    }

    //SPECIAL MOVEMENT
    void AddPulseForce()
    {
        //SNOWPULSE IMPLEMENTATION
        if (isSurfing)
        {
            Vector3 snowPulseDirection = Quaternion.Inverse(transform.rotation) * (transform.position - pulseDirection);
            _movementVector += new Vector3(snowPulseDirection.x, 0, snowPulseDirection.z).normalized * pulseForce;
            isSurfing = false;
        }
    }

    IEnumerator ISnowTornado.HitBySnowTornado(Transform tornadoTrans, Vector3 playerOffsetFromCenter, float spinSpeed, float playerLerpFactor, Vector3 releaseVelocity)
    {
        if (isSpinning && !canBeisSpinning)
            yield break;

        isSpinning = true;

        tornadoTrans.forward = -transform.right;

        while (true)
        {
            snowTornadoDesiredPlayerPosition = tornadoTrans.forward + playerOffsetFromCenter;
            transform.position = Vector3.Lerp(transform.position, tornadoTrans.position + tornadoTrans.rotation * playerOffsetFromCenter, playerLerpFactor);
            cameraYAngle += spinSpeed * Time.deltaTime;
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, spinSpeed * Time.deltaTime, 0);

            if (Input.GetButtonDown("A Button"))
            {
                _movementVector = releaseVelocity;
                transform.forward = Quaternion.Euler(new Vector3(0, cameraYAngle, 0)) * Vector3.forward;
                StartCoroutine(LaunchFromTornado());
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        canBeisSpinning = true;
    }

    void StartPushing()
    {
        if (!lockMovement)
        {
            newDirection = transform.position - playerPushDirection;
            newDirection = newDirection * playerPushForce;
            _movementVector = newDirection;
            lockMovement = true;
            StartCoroutine(StopBeingPushed());
        }
    }

    //INTERFACES
    public void HitByPulsingSnow(Vector3 origin, float pushVelocity)
    {
        pulseDirection = origin;
        pulseForce = pushVelocity;
        isSurfing = true;
    }

    public void HitBySnowball(float pushForce, float snowballPushTime, Vector3 pushDirection)
    {
        playerPushForce = pushForce;
		playerPushTime = snowballPushTime;
        playerPushDirection = pushDirection;
        isPushed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //raycast would be better. Also, player won't lose speed when colliding at the moment
        if (collision.gameObject.tag == "Flag") {
            //acceleratedSpeed = 0;
            isMovingForward = false;
        }
    }

    IEnumerator DeAccelerate()
    {
        isMoving = false;
        startCounting = true;
        yield return new WaitForSeconds(0.5F);
        if (!isMoving) {
            while (acceleratedSpeed > 0 && !isMoving) {
                acceleratedSpeed -= deAccelerationSpeed;
                yield return new WaitForSeconds(0.1F);
            }
        }
        startCounting = false;
    }

    IEnumerator DeAccelerateWhenStrafing()
    {
        hasChangedDirection = false;
        startCountingWhenStrafing = true;
        yield return new WaitForSeconds(1F);
        if (!hasChangedDirection) {
            while (acceleratedSpeed > 0) {
                acceleratedSpeed -= deAccelerationSpeed;
                yield return new WaitForSeconds(0.1F);
            }
        }
        startCountingWhenStrafing = false;
        hasChangedDirection = false;
    }

    IEnumerator StopBeingPushed()
    {
        yield return new WaitForSeconds(playerPushTime);
        lockMovement = false;
        isPushed = false;
    }

    IEnumerator LaunchFromTornado()
    {
        yield return new WaitForSeconds(1F);
        isSpinning = false;
    }
}