using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_SnowWaggle : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	public Transform moustacheBoyStartLocation, moustacheBoyEndLocation;
	public AudioClip waggleClip;
	AudioSource audioSource;

	[Header("Animation Settings")]
	Vector3 endPosition;
	Quaternion endRotation;
	public float rotationSpeed = 260;
	public float movementSpeed = 5, swayAngle = 20, swayTime = .2f;

	public void Initialize (Action proceedToExecute)
	{
        endPosition = moustacheBoyEndLocation.position;
        endRotation = moustacheBoyEndLocation.rotation;
        moustacheBoy.position = moustacheBoyStartLocation.position;
		moustacheBoy.rotation = moustacheBoyStartLocation.rotation;
		audioSource = GetComponent<AudioSource>();

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

		yield return new WaitForSeconds(.4f);

		//ROTATE TOWARDS TARGET POSITION
		while (moustacheBoy.rotation != targetRotation) {
			moustacheBoy.rotation = Quaternion.RotateTowards(moustacheBoy.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			yield return null;
		}

		//MOVE TO FINAL POSITION
		moustacheBoy.GetComponent<Animator>().SetBool("isWiggling", true);
		audioSource.clip = waggleClip;
		audioSource.loop = true;
		audioSource.Play();		

		float t = 0;
		while (moustacheBoy.position.SquareDistance(endPosition) > .01f) {
			moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.position, endPosition, movementSpeed * Time.deltaTime);
			transform.position = moustacheBoy.position;
			t += Time.deltaTime;
			yield return null;
		}
		moustacheBoy.GetComponent<Animator>().SetBool("isWiggling", false);
		audioSource.Stop();

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