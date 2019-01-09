using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using UnityEngine;

public class LaunchTest : MonoBehaviour {

    [Header("Test Stuff")]
    public Vector3 launchStageOneForce = new Vector3(0, 35, 10), launchStageTwoForce = new Vector3(0, 50, 22);
    public Renderer launchRenderer;
    public Color launchStageOneColor = Color.green, launchStageTwoColor = Color.red;
    public float launchStageTwoTime = .7f;
    public LayerMask triggerMask;

    [Header("Gravity Settings")]
    public float gravityStrength = 48;
    public float maximumFallingSpeed = 112;

    GameObject player;
    PlayerController playerScript;
    Vector3 velocity;
    Rigidbody rig;

    bool isBuildingLaunch, isLaunchingSuperSaiyan, canHop, playerIsAirborne, isPreLaunching;
    bool launchRoutineRunning = false;
    int[] launchMaterialIndexes = new int[] { 1, 3, 4 };
    Color launchBaseColor = Color.white;
    bool groundedSuspended = false;

    private void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        rig = player.GetComponent<Rigidbody>();

        GamePad.SetVibration(0, 0, 0);

        StartCoroutine(TestLaunch());
    }

    private void Update()
    {
        rig.velocity = transform.rotation * velocity;
    }

    private void FixedUpdate()
    {
        Gravity();
    }

    IEnumerator TestLaunch()
    {
        yield return new WaitForSeconds(0f);
        launchRoutineRunning = true;
        StartCoroutine(LaunchRoutine());
        StartCoroutine(EnablePlayer());
    }

    IEnumerator EnablePlayer()
    {
        yield return new WaitForSeconds(1.8f);
        playerScript.enabled = true;
    }

    IEnumerator LaunchRoutine()
    {
        float timeLapsed = 0;
        bool stageTwoReached = false;

        GamePad.SetVibration(PlayerIndex.One, .1f, .1f);

        for (int i = 0; i < launchMaterialIndexes.Length; i++)
        {
            launchRenderer.materials[launchMaterialIndexes[i]].color = launchStageOneColor;
        }

        while (Input.GetAxis("Right Trigger") != 0 || Input.GetButton("Keyboard Space"))
        {
            isBuildingLaunch = true;
            timeLapsed += Time.deltaTime;

            if (timeLapsed > launchStageTwoTime)
            {
                stageTwoReached = true;
                GamePad.SetVibration(PlayerIndex.One, .3f, .3f);
                for (int i = 0; i < launchMaterialIndexes.Length; i++)
                {
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

        if (!stageTwoReached)
        {
            //stage 1
            isLaunchingSuperSaiyan = true;
            velocity = new Vector3(velocity.x, 0, velocity.z).normalized * launchStageOneForce.z;
            velocity.y = launchStageOneForce.y;
        }
        else
        {
            //stage 2
            isLaunchingSuperSaiyan = false;
            velocity = new Vector3(velocity.x, 0, velocity.z).normalized * launchStageTwoForce.z;
            velocity.y = launchStageTwoForce.y;
        }
        canHop = true;

        StartCoroutine(PreLaunchRoutine());
        StopCoroutine(SuspendGroundedCheck());
        StartCoroutine(SuspendGroundedCheck());

        while (!Grounded())
        {
            yield return null;
        }

        for (int i = 0; i < launchMaterialIndexes.Length; i++)
        {
            launchRenderer.materials[launchMaterialIndexes[i]].color = launchBaseColor;
        }
        launchRoutineRunning = false;
    }

    void KillVibration(float timeBeforeKill = .1f)
    {
        StopCoroutine(KillVibrationRoutine());
        StartCoroutine(KillVibrationRoutine(timeBeforeKill));
    }

    IEnumerator KillVibrationRoutine(float timeBeforeKill = 0.1f)
    {
        yield return new WaitForSeconds(timeBeforeKill);
        GamePad.SetVibration((PlayerIndex)0, 0, 0);
    }

    bool Grounded()
    {
        if (groundedSuspended)
        {
            return false;
        }

        Ray groundedRay = new Ray(transform.position, Vector3.up * -1);
        RaycastHit groundedRayHit;
        if (Physics.SphereCast(groundedRay, .42f, out groundedRayHit, .1f, triggerMask))
        {
            playerIsAirborne = false;

            //beetje lelijk dit
            canHop = true;
            if (!isBuildingLaunch)
            {
                for (int i = 0; i < launchMaterialIndexes.Length; i++)
                {
                    launchRenderer.materials[launchMaterialIndexes[i]].color = launchBaseColor;
                }
            }

            return true;
        }
        else
        {
            playerIsAirborne = true;
            return false;
        }
    }

    IEnumerator PreLaunchRoutine()
    {
        isPreLaunching = true;
        yield return new WaitForSeconds(0.2F);
        isPreLaunching = false;
    }

    IEnumerator SuspendGroundedCheck(float suspensionTime = .1f)
    {
        groundedSuspended = true;
        yield return new WaitForSeconds(suspensionTime);
        groundedSuspended = false;
    }

    void Gravity()
    {
        if (!Grounded())
        {
            if (velocity.y > -maximumFallingSpeed)
                velocity.y -= gravityStrength * Time.fixedDeltaTime;

            Ray ceilingDetectRay = new Ray(transform.position, transform.up);
            if (Physics.SphereCast(ceilingDetectRay, .2f, .35f, triggerMask))
            {
                if (velocity.y > 0)
                    velocity.y = 0;
            }
        }
        else
        {
            velocity.y = 0;
        }
    }

}