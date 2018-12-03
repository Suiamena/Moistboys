using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWallMechanic : MonoBehaviour {

    //PLAYER
    GameObject player;

    //CREATURE
    GameObject moustacheBoi;

    //UI
    public GameObject pressButtonPopup;

    [Header("Platform Settings")]
    GameObject platformsObject;
    List<GameObject> platforms = new List<GameObject>();

    [Header("Settings")]
    public int triggerAbilityRange = 10;

    private void Awake()
    {
        player = GameObject.Find("Character");
        moustacheBoi = GameObject.Find("Creature");
        //platformsObject = Resources.FindObjectsOfTypeAll < GameObject.Find("platforms") >;
    }

    private void FixedUpdate()
    {
        CutsceneSetup();
        StartSequence();
    }

    void CutsceneSetup()
    {
        //FOLLOW THE PLAYER
        if (Vector3.Distance(moustacheBoi.transform.position, player.transform.position) < triggerAbilityRange)
        {
            pressButtonPopup.SetActive(true);
        }
        else
        {
            pressButtonPopup.SetActive(false);
        }
    }

    void StartSequence()
    {
        if (Input.GetButtonDown("A Button"))
        {
            platformsObject.SetActive(true);
        }
    }

}
