using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelFour : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level4_rough");
        }
    }

}
