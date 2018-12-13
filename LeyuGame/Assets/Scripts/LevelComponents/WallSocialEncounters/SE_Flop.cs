using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Flop : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	Animator moustacheAnimator;
	GameObject player;

	public void Initialize (Action proceedToExecute)
	{
		player = GameObject.FindGameObjectWithTag("Player");
		moustacheAnimator = moustacheBoy.GetComponent<Animator>();

		proceedToExecute();
	}
	
	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(Sneeze(proceedToEnd));
	}
	IEnumerator Sneeze (Action proceedToEnd)
	{
		//SPEEL AUDIO

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