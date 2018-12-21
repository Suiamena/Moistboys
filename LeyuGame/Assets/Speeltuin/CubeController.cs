using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;

public class ObjectController : MonoBehaviour, IBehaviour
{
	Data<Vector3>[] movementData;

	void Awake ()
	{
	}

	public void Execute ()
	{
		Vector3 movement = Vector3.zero;
		if (movementData.Length <= 0) {
			Data<Vector3>.DataPriorities highestPriority = Data<Vector3>.DataPriorities.Low;
			foreach (Data<Vector3> d in movementData) {
				movement = d.YieldData(movement, highestPriority);
				highestPriority = d.dataPriority;
			}
		}
		transform.Translate(movement);
	}

	void Update ()
	{
		Execute();
	}
}