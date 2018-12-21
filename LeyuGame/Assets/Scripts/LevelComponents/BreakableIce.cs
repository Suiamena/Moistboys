using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableIce : MonoBehaviour
{
	public GameObject soundPrefab;

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			Instantiate(soundPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}
}
