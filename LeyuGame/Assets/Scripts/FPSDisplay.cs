using UnityEngine;
using System;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	float deltaTime = 0.0f;
	Queue averageFPSStack = new Queue();
	int averageFPSMaxSize = 100;

	private void Awake ()
	{
		averageFPSStack.Enqueue(0.0f);
	}

	void Update ()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	void OnGUI ()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 4 / 100);
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = h * 4 / 100;
		style.normal.textColor = new Color(0.1f, 0.2f, 0.7f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		averageFPSStack.Enqueue(fps);
		if (averageFPSStack.Count > averageFPSMaxSize)
			averageFPSStack.Dequeue();
		float averageFPS = 0;
		foreach (object f in averageFPSStack) {
			averageFPS += Convert.ToSingle(f);
		}
		averageFPS = Mathf.Round(averageFPS / averageFPSStack.Count * 10) * .1f;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		text += ", average: " + averageFPS.ToString();
		GUI.Label(rect, text, style);
	}
}