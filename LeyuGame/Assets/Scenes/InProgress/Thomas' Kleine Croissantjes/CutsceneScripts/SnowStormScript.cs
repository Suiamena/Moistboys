using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SnowStormScript : MonoBehaviour {

    public Image image;
    bool FadingToWhite = false;

    Color tempColor;

	void Start ()
    {
        var tempColor = image.color;
        tempColor.a = 0f;
        image.color = tempColor;
    }

    void OnTriggerEnter ()
    {
        FadingToWhite = true;
    }
	
	void Update () {
        if (FadingToWhite == true)
        {
            var tempColor = image.color;
            tempColor.a += 0.0028f;
            image.color = tempColor;
        }

        if (image.color.a >= 1)
        {
            StartCoroutine(CutsceneTime());
        }

    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Level3_v1");
    }
}
