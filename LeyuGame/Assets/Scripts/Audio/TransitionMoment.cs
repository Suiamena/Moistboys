using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionMoment : MonoBehaviour
{

    GameObject managerObject;
    Level1Music managerScript;

    private void Awake()
    {
        managerObject = GameObject.Find("Level1MusicManager");
        managerScript = managerObject.GetComponent<Level1Music>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            managerScript.countMusicStage += 1;
            //StartCoroutine(TestMusic());
            Destroy(gameObject);
        }
    }

    IEnumerator TestMusic()
    {
        PlaySound.musicStage = 2.5f;
        yield return new WaitForSeconds(3f);
        PlaySound.musicStage = 3.5f;
        yield return new WaitForSeconds(3f);
        PlaySound.musicStage = 4.5f;
        yield return new WaitForSeconds(3f);
        PlaySound.musicStage = 5.5f;
        yield return new WaitForSeconds(3f);
        PlaySound.musicStage = 6.5f;
        yield return new WaitForSeconds(3f);
        PlaySound.musicStage = 7.5f;
        yield return new WaitForSeconds(3f);
    }

}
