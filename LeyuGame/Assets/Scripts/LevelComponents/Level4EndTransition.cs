using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Level4EndTransition : MonoBehaviour
{
	public float transitionLength = 2;
	public Renderer[] renderers;
	Material[][] materials;
	bool running = false;

	public IEnumerator Transition ()
	{
		float[] transitionRate = new float[renderers.Length], transitionFill = new float[renderers.Length];
		materials = new Material[renderers.Length][];

		for (int i = 0; i < renderers.Length; ++i) {
			materials[i] = renderers[i].materials;
			//transitionFill[i] = materials[i].GetFloat("_SurfaceSpreadTop");
			transitionRate[i] = transitionFill[i] / transitionLength;
		}

		for (float t = 0; t < transitionLength; t += Time.deltaTime) {
			for (int i = 0; i < materials.Length; ++i) {
				transitionFill[i] = Mathf.MoveTowards(transitionFill[i], 0, transitionRate[i] * Time.deltaTime);
				Debug.Log(transitionFill[i]);
				//materials[i].SetFloat("_SurfaceSpreadTop", transitionFill[i]);
			}
			yield return null;
		}
	}

	private void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !running) {
			running = true;
			StartCoroutine(Transition());
		}
	}
}