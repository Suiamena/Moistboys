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

    Color tempColor;

    GameObject player;
    //GameObject playerCamera;
    PlayerController controllerSwitch;

    public GameObject cutsceneCamera;
    Animator cameraAnim;

    public GameObject draakBeweging;
    Animator draakAnim;

    void Start()
    {
        player = GameObject.Find("Character");
        controllerSwitch = player.GetComponent<PlayerController>();
        controllerSwitch.enabled = false;
        cameraAnim = cutsceneCamera.GetComponent<Animator>();
        cameraAnim.enabled = false;
        draakAnim = draakBeweging.GetComponent<Animator>();
        draakAnim.enabled = false;
        StartCoroutine(CutsceneTime());
    }

    // Update is called once per frame
    void Update()
    {
        if (tekstFadeIn == true)
        {
            var tempColor = tekstImage.color;
            tempColor.a += 0.015f;
            tekstImage.color = tempColor;
        }

        if (tekstFadeAway == true)
        {
            var tempColor = tekstImage.color;
            tempColor.a -= 0.015f;
            tekstImage.color = tempColor;
        }

        if (fadingFromWhite == true)
        {
            var tempColor2 = background.color;
            tempColor2.a -= 0.005f;
            background.color = tempColor2;

            if (tempColor2.a <= 0)
            {

            }
        }
    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(1f);
        tekstFadeIn = true;

        yield return new WaitForSeconds(3f);
        tekstFadeIn = false;
        tekstFadeAway = true;

        yield return new WaitForSeconds(3f);
        fadingFromWhite = true;
        cameraAnim.enabled = true;

        yield return new WaitForSeconds(10f);
        draakAnim.enabled = true;

        yield return new WaitForSeconds(2f);
        controllerSwitch.enabled = true;
        warmthSourceSoundObject.SetActive(true);
        Destroy(gameObject);
    }
}
