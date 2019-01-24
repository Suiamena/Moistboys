using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
	enum ActiveScreen { Pause, Controls, Exit };
	ActiveScreen activeScreen = ActiveScreen.Pause;

	bool gamePaused = false;

	public GameObject pauseScreen, controlsScreen, exitScreen;
	PlayerController playerController;
	bool shouldPlayerBeEnabled = true;

	int optionSelected = 0;
	public GameObject[] pauseOptionSelectors = new GameObject[3];
	bool waitingForLeftStickReset = false, waitingForDPadReset = false;
	float directionInputDeadzone = .4f;

	public GameObject[] exitOptionSelectors = new GameObject[3];
	int exitOptionSelected = 0;

	private void Awake ()
	{
		playerController = GetComponent<PlayerController>();
	}

	void Update ()
	{
        if (!gamePaused) {
            if (Input.GetButtonDown("Start Button") || Input.GetKeyDown("escape")) {
				ActivatePause();
			}
		} else {
            switch (activeScreen) {
				case ActiveScreen.Pause:
                    if (Input.GetButtonDown("B Button") || Input.GetKeyDown("escape")) {
						DeactivatePause();
					}
					if (Input.GetButtonDown("A Button") || Input.GetButtonDown("Start Button") || Input.GetButtonDown("Keyboard Space")) {
						switch (optionSelected) {
							case 0:
								DeactivatePause();
								break;
							case 1:
								ActivateControlsScreen();
								break;
							case 2:
								ActivateExitScreen();
								break;
						}
					}
					if ((Mathf.Abs(Input.GetAxis("Left Stick Y")) > directionInputDeadzone && !waitingForLeftStickReset)) {
						SwitchPauseOption(Input.GetAxis("Left Stick Y"));
						waitingForLeftStickReset = true;
					}
					if (Input.GetAxis("DPad Y") != 0 && !waitingForDPadReset) {
						SwitchPauseOption(Input.GetAxis("DPad Y"));
						waitingForDPadReset = true;
					}
                    if (Input.GetButtonDown("W"))
                    {
                        SwitchPauseOption(1);
                    }
                    if (Input.GetButtonDown("S"))
                    {
                        SwitchPauseOption(-1);
                    }
                    break;
				case ActiveScreen.Controls:
					if (Input.GetButtonDown("A Button") || Input.GetButtonDown("B Button") || Input.GetButtonDown("Start Button") || Input.GetButtonDown("Keyboard Space"))
                    {
						DeactivateControlsScreen();
					}
					break;
				case ActiveScreen.Exit:
                    if ((Mathf.Abs(Input.GetAxis("Left Stick Y")) > directionInputDeadzone && !waitingForLeftStickReset))
                    {
                        SwitchExitOption(Input.GetAxis("Left Stick Y"));
						waitingForLeftStickReset = true;
					}
					if (Input.GetAxis("DPad Y") != 0 && !waitingForDPadReset) {
						SwitchExitOption(Input.GetAxis("DPad Y"));
						waitingForDPadReset = true;
					}
                    if (Input.GetButtonDown("W"))
                    {
                        SwitchExitOption(1);
                    }
                    if (Input.GetButtonDown("S"))
                    {
                        SwitchExitOption(-1);
                    }
                    if (Input.GetButtonDown("A Button") || Input.GetButtonDown("Start Button") || Input.GetButtonDown("Keyboard Space"))
                    {
						if (exitOptionSelected == 0) {
							DeactivateExitScreen();
						} else if (exitOptionSelected == 1) {
							Time.timeScale = 1;
							AmbienceManager.Ambience.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
							Level6Music.Music.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
							SceneManager.LoadScene("TitleScreen");
						} else {
							Application.Quit();
						}
					}
					if (Input.GetButtonDown("B Button") || Input.GetKeyDown("escape"))
						DeactivateExitScreen();
					break;
			}
		}

		if (Input.GetAxis("DPad Y") == 0)
			waitingForDPadReset = false;
		if ((Mathf.Abs(Input.GetAxis("Left Stick Y")) < directionInputDeadzone))
			waitingForLeftStickReset = false;
	}

	void ActivatePause ()
	{
		foreach (GameObject g in pauseOptionSelectors)
			g.SetActive(false);
		pauseOptionSelectors[optionSelected].SetActive(true);

		waitingForDPadReset = waitingForLeftStickReset = true;

		Time.timeScale = 0;
		if (playerController.enabled == true) {
			playerController.enabled = false;
			shouldPlayerBeEnabled = true;
		} else {
			shouldPlayerBeEnabled = false;
		}

		pauseScreen.SetActive(true);
		activeScreen = ActiveScreen.Pause;
		gamePaused = true;
	}
	void DeactivatePause ()
	{
		Time.timeScale = 1;
		if (shouldPlayerBeEnabled)
			playerController.enabled = true;

		optionSelected = 0;

		pauseScreen.SetActive(false);
		gamePaused = false;
	}

	void ActivateControlsScreen ()
	{
		pauseScreen.SetActive(false);
		controlsScreen.SetActive(true);
		activeScreen = ActiveScreen.Controls;
	}

	void DeactivateControlsScreen ()
	{
		pauseScreen.SetActive(true);
		controlsScreen.SetActive(false);
		activeScreen = ActiveScreen.Pause;
	}

	void ActivateExitScreen ()
	{
		foreach (GameObject g in exitOptionSelectors)
			g.SetActive(false);
		exitOptionSelectors[0].SetActive(true);

		pauseScreen.SetActive(false);
		exitScreen.SetActive(true);
		exitOptionSelected = 0;
		activeScreen = ActiveScreen.Exit;
	}

	void DeactivateExitScreen ()
	{
		pauseScreen.SetActive(true);
		exitScreen.SetActive(false);
		activeScreen = ActiveScreen.Pause;
	}

	void SwitchPauseOption (float input)
	{
		pauseOptionSelectors[optionSelected].SetActive(false);

		int direction;
		if (input < 0)
			direction = 1;
		else
			direction = -1;

		optionSelected += direction;

		if (optionSelected < 0)
			optionSelected = pauseOptionSelectors.Length - 1;
		if (optionSelected > pauseOptionSelectors.Length - 1)
			optionSelected = 0;

		pauseOptionSelectors[optionSelected].SetActive(true);
	}

	void SwitchExitOption (float input)
	{
		exitOptionSelectors[exitOptionSelected].SetActive(false);

		int direction;
		if (input < 0)
			direction = 1;
		else
			direction = -1;

		exitOptionSelected += direction;

		if (exitOptionSelected < 0)
			exitOptionSelected = exitOptionSelectors.Length - 1;
		if (exitOptionSelected > exitOptionSelectors.Length - 1)
			exitOptionSelected = 0;

		exitOptionSelectors[exitOptionSelected].SetActive(true);
	}
}