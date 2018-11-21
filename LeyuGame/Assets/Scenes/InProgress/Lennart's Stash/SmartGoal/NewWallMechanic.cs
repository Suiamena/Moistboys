using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour {

    [Header("Player Settings")]
    GameObject player;

    [Header("Other Settings")]
    GameObject moustacheBoi;
    public int creatureRange = 10;

    private void Awake()
    {
        player = GameObject.Find("Character");
        moustacheBoi = GameObject.Find("MOD_MoustacheBoi");
    }

    private void FixedUpdate()
    {
        CutsceneSetup();
    }

    void CutsceneSetup()
    {
        //FOLLOW THE PLAYER
        if (Vector3.Distance(moustacheBoi.transform.position, player.transform.position) < creatureRange)
        {
        }
    }

}
