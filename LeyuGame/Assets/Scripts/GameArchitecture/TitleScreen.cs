using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour {

    [Header("UI Objects")]
    public GameObject buttons;
    public GameObject instruction;
    public GameObject icon;

    GameObject positionUp;
    GameObject positionDown;

    int iconPosition;

    bool menuStarted;

    private void Awake()
    {
        positionUp = GameObject.Find("IconTransformUp");
        positionDown = GameObject.Find("IconTransformDown");
    }

    void Update ()
    {
        if (!menuStarted)
        {
            if (Input.anyKey)
            {
                buttons.SetActive(true);
                instruction.SetActive(false);
                menuStarted = true;
            }
        }

        if (menuStarted)
        {
            RunMenu();
        }
	}

    void RunMenu()
    {

        if (Input.GetAxis("Left Stick Y") > 0)
        {
            if (iconPosition == 2)
            {
                icon.transform.position = positionUp.transform.position;
                iconPosition = 1;
            }
        }

        if (Input.GetAxis("Left Stick Y") < 0)
        {
            if (iconPosition == 1)
            {
                icon.transform.position = positionDown.transform.position;
                iconPosition = 2;
            }
        }

    }

}
