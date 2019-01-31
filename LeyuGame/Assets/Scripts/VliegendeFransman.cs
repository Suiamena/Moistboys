using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VliegendeFransman : MonoBehaviour
{
	public float movementSpeed = 40, lookSensitivity = 1.5f;
	bool active = false;
	PlayerController playerController;
	Camera cam;
	Vector3 rotation;
	float sprintModifier = 2f, actualSpeed;

    //public GameObject player;
    //bool setPlayerRot = false;

	void Start ()
	{
		cam = GetComponent<Camera>();
		playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}
	
	void Update ()
	{
        //if (active) 
        //{
        //    if (setPlayerRot == false) 
        //    {
        //        player.transform.localRotation = Quaternion.Euler(player.transform.rotation.x, player.transform.eulerAngles.y, player.transform.rotation.z);
        //        setPlayerRot = true;
        //    }

        //}

        if (!active) {
			if (Input.GetButtonDown("Y Button")) {
				active = true;
                //playerController.DisablePlayer(true);
                playerController.enabled = false;
                cam.enabled = true;
			}
		} else {
			if (Input.GetButtonDown("Y Button")) {
				active = false;
                //playerController.EnablePlayer();
                playerController.enabled = true;
                playerController.launchEnabled = true;
                cam.enabled = false;
			}

			if (Input.GetButtonDown("X Button"))
				transform.position = playerController.transform.position;

			if (Input.GetButton("A Button"))
				actualSpeed = movementSpeed * sprintModifier;
			else
				actualSpeed = movementSpeed;

			transform.position += transform.rotation * new Vector3(
				Input.GetAxis("Left Stick X") * actualSpeed * Time.deltaTime,
				(Input.GetAxis("Right Trigger") - Input.GetAxis("Left Trigger")) * actualSpeed * Time.deltaTime,
				Input.GetAxis("Left Stick Y") * actualSpeed * Time.deltaTime);

			rotation.x += Input.GetAxis("Right Stick Y") * -lookSensitivity;
			rotation.y += Input.GetAxis("Right Stick X") * lookSensitivity;
			transform.rotation = Quaternion.Euler(rotation);
		}
	}
}
