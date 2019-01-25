using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Creature;

namespace Creature
{
	public class CreatureFlyAlong : MonoBehaviour, ICreature
	{
		public LayerMask obstacleDetectMask;
		RaycastHit obstacleRayHit;
		bool flyAlongRoutineRunning = false;
		Transform moustacheBoy, player;
		Animator moustacheBoyAnimator;
		public Vector3 startingOffset = new Vector3(0, 20, 0), flyingOffset = new Vector3(0, 2.2f, 3.6f), endingOffset = new Vector3(5, 20, 35);
		public float lerpFactor = .14f, baseSpeed = 10, flyAwaySpeed = 36, distanceSpeedIncrease = 60, turnRate = 250, flyingSway = 2.5f, obstacleDetectRange = 9, currentXRot = 0;
		bool flyAlong = false;
		Coroutine flyAlongRoutine = null;

		private void Awake ()
		{
			moustacheBoy = transform.GetChild(0);
			moustacheBoyAnimator = moustacheBoy.GetComponent<Animator>();
			moustacheBoy.gameObject.SetActive(false);
		}

		private void OnTriggerEnter (Collider other)
		{
			if (other.CompareTag("Player")) {
				player = other.transform.GetChild(0);
				ICreature script = CreatureManager.activeFlyAlongScript;
				if (script == null) {
					CreatureManager.activeFlyAlongScript = this;
					if (!flyAlongRoutineRunning) {
						flyAlongRoutineRunning = true;
						flyAlongRoutine = StartCoroutine(FlyAlong());
					}
				} else if (script.GetType() == typeof(CreatureFlyAlong)) {
					ReceiveCreature(CreatureManager.activeFlyAlongScript.GiveAwayCreature());
				} else if (script.GetType() == typeof(PlangeMuurInteractive)) {
					//DAFUQ GAAN WE HIERMEE DOEN????
				}
			}
		}
		private void OnTriggerExit (Collider other)
		{
			if (other.CompareTag("Player")) {
				flyAlong = false;
			}
		}

		public Transform GiveAwayCreature ()
		{
			moustacheBoy.gameObject.SetActive(false);
			flyAlong = false;
			flyAlongRoutineRunning = false;
			CreatureManager.activeFlyAlongScript = null;
			StopCoroutine(flyAlongRoutine);
			flyAlongRoutine = null;
			return moustacheBoy;
		}

		public void ReceiveCreature (Transform creatureTrans)
		{
			moustacheBoy.transform.position = creatureTrans.position;
			moustacheBoy.transform.rotation = creatureTrans.rotation;
			flyAlongRoutineRunning = true;
			CreatureManager.activeFlyAlongScript = this;
			if (flyAlongRoutine != null)
				StopCoroutine(flyAlongRoutine);
			flyAlongRoutine = StartCoroutine(FlyAlong(true));
		}

		public void FlyAway ()
		{
			flyAlong = false;
		}

		IEnumerator FlyAlong (bool skipFlyIn = false)
		{
			//Fly in
			flyAlong = true;
			if (!skipFlyIn) {
				moustacheBoy.position = player.position + startingOffset;

				float yAngle = player.eulerAngles.y;
				moustacheBoy.eulerAngles = new Vector3(0, yAngle, 0);
			}

			moustacheBoy.gameObject.SetActive(true);
			moustacheBoyAnimator.SetBool("isFlying", true);

			Vector3 targetPos;
			float correctedSpeed;
			while (flyAlong) {
				//Move 
				targetPos = player.position + player.rotation * flyingOffset + new Vector3(Mathf.Sin(Time.time * .1f) * flyingOffset.x, 0, Mathf.Sin(Time.time * 0.08f));

				float distance = Vector3.Distance(moustacheBoy.position, targetPos);
				correctedSpeed = baseSpeed + distance;

				moustacheBoy.position += moustacheBoy.forward * correctedSpeed * Time.deltaTime;

				//Steer based on targetPos
				float rightDistance = (moustacheBoy.position + moustacheBoy.right).SquareDistance(targetPos);
				if ((moustacheBoy.position + -moustacheBoy.right).SquareDistance(targetPos) < rightDistance)
					moustacheBoy.Rotate(new Vector3(0, -turnRate * Time.deltaTime, 0));
				else
					moustacheBoy.Rotate(new Vector3(0, turnRate * Time.deltaTime, 0));

				//Detect obstacles in front
				float obstacleDetectionTurnFactor = 0;
				for (int i = -1; i <= 1; ++i) {
					if (Physics.Raycast(moustacheBoy.position + moustacheBoy.right * i, moustacheBoy.forward, out obstacleRayHit, obstacleDetectRange, obstacleDetectMask)) {
						currentXRot += -15 * Time.deltaTime;
						if (Vector3.SignedAngle(moustacheBoy.forward, obstacleRayHit.normal, Vector3.up) < 0)
							obstacleDetectionTurnFactor += -1.2f;
						else
							obstacleDetectionTurnFactor += 1.2f;
					}
				}
				obstacleDetectionTurnFactor = Mathf.Clamp(obstacleDetectionTurnFactor, -2.8f, 2.8f);
				float yRotation = turnRate * obstacleDetectionTurnFactor * Time.deltaTime;
				moustacheBoy.Rotate(0, turnRate * obstacleDetectionTurnFactor * Time.deltaTime, 0, 0);

				if (targetPos.y > moustacheBoy.position.y) {
					currentXRot += -45 * Time.deltaTime;
				} else {
					currentXRot += 45 * Time.deltaTime;
				}
				if (Physics.Raycast(moustacheBoy.position, Vector3.down, flyingOffset.y + 1, obstacleDetectMask)) {
					currentXRot += -90 * Time.deltaTime;
				}
				currentXRot = Mathf.Clamp(currentXRot, -25, 25);
				moustacheBoy.rotation = Quaternion.Euler(currentXRot, moustacheBoy.eulerAngles.y, 0);
				yield return null;
			}

			targetPos = player.position + Quaternion.Euler(new Vector3(0, player.eulerAngles.y, 0)) * endingOffset;
			Quaternion oldRot = moustacheBoy.rotation;
			moustacheBoy.LookAt(targetPos);
			float targetYAngle = moustacheBoy.eulerAngles.y;
			moustacheBoy.rotation = oldRot;

			while (moustacheBoy.position.SquareDistance(targetPos) > 1) {
				moustacheBoy.position = Vector3.MoveTowards(moustacheBoy.position, targetPos, flyAwaySpeed * Time.deltaTime);
				moustacheBoy.eulerAngles = new Vector3(20, Mathf.Lerp(moustacheBoy.eulerAngles.y, targetYAngle, lerpFactor), 0);
				yield return null;
			}

			flyAlongRoutineRunning = false;
			moustacheBoy.gameObject.SetActive(false);
			CreatureManager.activeFlyAlongScript = null;
			flyAlongRoutine = null;
		}
	}
}