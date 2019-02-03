using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VervangGameObjects : MonoBehaviour
{
	public GameObject objectToReplaceWith;

	public void ReplaceObjects ()
	{
		int childCount = transform.childCount;
		List<GameObject> objects = new List<GameObject>();
		GameObject[] oldObjects = new GameObject[childCount];

		for (int i = 0; i < childCount; i++) {
			Transform tmp = Instantiate(objectToReplaceWith).transform;
			tmp.position = transform.GetChild(i).position;
			tmp.rotation = transform.GetChild(i).rotation;
			tmp.localScale = transform.GetChild(i).localScale;
			objects.Add(tmp.gameObject);
			oldObjects[i] = transform.GetChild(i).gameObject;
		}

		foreach (GameObject g in oldObjects)
			DestroyImmediate(g);

		foreach (GameObject g in objects)
			g.transform.parent = transform;
	}
}

[CustomEditor(typeof(VervangGameObjects))]
public class VervangGameObjectEditor : Editor
{

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		VervangGameObjects t = (VervangGameObjects) target;

		if (GUILayout.Button("Replace Objects")) {
			t.ReplaceObjects();
		}
	}
}