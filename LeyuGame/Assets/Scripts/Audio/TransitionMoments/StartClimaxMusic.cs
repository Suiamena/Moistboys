using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartClimaxMusic : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Level4Music.musicStage = 10.5f;
        }
    }

}
