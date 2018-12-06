using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionMoment : MonoBehaviour
{

    GameObject managerObject;
    MusicManager managerScript;

    private void Awake()
    {
        managerObject = GameObject.Find("NewMusicManager");
        managerScript = managerObject.GetComponent<MusicManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            managerScript.countMusicStage += 1;
            Destroy(gameObject);
        }
    }

}
