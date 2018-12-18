using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroTekstScript : MonoBehaviour {

    public Image tekstImage;
    public Image background;
    bool fadingToBlack = false;
    bool tekstFadeAway = false;
    bool tekstFadeIn = false;

    Color tempColor;

    GameObject player;
    PlayerController controllerSwitch;

    void Start()
    {
        player = GameObject.Find("Character");
        controllerSwitch = player.GetComponent<PlayerController>();
        controllerSwitch.enabled = false;
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

        if (fadingToBlack == true)
        {
            var tempColor2 = background.color;
            tempColor2.a -= 0.005f;
            background.color = tempColor2;
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
        fadingToBlack = true;

        yield return new WaitForSeconds(1f);
        controllerSwitch.enabled = true;
        print("lol");
        //Destroy(gameObject);
    }
}
