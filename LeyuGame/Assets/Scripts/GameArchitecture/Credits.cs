using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Credits : MonoBehaviour {

    private void Update()
    {
        if (Input.GetButtonDown("A Button") || Input.GetButton("Left Mouse Button"))
        {
            StartCoroutine(LoadTitleScreen());
        }
    }

    IEnumerator LoadTitleScreen()
    {
        yield return new WaitForSeconds(0f);
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        CreditsMusic.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("TitleScreen");
    }

}
