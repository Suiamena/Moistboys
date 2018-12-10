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
	public float rotationSpeed = 260;
	public float movementSpeed = 7, swayAngle = 20, swayTime = .2f;

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
		
		//ROTATE TOWARDS TARGET POSITION
		while (moustacheBoy.rotation != targetRotation) {
			moustacheBoy.rotation = Quaternion.RotateTowards(moustacheBoy.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			yield return null;
		}
		
		//MOVE TO FINAL POSITION
		float t = 0;
		while (moustacheBoy.position != endPosition) {
			moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.position, endPosition, movementSpeed * Time.deltaTime);
			moustacheBoy.rotation = Quaternion.Euler(new Vector3(0, moustacheBoy.eulerAngles.y, Mathf.Sin(t / swayTime) * swayAngle));
			t += Time.deltaTime;
			yield return null;
		}
		
		//ROTATE TO FINAL ROTATION
		while (moustacheBoy.rotation != endRotation) {
			moustacheBoy.rotation = Quaternion.RotateTowards(moustacheBoy.rotation, endRotation, rotationSpeed * Time.deltaTime);
			yield return null;
		}
		
		proceedToEnd();
	}


	//END BLOCK
	public void End (Action endEncounter)
	{
		endEncounter();
	}
}