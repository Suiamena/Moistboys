using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReuniteCreatureSound : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Level3Music.musicStage = 8.5f;
            Destroy(gameObject);
        }
    }

}
