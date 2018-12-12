using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonUnderSnowScript : MonoBehaviour {

    public Image image;
    bool fadingToWhite = false;
    bool playerCanMove = false;
    bool playerHasMoved = false;
    bool cameraMoving = false;

    Color tempColor;

    GameObject player;
    GameObject playerCamera;
    PlayerController controllerSwitch;

    public GameObject destructibleBoi;
    public ParticleSystem snowExplosionPrefab;

    public GameObject cutsceneCamera;
    Animator cameraAnim;

    Vector3 distanceToPlayerCam;

    void Start()
    {
        StartCoroutine(CutsceneTime());
        player = GameObject.Find("Character");
        playerCamera = GameObject.Find("Main Camera");
        controllerSwitch = player.GetComponent<PlayerController>();
        controllerSwitch.enabled = false;
        cameraAnim = cutsceneCamera.GetComponent<Animator>();
    }

    void Update()
    {
        if (fadingToWhite == true)
        {
            var tempColor = image.color;
            tempColor.a -= 0.015f;
            image.color = tempColor;
        }

        if (playerCanMove == true)
        {
            if (Input.GetButtonDown("A Button") || Mathf.Abs(Input.GetAxis("Left Stick X")) > 0 ||
                Mathf.Abs(Input.GetAxis("Left Stick Y")) > 0)
            {
                playerHasMoved = true;
            }
        }

        if (playerHasMoved == true)
        {    
            cutsceneCamera.transform.LookAt(player.transform.position);
            //LOOK AT WHRE PLAYER CAMERA LOOKS AT (after snowexplosion)

            if (cameraMoving == true)
            {
                cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, 25 * Time.deltaTime);
            }



            //TRIES TO MOVE TO ROTATION OF PLAYER MAIN CAMERA
            //cutsceneCamera.transform.localRotation = Vector3.RotateTowards(cutsceneCamera.transform.localRotation, playerCamera.transform.localRotation, 50 * Time.deltaTime, 0.0f);
            //cutsceneCamera.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.x, playerCamera.transform.rotation.y, playerCamera.transform.rotation.z,);
        }

        distanceToPlayerCam = cutsceneCamera.transform.position - playerCamera.transform.position;
        distanceToPlayerCam = new Vector3(Mathf.Abs(distanceToPlayerCam.x), distanceToPlayerCam.y, distanceToPlayerCam.z);

        /*
        if (distanceToPlayerCam.x < 1 && distanceToPlayerCam.y < 1 && distanceToPlayerCam.z < 1)
        {
            cutsceneCamera.transform.rotation = Quaternion.Euler(Mathf.Lerp(cutsceneCamera.transform.rotation.x, playerCamera.transform.rotation.x, 0.01f * Time.deltaTime), Mathf.Lerp(cutsceneCamera.transform.rotation.y, playerCamera.transform.rotation.y, 25 * Time.deltaTime), Mathf.Lerp(cutsceneCamera.transform.rotation.z, playerCamera.transform.rotation.z, 25 * Time.deltaTime));
        }
        else
        {
            if (playerHasMoved == true)
            {
                cutsceneCamera.transform.LookAt(player.transform.position);
            }
        }
        */

        if (distanceToPlayerCam.x < 0.1f && distanceToPlayerCam.y < 0.1f && distanceToPlayerCam.z < 0.1f)
        {
            cutsceneCamera.SetActive(false);
            cameraMoving = false;
        }
    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(1f);
        fadingToWhite = true;

        yield return new WaitForSeconds(7f);
        cameraAnim.enabled = false;
        playerCanMove = true;

        while (playerHasMoved == false)
        {
            yield return null;
        }

        controllerSwitch.enabled = true;
        print("playerHasMoved = " + playerHasMoved);
        ParticleSystem snowExplosion = Instantiate(snowExplosionPrefab) as ParticleSystem;
        snowExplosion.transform.position = player.transform.position;
        //snowExplosion.SetActive(false);
        Destroy(destructibleBoi);

        yield return new WaitForSeconds(0.5f);
        cameraMoving = true;

        //Not really necessary but for performance I guess.
        while (cameraMoving == true)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}
