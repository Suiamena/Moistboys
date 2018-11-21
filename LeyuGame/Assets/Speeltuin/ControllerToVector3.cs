using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ControllerToVector3 : MonoBehaviour, IData<Vector3>
{
	[SerializeField]
	float factor = 3;

	public Vector3 YieldData ()
	{
		Vector3 vector;
		vector = new Vector3(Input.GetAxis("Left Stick X"), 0, Input.GetAxis("Left Stick Y")) * factor;
		return vector;
	}
}