using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_SnowWaggle : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	public Transform moustacheBoiStartPosition;

	[Header("Animation Settings")]
	Vector3 endPosition;
	Quaternion endRotation;
	public float rotationSpeed = 100;
	public float movementSpeed = 3;

	//INITIAL BLOCK
	bool isPlayerInRange = false;
	public void Initialize (Action proceedToExecute)
	{
		endPosition = moustacheBoy.position;
		endRotation = moustacheBoy.rotation;
		moustacheBoy.position = moustacheBoiStartPosition.position;
		moustacheBoy.rotation = moustacheBoiStartPosition.rotation;

		StartCoroutine(WaitForPlayer(proceedToExecute));
	}
	IEnumerator WaitForPlayer (Action proceedToExecute)
	{
		while (!isPlayerInRange) {
			Debug.Log("Waiting for player");
			yield return null;
		}
		proceedToExecute();
	}
	public void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
			isPlayerInRange = true;
	}


	//EXECUTION BLOCK
	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(CreatureWaggle(proceedToEnd));
	}

	IEnumerator CreatureWaggle (Action proceedToEnd)
	{
		Quaternion targetRotation, oldRotation;
		oldRotation = moustacheBoy.rotation;
		moustacheBoy.LookAt(endPosition);
		targetRotation = moustacheBoy.rotation;
		moustacheBoy.rotation = oldRotation;

		Debug.Log("First Rot");
		//ROTATE TOWARDS TARGET POSITION
		while (moustacheBoy.rotation != targetRotation) {
			moustacheBoy.rotation = Quaternion.RotateTowards(moustacheBoy.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			yield return null;
		}

		Debug.Log("Moving");
		//MOVE TO FINAL POSITION
		while (moustacheBoy.position != endPosition) {
			moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.position, endPosition, movementSpeed * Time.deltaTime);
			yield return null;
		}

		Debug.Log("Second Rot");
		//ROTATE TO FINAL ROTATION
		while (moustacheBoy.rotation != endRotation) {
			moustacheBoy.rotation = Quaternion.RotateTowards(moustacheBoy.rotation, endRotation, rotationSpeed * Time.deltaTime);
			yield return null;
		}

		Debug.Log("Done");
		proceedToEnd();
	}


	//END BLOCK
	public void End (Action endEncounter)
	{
		endEncounter();
	}
}