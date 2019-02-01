using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicateChoice : MonoBehaviour
{
    GameObject player;
    PlayerController playerScript;

    public GameObject playerAureool;

    void Awake()
    {
        player = GameObject.Find("Character");
        playerScript = player.GetComponent<PlayerController>();
        if (VariablesGlobal.chosenForCompetence)
            playerScript.launchEnabled = true;
        else
            playerScript.launchEnabled = false;

        if (VariablesGlobal.chosenForSocial)
        {
            playerScript.creatureWallsEnabled = true;
            playerAureool.SetActive(false);
        }
        else
        {
            StartCoroutine(DisableCreature());
            playerAureool.SetActive(true);
        }
    }

    IEnumerator DisableCreature()
    {
        yield return new WaitForSeconds(3f);
        playerScript.creatureWallsEnabled = false;
    }

}