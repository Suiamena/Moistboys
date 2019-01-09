using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureFlyAlong : MonoBehaviour
{

	bool flyAlongRoutineRunning = false;
	Transform moustacheBoy, player;
	Animator moustacheBoyAnimator;
	public Vector3 startingOffset = new Vector3(0, 20, 0), flyingOffset = new Vector3(0, 2.2f, 3.6f), endingOffset = new Vector3(5, 20, 35);
	public float lerpFactor = .14f, baseSpeed = 6, distanceSpeedIncrease = 2, turnRate = 250, flyingSway = 2.5f;
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

		Vector3 targetPos;
		float correctedSpeed;
		while (flyAlong) {
			targetPos = player.position + player.rotation * flyingOffset + new Vector3(Mathf.Sin(Time.time * .1f) * flyingOffset.x, 0, Mathf.Sin(Time.time * 0.08f));

			float distance = Vector3.Distance(moustacheBoy.position, targetPos);
			correctedSpeed = baseSpeed + distance;

			moustacheBoy.position += moustacheBoy.forward * correctedSpeed * Time.deltaTime;

			Vector3 currentPos = moustacheBoy.position;
			currentPos.y = Mathf.Lerp(currentPos.y, targetPos.y, lerpFactor);
			moustacheBoy.position = currentPos;

			float rightDistance = (moustacheBoy.position + moustacheBoy.right).SquareDistance(targetPos);
			if ((moustacheBoy.position + -moustacheBoy.right).SquareDistance(targetPos) < rightDistance)
				moustacheBoy.Rotate(new Vector3(0, -turnRate * Time.deltaTime, 0));
			else
				moustacheBoy.Rotate(new Vector3(0, turnRate * Time.deltaTime, 0));
			yield return null;

			//moustacheBoy.position = Vector3.Lerp(moustacheBoy.position, player.position + player.rotation * flyingOffset, lerpFactor);

			//yAngle = Mathf.Lerp(yAngle, player.eulerAngles.y, lerpFactor);
			//moustacheBoy.eulerAngles = new Vector3(20, yAngle, 0);
			//yield return null;
		}

		targetPos = player.position + Quaternion.Euler(new Vector3(0, player.eulerAngles.y, 0)) * endingOffset;
		Quaternion oldRot = moustacheBoy.rotation;
		moustacheBoy.LookAt(targetPos);
		float targetYAngle = moustacheBoy.eulerAngles.y;
		moustacheBoy.rotation = oldRot;

		while (moustacheBoy.position.SquareDistance(targetPos) > 1) {
			moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.position, targetPos, baseSpeed * Time.deltaTime);
			moustacheBoy.eulerAngles = new Vector3(20, Mathf.Lerp(moustacheBoy.eulerAngles.y, targetYAngle, lerpFactor), 0);
			yield return null;
		}

		flyAlongRoutineRunning = false;
		moustacheBoy.gameObject.SetActive(false);
	}
}