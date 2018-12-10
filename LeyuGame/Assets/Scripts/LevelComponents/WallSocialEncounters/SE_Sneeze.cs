using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SE_Sneeze : MonoBehaviour, ISocialEncounter
{
	public Transform moustacheBoy;
	public GameObject pressButtonPopup;
	GameObject player;

	[Header("Animation Settings")]
	public float shiversPerSecond = 6;
	float timePerShiver;
	public float shiverYAngle = 12, shiverXAngle = 8, timeBeforeSneeze = 3;
	Quaternion defaultRot;
	bool sneezed = false;
	public AudioClip sneezeClip, blessYouClip, thankYouClip;
	AudioSource audioSource;

	public float sneezeBackRotation = -15, sneezeFrontRotation = 20, sneezeBackTime = .2f, sneezeFrontTime = .08f;

	//INITIAL BLOCK
	bool isPlayerInRange = false;
	public void Initialize (Action proceedToExecute)
	{
		timePerShiver = 1 / shiversPerSecond;
		defaultRot = moustacheBoy.rotation;
		moustacheBoy.rotation *= Quaternion.Euler(new Vector3(shiverXAngle, 0, 0));
		transform.position = moustacheBoy.position;

		player = GameObject.FindGameObjectWithTag("Player");
		audioSource = GetComponent<AudioSource>();

		StartCoroutine(Shiver(proceedToExecute));
	}
	IEnumerator Shiver (Action proceedToExecute)
	{
		float shiverTimer = timePerShiver * .5f, sneezeTimer = 0;
		bool shiverDirection = false;

		while (true) {
			moustacheBoy.Rotate(new Vector3(0, (-1 + shiverDirection.ToInt() * 2) * shiverYAngle * 2 * shiversPerSecond * Time.deltaTime, 0));

			shiverTimer += Time.deltaTime;
			if (shiverTimer >= timePerShiver) {
				shiverDirection = !shiverDirection;
				shiverTimer = 0;
			}
			Debug.Log(Vector3.Distance(player.transform.position, moustacheBoy.position));
			if (IsPlayerInRange()) {
				pressButtonPopup.SetActive(true);
				sneezeTimer += Time.deltaTime;

				if (sneezeTimer >= timeBeforeSneeze) {
					sneezed = true;
					proceedToExecute();
					break;
				} else if (Input.GetButtonDown("A Button")) {
					sneezed = false;
					moustacheBoy.rotation = defaultRot;
					proceedToExecute();
					break;
				}
			} else {
				pressButtonPopup.SetActive(false);
				sneezeTimer = 0;
			}
			yield return null;
		}
	}
	bool IsPlayerInRange ()
	{
		if (Vector3.Distance(player.transform.position, moustacheBoy.position) < NewWallMechanic.triggerAbilityRange)
			return true;
		else
			return false;
	}


	//EXECUTION BLOCK
	public void Execute (Action proceedToEnd)
	{
		if (sneezed)
			StartCoroutine(Sneeze(proceedToEnd));
		else
			proceedToEnd();
	}

	IEnumerator Sneeze (Action proceedToEnd)
	{
		for (float t = 0; t < sneezeBackTime; t += Time.deltaTime) {
			moustacheBoy.Rotate(new Vector3(sneezeBackRotation / sneezeBackTime * Time.deltaTime, 0, 0));
			yield return null;
		}

		audioSource.clip = sneezeClip;
		audioSource.Play();

		for (float t = 0; t < sneezeFrontTime; t += Time.deltaTime) {
			moustacheBoy.Rotate(new Vector3((Mathf.Abs(sneezeBackRotation) + sneezeFrontRotation) / sneezeFrontTime * Time.deltaTime, 0, 0));
			yield return null;
		}

		for (float t = 0; t < .4f; t += Time.deltaTime) {
			moustacheBoy.Rotate(new Vector3(-sneezeFrontRotation / .4f * Time.deltaTime, 0, 0));
			yield return null;
		}

		yield return new WaitForSeconds(.7f);
		transform.position = player.transform.position;
		audioSource.clip = blessYouClip;
		audioSource.Play();

		yield return new WaitForSeconds(1.4f);
		transform.position = moustacheBoy.position;
		audioSource.clip = thankYouClip;
		audioSource.Play();

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


	//END BLOCK
	public void End (Action endEncounter)
	{
		endEncounter();
	}
}