using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Music : MonoBehaviour
{
    bool abilityGot;

    private void FixedUpdate()
    {
        Debug.Log(PlaySound.musicStage);
        if (!abilityGot)
        {
            PlaySound.musicStage = 6.5f;
            abilityGot = true;
            StartCoroutine(PlayCompetentMusic());
            //PlaySound.musicStage += Mathf.Lerp(0f, 6.5f, 0.096f);
            //PlaySound.musicStage = Mathf.Clamp(PlaySound.musicStage, 0, 6.5f);
        }
        //if (PlaySound.musicStage == 6.5f && PlaySound.musicStage < 7.5f)
        //{
        //    abilityGot = true;
        //    StartCoroutine(PlayCompetentMusic());
        //}
    }

    IEnumerator PlayCompetentMusic()
    {
        yield return new WaitForSeconds(3F);
        PlaySound.musicStage = 7.5f;
        PlaySound.musicStage += Mathf.Lerp(0f, 7.5f, 0.005f);
        PlaySound.musicStage = Mathf.Clamp(PlaySound.musicStage, 0, 7.5f);
    }

}
