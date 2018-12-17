﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelTwo : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level 2");
        }
    }

}