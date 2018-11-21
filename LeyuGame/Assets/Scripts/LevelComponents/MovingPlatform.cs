using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	Transform platformTrans;
	int index = 0;
	public Vector3[] points;
	public float movementSpeed = 3;

	private void Start ()
	{
		platformTrans = transform.GetChild(0);
	}

	void Update ()
	{
		if (platformTrans.localPosition == points[index]) {
			index++;
			if (index >= points.Length) {
				index = 0;
			}
		}
		platformTrans.localPosition = Vector3.MoveTowards(platformTrans.localPosition, points[index], movementSpeed * Time.deltaTime);
	}
}
