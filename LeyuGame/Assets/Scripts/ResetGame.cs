using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ResetGame : MonoBehaviour {

	void Update ()
    {
        if (Input.GetKeyDown("1"))
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level5Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level 1");
        }

        if (Input.GetKeyDown("2"))
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level5Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level 2");
        }

        if (Input.GetKeyDown("3"))
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level5Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level 3");
        }

        if (Input.GetKeyDown("4"))
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level5Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("level 4");
        }

        if (Input.GetKeyDown("5"))
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level5Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("Level 6");
        }

        if (Input.GetKeyDown("6"))
        {
            AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level1Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level2Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level3Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level4Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level5Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
