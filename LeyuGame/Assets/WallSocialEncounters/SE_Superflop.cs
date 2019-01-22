using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Superflop : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	AudioSource audioSource;
	public AudioClip superFlopClip;
	Animator moustacheAnimator;
	GameObject player;

	public void Initialize (Action proceedToExecute)
	{
		player = GameObject.FindGameObjectWithTag("Player");
		moustacheAnimator = moustacheBoy.GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();

		proceedToExecute();
	}

	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(SuperFlop(proceedToEnd));
	}
	IEnumerator SuperFlop (Action proceedToEnd)
	{
		moustacheBoy.LookAt(player.transform);

		transform.position = moustacheBoy.position;
		audioSource.clip = superFlopClip;
		audioSource.Play();
		moustacheAnimator.SetBool("isSuperFlop", true);
		yield return new WaitForSeconds(1.5f);

		moustacheAnimator.SetBool("isSuperFlop", false);
		proceedToEnd();
	}

	public void End (Action endEncounter)
	{
		endEncounter();
	}
}