using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Flop : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	AudioSource audioSource;
	public AudioClip flopClip;
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
		StartCoroutine(Flop(proceedToEnd));
	}
	IEnumerator Flop (Action proceedToEnd)
	{
		transform.position = moustacheBoy.position;
		audioSource.Play();

		moustacheAnimator.SetBool("isFlop", true);

		yield return new WaitForSeconds(1);

		moustacheAnimator.SetBool("isFlop", false);
		
		proceedToEnd();
	}
	
	public void End (Action endEncounter)
	{
		endEncounter();
	}
}