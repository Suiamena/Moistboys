using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDispensableBoi : MonoBehaviour
{

    public GameObject dispensableBoi;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(dispensableBoi);
            print("u useless bye");
        }
    }
}
