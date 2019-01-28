using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// WHAT THE FUCK!!!!!!!!

public class DragonUnderSnowScript : MonoBehaviour {

    public Image image;
    bool fadingToWhite = false;
    bool playerCanMove = false;
    bool playerHasMoved = false;
    bool cameraMoving = false;

    bool firstLaunch = false;
    int chargingTime;

    Color tempColor;

    GameObject player;
    GameObject playerCamera;
    public GameObject playerModel;
    Animator playerAnim;
    PlayerController controllerSwitch;

    public GameObject destructibleBoi;
    //public ParticleSystem snowExplosionPrefab;    //Voor de zekerheid gecomment lol.
    public GameObject snowExplosion;

    public GameObject cutsceneCamera;
    Animator cameraAnim;

    //Vector3 distanceToPlayerCam;
    float cameraDistance;
    float cameraSpeed = 0;

    public GameObject rightTriggerUI;
    public Image RTOrangeUI;
    public Image RTRedUI;
    Color AlphaVar = new Color(1,1,1,0);
    bool fadingOut = false;
    bool fadingIn = true;

    void Start()
    {
        StartCoroutine(CutsceneTime());
        player = GameObject.Find("Character");
        playerCamera = GameObject.Find("Main Camera");
        playerCamera.SetActive(false);
        playerAnim = playerModel.GetComponent<Animator>();
        playerAnim.enabled = false;
        controllerSwitch = player.GetComponent<PlayerController>();
        cameraAnim = cutsceneCamera.GetComponent<Animator>();
        rightTriggerUI.SetActive(false);
        controllerSwitch.enabled = false;
        controllerSwitch.launchEnabled = true;
    }

    void Update()
    {
        if (fadingToWhite == true)
        {
            var tempColor = image.color;
            tempColor.a -= 0.015f;
            image.color = tempColor;
        }

        if (playerHasMoved == true)
        {    
            cutsceneCamera.transform.LookAt(player.transform.position);
            //LOOK AT WHRE PLAYER CAMERA LOOKS AT (after snowexplosion)

            if (cameraMoving == true)
            {
                cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, cameraSpeed * Time.deltaTime);
                cutsceneCamera.transform.rotation = Quaternion.RotateTowards(cutsceneCamera.transform.rotation, playerCamera.transform.rotation, 3*cameraSpeed * Time.deltaTime);
                cameraSpeed += 1f;
            }
        }

        cameraDistance = Vector3.Distance(cutsceneCamera.transform.position, playerCamera.transform.position);
        print(cameraDistance);

        if (cameraDistance < 0.1f)
        {
            cutsceneCamera.SetActive(false);
            playerCamera.SetActive(true);
            cameraMoving = false;
            print("loooolll");
        }
        //distanceToPlayerCam = cutsceneCamera.transform.position - playerCamera.transform.position;
        //distanceToPlayerCam = new Vector3(Mathf.Abs(distanceToPlayerCam.x), distanceToPlayerCam.y, distanceToPlayerCam.z);
        //distanceToPlayerCam.x = Mathf.Abs(distanceToPlayerCam.x);
        //distanceToPlayerCam.y = Mathf.Abs(distanceToPlayerCam.y);
        //distanceToPlayerCam.z = Mathf.Abs(distanceToPlayerCam.z);

        //if (distanceToPlayerCam.x < 0.7f && distanceToPlayerCam.y < 0.7f && distanceToPlayerCam.z < 0.7f)
        //{
        //    cutsceneCamera.SetActive(false);
        //    cameraMoving = false;
        //}
        
        if (playerCanMove == true)
        {
            var tempColor = RTRedUI.color;
            tempColor.a += 0.05f;
            RTRedUI.color = tempColor;

            if (RTRedUI.color.a >= 1f)
            {
                if (RTOrangeUI.color.a >= 1f)
                {
                    //var tempColor2 = RTOrangeUI.color;
                    //tempColor2.a -= 0.01f;
                    //RTOrangeUI.color = tempColor2;
                    fadingIn = false;
                    fadingOut = true;
                }

                if (RTOrangeUI.color.a <= 0f)
                {
                    fadingIn = true;
                    fadingOut = false;
                }

                if (fadingIn == true)
                {
                    AlphaVar.a += 0.05f;
                }

                if (fadingOut == true)
                {
                    AlphaVar.a -= 0.03f;
                }

                RTOrangeUI.color = AlphaVar;
            } 
        }
    }

    //void UIDelay()
    //{
    //    playerCanMove = true;
    //    rightTriggerUI.SetActive(true);
    //}

    IEnumerator CutsceneTime()
    {
        Level3Music.musicStage = 5.8f;
        yield return new WaitForSeconds(1f);
        fadingToWhite = true;

        yield return new WaitForSeconds(7f);
        cameraAnim.enabled = false;
        controllerSwitch.enabled = true;
        controllerSwitch.enableLaunchOnly = true;


        //Invoke("UIDelay", 1);
        rightTriggerUI.SetActive(true);
        playerCanMove = true;

        //while (!firstLaunch)
        //{
        //    while (Input.GetAxis("Right Trigger") == 0)
        //    {
        //        yield return null;
        //    }

        //    //START PRESSING
        //    while (Input.GetAxis("Right Trigger") > 0)
        //    {
        //        chargingTime += 1;
        //        Debug.Log(chargingTime);
        //        yield return null;
        //    }
        //    if (chargingTime > 35)
        //    {
        //        firstLaunch = true;
        //    }
        //    chargingTime = 0;
        //}

        //while (Input.GetAxis("Right Trigger") == 0 || Input.GetButtonDown("Keyboard Space") == false)
        //{
        //    yield return null;
        //}
        //while (Input.GetAxis("Right Trigger") != 0 || Input.GetButton("Keyboard Space"))
        //{
        //    yield return null;
        //}

        while (Input.GetAxis("Right Trigger") == 0)
        {
            yield return null;
        }
        while (Input.GetAxis("Right Trigger") > 0)
        {
            yield return null;
        }

        playerAnim.enabled = true;
        controllerSwitch.enableLaunchOnly = false;
        playerHasMoved = true;
        rightTriggerUI.SetActive(false);

        Level3Music.musicStage = 5.9f;
        print("playerHasMoved = " + playerHasMoved);
        //ParticleSystem snowExplosion = Instantiate(snowExplosionPrefab) as ParticleSystem;
        //snowExplosion.transform.position = player.transform.position;
        snowExplosion.SetActive(true);
        Destroy(destructibleBoi);

        //yield return new WaitForSeconds(1f);
        cameraMoving = true;

        //Not really necessary but for performance I guess.
        while (cameraMoving == true)
        {
            yield return null;
        }
        Level3Music.startMusic = true;
        Destroy(snowExplosion);
        Destroy(gameObject);
    }
}
