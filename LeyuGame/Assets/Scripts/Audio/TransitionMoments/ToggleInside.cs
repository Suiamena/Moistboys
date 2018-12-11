using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToggleInside : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            AmbienceManager.ToggleAmbience();
        }
    }

}
