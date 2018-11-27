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
        managerObject = GameObject.Find("MusicManager");
        managerScript = managerObject.GetComponent<MusicManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            managerScript.musicStage += 1;
            Destroy(gameObject);
        }
    }

}
