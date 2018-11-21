using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingSnowPulse : MonoBehaviour
{
	float pushVelocity = 4;

	public void Initialize (float pushVelocity)
	{
		this.pushVelocity = pushVelocity;
	}

	private void OnTriggerStay (Collider other)
	{
		//Debug.Log(other.name);
		if (other.tag == "Player") {
			other.transform.GetComponent<IPulsingSnow>().HitByPulsingSnow(transform.position, pushVelocity);
		}
	}
}