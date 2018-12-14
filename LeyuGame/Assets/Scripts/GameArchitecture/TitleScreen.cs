using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

    [Header("UI Objects")]
    public GameObject buttons;
    public GameObject instruction;
    public GameObject pointer;
    public GameObject positionUp;
    public GameObject positionDown;
    public GameObject controlsInstruction;

    int iconPosition;

    bool menuStarted, titleScreenDone;

    private void Awake()
    {
        iconPosition = 1;
    }

    void Update ()
    {
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
                }
            }
        }

        if (menuStarted)
        {
            RunMenu();
        }
	}

    void RunMenu()
    {
        Debug.Log(menuStarted);
        if (Input.GetAxis("Left Stick Y") > 0)
        {
            if (iconPosition != 1)
            {
                pointer.transform.position = positionUp.transform.position;
                iconPosition = 1;
            }
        }

        if (Input.GetAxis("Left Stick Y") < 0)
        {
            if (iconPosition != 2)
            {
                pointer.transform.position = positionDown.transform.position;
                iconPosition = 2;
            }
        }

        if (Input.GetButtonDown("A Button"))
        {
            if (iconPosition == 1)
            {
                Debug.Log("next scene");
            }
            if (iconPosition == 2)
            { 
                menuStarted = false;
                controlsInstruction.SetActive(true);
                buttons.SetActive(false);
            }
        }

    }

}
