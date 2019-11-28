using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballThrower : MonoBehaviour
{

	public GameObject snowballPrefab;
	public bool enabled = true;
	public float timeBetweenThrows;

	public float snowballSpeed = -15;
	public float snowballPushVelocity = 10, snowballPushTime = .3f;

	Vector3 spawnLocation;
	bool throwingSnowballs = false;

	void Awake ()
	{
		spawnLocation = transform.position + new Vector3(0, 0, -2);

		if (enabled)
			StartCoroutine(ThrowSnowballs());
	}

	private void Update ()
	{
		if (enabled && !throwingSnowballs) {
			throwingSnowballs = true;
			StartCoroutine(ThrowSnowballs());
		}
	}

	IEnumerator ThrowSnowballs ()
	{
		throwingSnowballs = true;
		Snowball newSnowball = Instantiate(snowballPrefab, spawnLocation, transform.rotation).GetComponent<Snowball>();
		newSnowball.Initialize(snowballSpeed, snowballPushVelocity, snowballPushTime);
		yield return new WaitForSeconds(timeBetweenThrows);
		if (enabled)
			yield return StartCoroutine(ThrowSnowballs());
		else
			throwingSnowballs = false;
	}
}