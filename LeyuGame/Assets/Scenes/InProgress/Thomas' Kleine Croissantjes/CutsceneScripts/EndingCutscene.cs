using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingCutscene : MonoBehaviour {

    public Image tekstImage;
    public Image background;
    bool fadingToWhite = false;
    bool fadingFromWhite = false;
    bool tekstFadeAway = false;
    bool tekstFadeIn = false;

    Color tempColor;
    Color tempColorTwee;

    void Start()
    {
        var tempColor = tekstImage.color;
        tempColor.a = 0f;
        tekstImage.color = tempColor;

        var tempColorTwee = background.color;
        tempColorTwee.a = 0f;
        background.color = tempColorTwee;
    }

    void OnTriggerEnter()
    {
        StartCoroutine(CutsceneTime());
        fadingToWhite = true;
        tekstFadeIn = true;
    }

    void Update()
    {
        if (fadingToWhite == true)
        {
            var tempColorTwee = background.color;
            tempColorTwee.a += 0.015f;
            background.color = tempColorTwee;
        }

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
            var tempColorTwee = background.color;
            tempColorTwee.a -= 0.005f;
            background.color = tempColorTwee;
        }
    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(3f);
        tekstFadeIn = false;
        fadingToWhite = false;

        yield return new WaitForSeconds(3f);
        tekstFadeAway = true;

        yield return new WaitForSeconds(2f);
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("TitleScreen");
        print("lol");
    }
}