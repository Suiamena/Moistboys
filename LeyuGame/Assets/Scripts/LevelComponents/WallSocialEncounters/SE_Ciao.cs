using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Ciao : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	GameObject player;

	[Header("Animation Settings")]
	public Vector3 flyInOutPoint = new Vector3(0, 20, -6);
	public float flyingSpeed = 35, timeBeforeDeparture = 3;
	Vector3 defaultCreaturePos;
	Quaternion defaultCreatureRot;

	//INITIAL BLOCK
	public void Initialize (Action proceedToExecute)
	{
		Debug.Log("Initialize");
		defaultCreaturePos = moustacheBoy.transform.position;
		defaultCreatureRot = moustacheBoy.transform.rotation;
		moustacheBoy.gameObject.SetActive(false);

		player = GameObject.FindGameObjectWithTag("Player");

		StartCoroutine(FlyIn(proceedToExecute));
	}
	IEnumerator FlyIn (Action proceedToExecute)
	{
		moustacheBoy.position = defaultCreaturePos + defaultCreatureRot * flyInOutPoint;
		moustacheBoy.LookAt(defaultCreaturePos);
		moustacheBoy.Rotate(new Vector3(-moustacheBoy.transform.eulerAngles.x, 0, -moustacheBoy.transform.eulerAngles.z));
		moustacheBoy.gameObject.SetActive(true);
		MoustacheBoiAudio.PlayFlaps();

		while (Vector3.Distance(moustacheBoy.transform.position, defaultCreaturePos) > .1f) {
			moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.transform.position, defaultCreaturePos, flyingSpeed * Time.deltaTime);
			yield return null;
		}
		moustacheBoy.position = defaultCreaturePos;

		proceedToExecute();
	}


	//EXECUTION BLOCK
	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(Wave(proceedToEnd));
		Debug.Log("Execute");
	}

	IEnumerator Wave (Action proceedToEnd)
	{
		MoustacheBoiAudio.PlayScreeches();
		//PLAY ZWAAI ANIMATION

		float t = 0;
		while (Vector3.Distance(moustacheBoy.position, player.transform.position) < NewWallMechanic.triggerAbilityRange && t < timeBeforeDeparture) {
			moustacheBoy.LookAt(player.transform);
			moustacheBoy.Rotate(new Vector3(-moustacheBoy.transform.eulerAngles.x, 0, -moustacheBoy.transform.eulerAngles.z));
			t += Time.deltaTime;
			yield return null;
		}
		MoustacheBoiAudio.StopFlaps();

		proceedToEnd();
	}


	//END BLOCK
	public void End (Action endEncounter)
	{
		Debug.Log("End");
		StartCoroutine(FlyAway(endEncounter));
		endEncounter();
	}
	IEnumerator FlyAway (Action endEncounter)
	{
		moustacheBoy.LookAt(flyInOutPoint);
		moustacheBoy.Rotate(new Vector3(-moustacheBoy.transform.eulerAngles.x, 0, -moustacheBoy.transform.eulerAngles.z));
		//LIFT ANIMATIE
		for (float t = 0; t < 0.6f; t += Time.deltaTime) {
			yield return null;
		}
		//FLY ANIMATIE
		MoustacheBoiAudio.PlayFlaps();

		while (Vector3.Distance(moustacheBoy.transform.position, defaultCreaturePos + defaultCreatureRot * flyInOutPoint) > .1f) {
			moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.transform.position, defaultCreaturePos + defaultCreatureRot * flyInOutPoint, flyingSpeed * Time.deltaTime);
			yield return null;
		}
		moustacheBoy.gameObject.SetActive(false);
		MoustacheBoiAudio.StopFlaps();

		endEncounter();
	}
}