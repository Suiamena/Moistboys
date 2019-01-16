using System.Collections;
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

    bool menuStarted, titleScreenDone, controlsOpened, stickPushed, keyboardPressed;

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
        if (Input.GetAxis("Left Stick Y") == 0)
        {
            stickPushed = false;
        }
        if (Input.GetAxis("Keyboard WS") == 0)
        {
            keyboardPressed = false;
        }
        //INPUT
        if (stickPushed || keyboardPressed)
        {
        }
        else
        {
            if (Input.GetAxis("Left Stick Y") > 0 || Input.GetAxis("Keyboard WS") > 0)
            {
                if (iconPosition == 1)
                {
                    iconPosition = 3;
                    pointer.transform.position = quitPointer.transform.position;
                }
                else
                {
                    if (iconPosition == 3)
                    {
                        iconPosition = 2;
                        pointer.transform.position = controlsPointer.transform.position;
                    }
                    else
                    {
                        if (iconPosition == 2)
                        {
                            iconPosition = 1;
                            pointer.transform.position = startPointer.transform.position;
                        }
                    }
                }
            }

            if (Input.GetAxis("Left Stick Y") < 0 || Input.GetAxis("Keyboard WS") < 0)
            {
                if (iconPosition == 1)
                {
                    iconPosition = 2;
                    pointer.transform.position = controlsPointer.transform.position;
                }
                else
                {
                    if (iconPosition == 2)
                    {
                        iconPosition = 3;
                        pointer.transform.position = quitPointer.transform.position;
                    }
                    else
                    {
                        if (iconPosition == 3)
                        {
                            iconPosition = 1;
                            pointer.transform.position = startPointer.transform.position;
                        }
                    }
                }
            }
            if (Input.GetAxis("Left Stick Y") != 0)
            {
                stickPushed = true;
            }
            if (Input.GetAxis("Keyboard WS") != 0)
            {
                keyboardPressed = true;
            }
        }

        //OPTIONS
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
            if (iconPosition == 3)
            {
                Application.Quit();
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