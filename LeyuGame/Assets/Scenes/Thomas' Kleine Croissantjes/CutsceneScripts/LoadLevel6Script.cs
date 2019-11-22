using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevel6Script : MonoBehaviour {

    public Image image;
    bool FadingToBlack = false;

    Color tempColor;

    void Start()
    {
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FadingToBlack = true;
        }
    }

    void Update()
    {
        if (FadingToBlack == true)
        {
            var tempColor = image.color;
            tempColor.a += 0.02f;
            image.color = tempColor;
        }

        if (FadingToBlack == true && image.color.a >= 1)
        {
            StartCoroutine(CutsceneTime());
        }

    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(1f);
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("level 6");
    }

}
