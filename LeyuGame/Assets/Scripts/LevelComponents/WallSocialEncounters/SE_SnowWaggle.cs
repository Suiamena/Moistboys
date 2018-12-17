using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_SnowWaggle : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	public Transform moustacheBoyStartLocation, moustacheBoyEndLocation;

	[Header("Animation Settings")]
	Vector3 endPosition;
	Quaternion endRotation;
	public float rotationSpeed = 260;
	public float movementSpeed = 7, swayAngle = 20, swayTime = .2f;
	
	public void Initialize (Action proceedToExecute)
	{
		endPosition = moustacheBoyEndLocation.position;
		endRotation = moustacheBoyEndLocation.rotation;
		moustacheBoy.position = moustacheBoyStartLocation.position;
		moustacheBoy.rotation = moustacheBoyStartLocation.rotation;

		proceedToExecute();
	}
	
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
		while (Vector3.Distance(moustacheBoy.position, endPosition) > .1f) {
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

	public void End (Action endEncounter)
	{
		endEncounter();
	}
}