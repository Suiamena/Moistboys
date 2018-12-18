using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommunicateChoice : MonoBehaviour
{
    public GameObject player;
    PlayerController playerScript;
    public GameObject walls;

    private void Awake()
    {
        Debug.Log("scri[t");
        playerScript = player.GetComponent<PlayerController>();
        if (VariablesGlobal.chosenForCompetence)
        {
            Debug.Log("COMPETENCE");
            playerScript.launchEnabled = true;
        }

        if (VariablesGlobal.chosenForSocial)
        {
            Debug.Log("SOCIAL");
            walls.SetActive(true);
        }
    }

    private void Update()
    {
        if (VariablesGlobal.chosenForSocial)
        {
            playerScript.launchEnabled = false;
        }
    }

}
