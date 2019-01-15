using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureWallParticle : MonoBehaviour {

    private void Awake()
    {
        Destroy(gameObject, 1);
    }

}
