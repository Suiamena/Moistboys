using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelShit : MonoBehaviour
{
	int count = 0;

	private void OnDrawGizmosSelected ()
	{
		count = CountChildren(transform);
		Debug.Log(count.ToString());
	}

	int CountChildren (Transform parent)
	{
		int result = 0;

		foreach (Transform t in parent) {
			if (t.childCount == 0) {
				result++;
			} else {
				result += CountChildren(t);
			}
		}

		return result;
	}
}