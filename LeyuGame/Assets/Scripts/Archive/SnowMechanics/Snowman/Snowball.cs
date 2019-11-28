using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowball : MonoBehaviour
{
    int lifetime = 10;

	Vector3 movementVector;
	Rigidbody rig;

	float snowballSpeed, snowballPushVelocity, snowballPushTime;

	public void Initialize (float snowballSpeed, float snowballPushVelocity, float snowballPushTime)
	{
		rig = GetComponent<Rigidbody>();
		this.snowballSpeed = snowballSpeed;
		this.snowballPushVelocity = snowballPushVelocity;
		this.snowballPushTime = snowballPushTime;
        Destroy(gameObject, lifetime);
	}

	void FixedUpdate ()
	{
		movementVector.z = snowballSpeed;

		rig.velocity = new Vector3(0, rig.velocity.y, movementVector.z);
	}

	private void OnCollisionEnter (Collision other)
	{
		if (other.gameObject.tag == "Player") {
			other.transform.GetComponent<ISnowball>().HitBySnowball(snowballPushVelocity, snowballPushTime, transform.position);
		}
	}
}