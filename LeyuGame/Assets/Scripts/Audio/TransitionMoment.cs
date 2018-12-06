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
            Destroy(gameObject);
        }
    }

}
