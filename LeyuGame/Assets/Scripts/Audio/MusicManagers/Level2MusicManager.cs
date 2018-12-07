using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2MusicManager : MonoBehaviour
{

    private void FixedUpdate()
    {
        RegulateMusic();
    }

    void RegulateMusic()
    {
        StartCoroutine(DoMusic());
        //PlaySound.musicStage += Mathf.Lerp(0f, 4.5f, 0.005f);
        //PlaySound.musicStage = Mathf.Clamp(PlaySound.musicStage, 0, 4.5f);
        //PlaySound.musicStage = 4.5f;
    }

    IEnumerator DoMusic()
    {
        PlaySound.musicStage = 3.5f;
        yield return new WaitForSeconds(3F);
        PlaySound.musicStage = 4.5f;
    }

}
