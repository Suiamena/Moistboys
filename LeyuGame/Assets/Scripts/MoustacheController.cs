using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoustacheController : MonoBehaviour
{
	Vector3 inputs;

	public float movementSpeed = 14, rotationSpeed = 30;

	void Update ()
	{
		inputs = new Vector3(Input.GetKey(KeyCode.D).ToInt() + Input.GetKey(KeyCode.A).ToInt() * -1,
			Input.GetKey(KeyCode.E).ToInt() + Input.GetKey(KeyCode.Q).ToInt() * -1,
			Input.GetKey(KeyCode.W).ToInt() + Input.GetKey(KeyCode.S).ToInt() * -1);
		transform.Translate(inputs * movementSpeed * Time.deltaTime);
		transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime, 0));
	}
}
