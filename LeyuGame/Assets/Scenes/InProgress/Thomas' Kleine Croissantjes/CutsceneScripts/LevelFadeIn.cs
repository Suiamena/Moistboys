using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFadeIn : MonoBehaviour {

    public Image image;
    bool fadingToBlack = false;

    Color tempColor;

    void Start ()
    {
        StartCoroutine(CutsceneTime());
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (fadingToBlack == true)
        {
            var tempColor = image.color;
            tempColor.a -= 0.015f;
            image.color = tempColor;
        }
    }

    IEnumerator CutsceneTime()
    {
        yield return new WaitForSeconds(0.5f);
        fadingToBlack = true;
        //Destroy(gameObject);
    }

}
