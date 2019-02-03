using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VervangGameObject))]
public class VervangGameObjectEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();

		VervangGameObject t = (VervangGameObject) target;

		if (GUILayout.Button("Replace Objects")) {
			t.ReplaceObjects();
		}
	}
}