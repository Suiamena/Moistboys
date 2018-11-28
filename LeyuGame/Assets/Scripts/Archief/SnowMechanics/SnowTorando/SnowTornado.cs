using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowTornado : MonoBehaviour
{

	int lifetime;
	float speedz;
	float speedy;

	Vector3 movementVector;
	Rigidbody tornadoRig;

	//INTERFACE DINGEN

	public Vector3 playerOffset = new Vector3(0, 6, 3), releaseVelocity = new Vector3(0, 3, 30);
	public float playerLerpFactor = .84f, spinSpeed = 180;

	void Awake ()
	{
		tornadoRig = GetComponent<Rigidbody>();
	}

	void FixedUpdate ()
	{
		Movement();
		Rotate();

		//tornadoRig.velocity = movementVector;
	}

	void Movement ()
	{
		movementVector = new Vector3(speedz, 0, speedy);
	}
	void Rotate ()
	{
		transform.Rotate(new Vector3(0, spinSpeed * Time.fixedDeltaTime, 0));
	}
	void DestroyTornado ()
	{
		Destroy(gameObject, lifetime);
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			StartCoroutine(other.transform.GetComponent<ISnowTornado>().HitBySnowTornado(transform, playerOffset, spinSpeed, playerLerpFactor, releaseVelocity));
		}
	}

}
