using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadCredits : MonoBehaviour
{

    private void FixedUpdate()
    {
        StartCoroutine(StartCredits());
    }

    IEnumerator StartCredits()
    {
        yield return new WaitForSeconds(3F);
        SceneManager.LoadScene("Credits_rough");
    }

}
