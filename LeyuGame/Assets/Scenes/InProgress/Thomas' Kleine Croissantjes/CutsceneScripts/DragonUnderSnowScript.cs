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
    float cameraSpeed = 0;

    void Start()
    {
        StartCoroutine(CutsceneTime());
        player = GameObject.Find("Character");
        playerCamera = GameObject.Find("Main Camera");
        controllerSwitch = player.GetComponent<PlayerController>();
        controllerSwitch.launchEnabled = true;
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
            //PLAYER MUST LAUNCH OUT OF SNOW
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
                cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, cameraSpeed * Time.deltaTime);
                cutsceneCamera.transform.rotation = Quaternion.RotateTowards(cutsceneCamera.transform.rotation, playerCamera.transform.rotation, cameraSpeed * Time.deltaTime);
                cameraSpeed += 1f;
            }
        }

        distanceToPlayerCam = cutsceneCamera.transform.position - playerCamera.transform.position;
        distanceToPlayerCam = new Vector3(Mathf.Abs(distanceToPlayerCam.x), distanceToPlayerCam.y, distanceToPlayerCam.z);
        distanceToPlayerCam.x = Mathf.Abs(distanceToPlayerCam.x);
        distanceToPlayerCam.y = Mathf.Abs(distanceToPlayerCam.y);
        distanceToPlayerCam.z = Mathf.Abs(distanceToPlayerCam.z);

        if (distanceToPlayerCam.x < 0.7f && distanceToPlayerCam.y < 0.7f && distanceToPlayerCam.z < 0.7f)
        {
            cutsceneCamera.SetActive(false);
            cameraMoving = false;
        }
    }

    IEnumerator CutsceneTime()
    {
        Level3Music.musicStage = 5.8f;
        yield return new WaitForSeconds(1f);
        fadingToWhite = true;

        yield return new WaitForSeconds(7f);
        cameraAnim.enabled = false;
        playerCanMove = true;

        while (playerHasMoved == false)
        {
            yield return null;
        }
        Level3Music.musicStage = 5.9f;
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
