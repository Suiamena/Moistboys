using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSnowOn : MonoBehaviour
{

    public GameObject snowParticles;

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            snowParticles.SetActive(true);
        }
    }

}
