using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchParticleDestroy : MonoBehaviour {

    private void Awake()
    {
        Destroy(gameObject, 1);
    }
}
