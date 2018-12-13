using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Superflop : MonoBehaviour, ISocialEncounter
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
		moustacheBoy.LookAt(player.transform);

		MoustacheBoiAudio.PlayScreeches();
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