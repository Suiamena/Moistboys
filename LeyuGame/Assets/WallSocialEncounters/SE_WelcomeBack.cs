using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_WelcomeBack : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	AudioSource audioSource;
	public AudioClip welcomeBackClip;
	Animator moustacheAnimator;
	GameObject player;

	[Header("Animation Settings")]
	public float timeBeforeAdvancing = 3;

	//INITIAL BLOCK
	public void Initialize (Action proceedToExecute)
	{
		player = GameObject.FindGameObjectWithTag("Player");
		moustacheAnimator = moustacheBoy.GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		proceedToExecute();
	}


	//EXECUTION BLOCK
	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(Wave(proceedToEnd));
	}

	IEnumerator Wave (Action proceedToEnd)
	{
		audioSource.clip = welcomeBackClip;
		audioSource.Play();
		moustacheAnimator.SetBool("goodBye", true);

		float t = 0;
		while (!Input.GetButtonDown("A Button") && t < timeBeforeAdvancing) {
			moustacheBoy.LookAt(player.transform);
			moustacheBoy.Rotate(new Vector3(-moustacheBoy.transform.eulerAngles.x, 0, -moustacheBoy.transform.eulerAngles.z));
			t += Time.deltaTime;
			yield return null;
		}
		moustacheAnimator.SetBool("goodBye", false);

		proceedToEnd();
	}


	//END BLOCK
	public void End (Action endEncounter)
	{
		endEncounter();
	}
}