using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFourPlaceholder : MonoBehaviour
{

    private void FixedUpdate()
    {
        StartCoroutine(LoadLevelFive());
    }

    IEnumerator LoadLevelFive()
    {
        yield return new WaitForSeconds(3F);
        SceneManager.LoadScene("Level5_rough");
    }

}
