using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureFlyAlong : MonoBehaviour
{
	bool flyAlongRoutineRunning = false;
	Transform moustacheBoy, player;
	Animator moustacheBoyAnimator;
	public Vector3 startingOffset = new Vector3(0, 20, 0), flyingOffset = new Vector3(3.2f, 2.2f, 3.6f), endingOffset = new Vector3(5, 20, 35);
	public float lerpFactor = .14f;
	bool flyAlong = false;

	private void Awake ()
	{
		moustacheBoy = transform.GetChild(0);
		moustacheBoyAnimator = moustacheBoy.GetComponent<Animator>();
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag("Player")) {
			player = other.transform.GetChild(0);
			if (!flyAlongRoutineRunning) {
				flyAlongRoutineRunning = true;
				StartCoroutine(FlyAlong());
			}
		}
	}
	private void OnTriggerExit (Collider other)
	{
		if (other.CompareTag("Player")) {
			flyAlong = false;
		}
	}

	IEnumerator FlyAlong ()
	{
		flyAlong = true;
		moustacheBoy.position = player.position + startingOffset;
		moustacheBoy.gameObject.SetActive(true);

		float yAngle = player.eulerAngles.y;
		moustacheBoy.eulerAngles = new Vector3(20, yAngle, 0);

		moustacheBoyAnimator.SetBool("isFlying", true);

		while (flyAlong) {
			moustacheBoy.position = Vector3.Lerp(moustacheBoy.position, player.position + player.rotation * flyingOffset, lerpFactor);

			yAngle = Mathf.Lerp(yAngle, player.eulerAngles.y, lerpFactor);
			moustacheBoy.eulerAngles = new Vector3(20, yAngle, 0);
			yield return null;
		}

		Vector3 targetPos = player.position + startingOffset;
		Quaternion oldRot = moustacheBoy.rotation;
		moustacheBoy.LookAt(targetPos);
		float targetYAngle = moustacheBoy.eulerAngles.y;
		moustacheBoy.rotation = oldRot;

		while (moustacheBoy.position.SquareDistance(targetPos) > 1) {
			moustacheBoy.position = Vector3.Lerp(moustacheBoy.position, targetPos, lerpFactor);
			moustacheBoy.eulerAngles = new Vector3(20, Mathf.Lerp(moustacheBoy.eulerAngles.y, targetYAngle, lerpFactor), 0);
			yield return null;
		}

		flyAlongRoutineRunning = false;
		moustacheBoy.gameObject.SetActive(false);
	}
}