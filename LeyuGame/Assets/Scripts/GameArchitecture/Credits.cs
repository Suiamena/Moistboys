using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Credits : MonoBehaviour {

    private void Awake()
    {
        StartCoroutine(LoadTitleScreen());
    }

    IEnumerator LoadTitleScreen()
    {
        yield return new WaitForSeconds(5f);
        Debug.Log("load");
        SceneManager.LoadScene("TitleScreen");
    }

}
