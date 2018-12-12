using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Superflop : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	GameObject player;

	public void Initialize (Action proceedToExecute)
	{
		player = GameObject.FindGameObjectWithTag("Player");

		proceedToExecute();
	}

	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(Sneeze(proceedToEnd));
	}
	IEnumerator Sneeze (Action proceedToEnd)
	{
		//SPEEL AUDIO

		//VERVANG MET ANIMATIE
		moustacheBoy.LookAt(player.transform);
		for (float t = 0; t < .3f; t += Time.deltaTime) {
			moustacheBoy.position += Vector3.up * 5 * Time.deltaTime;
			yield return null;
		}
		for (float t = 0; t < .3f; t += Time.deltaTime) {
			moustacheBoy.position -= Vector3.up * 5 * Time.deltaTime;
			yield return null;
		}
		proceedToEnd();
	}

	public void End (Action endEncounter)
	{
		endEncounter();
	}
}