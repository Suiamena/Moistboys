using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Cia : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	public GameObject pressButtonPopup;
	GameObject player;

	[Header("Animation Settings")]
	public Vector3 flyInOutPoint = new Vector3(0, 20, -6);
	public float flyingSpeed = 35, timeBeforeDeparture = 3;
	Vector3 defaultCreaturePos;
	Quaternion defaultCreatureRot;

	//INITIAL BLOCK
	public void Initialize (Action proceedToExecute)
	{
		defaultCreaturePos = moustacheBoy.transform.position;
		defaultCreatureRot = moustacheBoy.transform.rotation;

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

		proceedToExecute();
	}


	//EXECUTION BLOCK
	public void Execute (Action proceedToEnd)
	{
		StartCoroutine(Wave(proceedToEnd));
	}

	IEnumerator Wave (Action proceedToEnd)
	{
		MoustacheBoiAudio.PlayScreeches();
		//PLAY ZWAAI ANIMATION

		float t = 0;
		while (Vector3.Distance(moustacheBoy.position, player.transform.position) < NewWallMechanic.triggerAbilityRange && t < timeBeforeDeparture) {
			moustacheBoy.LookAt(player.transform);
			t += Time.deltaTime;
			yield return null;
		}

		proceedToEnd();
	}
	bool IsPlayerInRange ()
	{
		if (Vector3.Distance(player.transform.position, moustacheBoy.position) < NewWallMechanic.triggerAbilityRange)
			return true;
		else
			return false;
	}


	//END BLOCK
	public void End (Action endEncounter)
	{
		endEncounter();
	}
}