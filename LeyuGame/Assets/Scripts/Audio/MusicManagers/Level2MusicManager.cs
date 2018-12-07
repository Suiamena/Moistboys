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
        PlaySound.musicStage += Mathf.Lerp(0f, 4.5f, 0.005f);
        PlaySound.musicStage = Mathf.Clamp(PlaySound.musicStage, 0, 4.5f);
    }

}
