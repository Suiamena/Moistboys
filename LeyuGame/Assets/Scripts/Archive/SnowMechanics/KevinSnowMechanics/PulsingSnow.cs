using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsingSnow : MonoBehaviour
{
	public GameObject pulsePrefab;
	public bool pulsingEnabled = true;
	public float minTimeBetweenPulses = 6, maxTimeBetweenPulses = 9, pulseLateralGrowthRate = 3, pulseVerticalGrowthRate = -.4f, pulseMaxRadius = 12, velocityToPlayerOnHit = 10;
	public int numberOfPulses = 9999;
	int pulsesPerformed = 0;

	List<GameObject> pulses = new List<GameObject>();
	bool pulseRoutineRunning = false;

	void Start ()
	{
		if (pulsingEnabled && !pulseRoutineRunning) {
			pulseRoutineRunning = true;
			StartCoroutine(PulseRoutine());
		}
	}

	void Update ()
	{
		for (int i = 0; i < pulses.Count; i++) {
			if (pulses[i].transform.localScale.x >= pulseMaxRadius) {
				Destroy(pulses[i]);
				pulses.RemoveAt(i);
				pulses.TrimExcess();
				i--;
			} else {
				pulses[i].transform.localScale += new Vector3(pulseLateralGrowthRate, pulseVerticalGrowthRate, pulseLateralGrowthRate) * Time.deltaTime;
			}
		}

		if (pulsingEnabled && !pulseRoutineRunning) {
			pulseRoutineRunning = true;
			StartCoroutine(PulseRoutine());
		}
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player") {
			StopCoroutine(PulseRoutine());
			foreach (GameObject g in pulses)
				Destroy(g);
			Destroy(gameObject);
		}
	}

	IEnumerator PulseRoutine ()
	{
		pulses.Add(Instantiate(pulsePrefab, transform.position - Vector3.up * .25f, Quaternion.identity));
		pulses[pulses.Count - 1].GetComponent<PulsingSnowPulse>().Initialize(velocityToPlayerOnHit);

		float timeBeforeNextPulse = Random.Range(minTimeBetweenPulses, maxTimeBetweenPulses);
		for (float t = 0; t < timeBeforeNextPulse; t += Time.deltaTime) {
			if (!pulsingEnabled) {
				pulseRoutineRunning = false;
				yield break;
			}
			yield return null;
		}

		pulsesPerformed++;

		if (pulsingEnabled && pulsesPerformed < numberOfPulses)
			yield return StartCoroutine(PulseRoutine());
		else {
			pulseRoutineRunning = false;
			pulsesPerformed = 0;
		}
	}
}