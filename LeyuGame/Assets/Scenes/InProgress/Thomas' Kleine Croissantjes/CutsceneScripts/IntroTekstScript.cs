using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroTekstScript : MonoBehaviour {

    public GameObject warmthSourceSoundObject;
    public Image tekstImage;
    public Image background;
    bool fadingFromWhite = false;
    bool tekstFadeAway = false;
    bool tekstFadeIn = false;

    bool playerCanMove = false;
    bool playerHasMoved = false;
    bool cameraMoving = false;
    float cameraDistance;
    float cameraSpeed = 0;
    public GameObject playerCamera;

    Color tempColor;

    GameObject player;
    //GameObject playerCamera;
    PlayerController controllerSwitch;

    public GameObject cutsceneCamera;
    Animator cameraAnim;

    public GameObject draakBeweging;
    Animator draakAnim;

    void Start() {
        player = GameObject.Find("Character");
        controllerSwitch = player.GetComponent<PlayerController>();
        controllerSwitch.launchEnabled = false;
        controllerSwitch.DisablePlayer(true);
        controllerSwitch.transform.rotation = Quaternion.Euler(0, 90, 0);
        playerCamera.transform.position = controllerSwitch.transform.position + Quaternion.Euler(0, 90, 0) * (playerCamera.transform.position - controllerSwitch.transform.position);
        //controllerSwitch.enabled = false;
        cameraAnim = cutsceneCamera.GetComponent<Animator>();
        cameraAnim.enabled = false;
        draakAnim = draakBeweging.GetComponent<Animator>();
        draakAnim.enabled = false;
        StartCoroutine(CutsceneTime());
    }

    void Update() {
        //player.transform.eulerAngles = new Vector3(player.transform.eulerAngles.x, 0, player.transform.eulerAngles.z);
        //Quaternion target = Quaternion.Euler(player.transform.rotation.x, 0, player.transform.rotation.z);
        //player.transform.rotation = target;
        //player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, Mathf.Clamp(player.transform.rotation.y, 0, 0), player.transform.rotation.z);

        if (tekstFadeIn == true) {
            var tempColor = tekstImage.color;
            tempColor.a += 0.015f;
            tekstImage.color = tempColor;
        }

        if (tekstFadeAway == true) {
            var tempColor = tekstImage.color;
            tempColor.a -= 0.015f;
            tekstImage.color = tempColor;
        }

        if (fadingFromWhite == true) {
            var tempColor2 = background.color;
            tempColor2.a -= 0.005f;
            background.color = tempColor2;

            //if (tempColor2.a <= 0)
            //{

            //}
        }

        if (cameraMoving == true) {
            cutsceneCamera.transform.position = Vector3.MoveTowards(cutsceneCamera.transform.position, playerCamera.transform.position, cameraSpeed * Time.deltaTime);
            cutsceneCamera.transform.LookAt(player.transform);
            cameraSpeed += 0.5f;
        }

        cameraDistance = Vector3.Distance(cutsceneCamera.transform.position, playerCamera.transform.position);
        //print(cameraDistance);

        if (cameraDistance < 0.01f) {
            controllerSwitch.EnablePlayer();
            //controllerSwitch.enabled = true;
            cutsceneCamera.SetActive(false);
            cameraMoving = false;
            Destroy(gameObject);
        }
    }

    IEnumerator CutsceneTime() {
        yield return new WaitForSeconds(1f);
        tekstFadeIn = true;

        yield return new WaitForSeconds(3f);
        tekstFadeIn = false;
        tekstFadeAway = true;

        yield return new WaitForSeconds(3f);
        fadingFromWhite = true;
        cameraAnim.enabled = true;

        yield return new WaitForSeconds(14.5f);
        draakAnim.enabled = true;

        yield return new WaitForSeconds(2.75f);
        warmthSourceSoundObject.SetActive(true);
        cameraMoving = true;
        print(cameraMoving);
        cameraAnim.enabled = false;
        //Destroy(gameObject);
    }
}
