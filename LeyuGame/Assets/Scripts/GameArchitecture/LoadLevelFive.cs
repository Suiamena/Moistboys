using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevelFive : MonoBehaviour
{
    public Image fadeOutImage;
    public Color fadeColor;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        fadeColor.a = 0;
        fadeOutImage.color = fadeColor;
        fadeOutImage.enabled = true;

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            fadeOutImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, t);
            yield return null;
        }
        EndLevel();
    }

    void EndLevel()
    {
        AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("Level 5");
    }
}
