﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

    [Header("UI Objects")]
    public GameObject buttons;
    public GameObject instruction;
    public GameObject pointer;
    public GameObject startPointer;
    public GameObject controlsPointer;
    public GameObject quitPointer;
    public GameObject controlsInstruction;

    int iconPosition;

    bool menuStarted, titleScreenDone, controlsOpened;

    public GameObject level;

    void Update ()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 10);

        if (!menuStarted)
        {
            if (Input.anyKey)
            {
                if (!titleScreenDone)
                {
                    buttons.SetActive(true);
                    instruction.SetActive(false);
                    menuStarted = true;
                    titleScreenDone = true;
                    StartCoroutine(LoadNextMenuDelay());
                }
            }
        }

        if (menuStarted)
        {
            RunMenu();
        }

        if (controlsOpened)
        {
            BackToMenu();
        }
	}

    void RunMenu()
    {
        if (Input.GetAxis("Left Stick Y") > 0 || Input.GetAxis("Keyboard WS") > 0)
        {
            iconPosition += 1;
        }
        if (Input.GetAxis("Left Stick Y") < 0 || Input.GetAxis("Keyboard WS") < 0)
        {
            iconPosition -= 1;
        }

        if (Input.GetAxis("Left Stick Y") > 0 || Input.GetAxis("Keyboard WS") > 0)
        {
            if (iconPosition != 1)
            {
                pointer.transform.position = startPointer.transform.position;
                iconPosition = 1;
            }
        }

        if (Input.GetAxis("Left Stick Y") < 0 || Input.GetAxis("Keyboard WS") < 0)
        {
            if (iconPosition != 2)
            {
                pointer.transform.position = controlsPointer.transform.position;
                iconPosition = 2;
            }
        }

        if (Input.GetAxis("Left Stick Y") < 0 || Input.GetAxis("Keyboard WS") < 0)
        {
            if (iconPosition != 2)
            {
                pointer.transform.position = controlsPointer.transform.position;
                iconPosition = 3;
            }
        }

        if (Input.GetButtonDown("A Button") || Input.GetButtonDown("Keyboard Space"))
        {
            if (iconPosition == 1)
            {
                SceneManager.LoadScene("Level 1");
            }
            if (iconPosition == 2)
            { 
                menuStarted = false;
                controlsInstruction.SetActive(true);
                buttons.SetActive(false);
                StartCoroutine(BackToMenuDelay());
            }
        }

    }

    void BackToMenu()
    {
        if (Input.GetButtonDown("A Button") || Input.GetButtonDown("Keyboard Space"))
        {
            controlsInstruction.SetActive(false);
            controlsOpened = false;
            buttons.SetActive(true);
            menuStarted = true;
        }
    }

    IEnumerator LoadNextMenuDelay()
    {
        yield return new WaitForSeconds(0.1f);
        iconPosition = 1;
    }

    IEnumerator BackToMenuDelay()
    {
        yield return new WaitForSeconds(0.1f);
        controlsOpened = true;
    }

}
