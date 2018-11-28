using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerData;

public class CubeController : MonoBehaviour
{
	IData<Vector3>[] data;

	void Awake ()
	{
		data = GetComponents<IData<Vector3>>();
	}

	public void Execute ()
	{
		Vector3 movement = Vector3.zero;
		foreach (IData<Vector3> d in data) {
			movement += d.YieldData();
		}
		transform.Translate(movement);
	}

	void Update ()
	{
		Execute();
	}

	Bounce test = new Bounce();
}