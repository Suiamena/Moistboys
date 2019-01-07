﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialChoice : MonoBehaviour
{

    bool playerCanMakeChoice;
    public bool playerChooseSocial;

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
            if (playerChooseSocial)
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
        if (Input.GetButtonDown("A Button") || Input.GetButtonDown("Keyboard Space"))
        {
            Level5Music.musicStage = 11.5f;
            playerChooseSocial = true;
            choiceMessage.SetActive(false);
        }
    }

}
