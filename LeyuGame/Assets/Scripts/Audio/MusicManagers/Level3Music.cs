using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3Music : MonoBehaviour
{
    bool abilityGot;

    private void FixedUpdate()
    {
        if (!abilityGot)
        {
            ContinuePlayingAgain.musicStage += Mathf.Lerp(0f, 6.5f, 0.096f);
            ContinuePlayingAgain.musicStage = Mathf.Clamp(ContinuePlayingAgain.musicStage, 0, 6.5f);
        }
        if (ContinuePlayingAgain.musicStage == 6.5f && ContinuePlayingAgain.musicStage < 7.5f && !abilityGot)
        {
            abilityGot = true;
            StartCoroutine(PlayCompetentMusic());
        }
    }

    IEnumerator PlayCompetentMusic()
    {
        yield return new WaitForSeconds(3F);
        ContinuePlayingAgain.musicStage = 7.5f;
        //ContinuePlayingSound.musicStage += Mathf.Lerp(0f, 7.5f, 0.005f);
        //ContinuePlayingSound.musicStage = Mathf.Clamp(PlaySound.musicStage, 0, 7.5f);
    }

}
