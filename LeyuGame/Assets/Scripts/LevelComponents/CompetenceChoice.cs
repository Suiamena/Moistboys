using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceChoice : MonoBehaviour
{

    bool playerCanMakeChoice;
    public bool playerChooseCompetence;

    public GameObject choiceMessage;

    private void Update()
    {
        if (playerCanMakeChoice)
        {
            MakeDecision();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (playerChooseCompetence)
            {
                choiceMessage.SetActive(false);
            }
            else
            {
                choiceMessage.SetActive(true);
                playerCanMakeChoice = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            choiceMessage.SetActive(false);
            playerCanMakeChoice = false;
        }
    }

    void MakeDecision()
    {
        if (Input.GetButtonDown("A Button"))
        {
            playerChooseCompetence = true;
            choiceMessage.SetActive(false);
        }
    }

}
