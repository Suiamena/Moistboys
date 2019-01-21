using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Level4EndTransition : MonoBehaviour
{
	public float transitionLength = 2;
	public Renderer[] renderers;
	Material[] materials;

	public IEnumerator Transition ()
	{
		float[] transitionRate = new float[renderers.Length], transitionFill = new float[renderers.Length];
		materials = new Material[renderers.Length];

		for (int i = 0; i < renderers.Length; ++i) {
			materials[i] = renderers[i].materials[0];
			transitionFill[i] = materials[i].GetFloat("_SurfaceTopSpread");
			transitionRate[i] = transitionFill[i] / transitionLength;
		}

		for (float t = 0; t < transitionLength; t += Time.deltaTime) {
			for (int i = 0; i < materials.Length; ++i) {
				transitionFill[i] -= transitionRate[i] * Time.deltaTime;
				materials[i].SetFloat("_SurfaceSpreadTop", transitionFill[i]);
			}
			yield return null;
		}
	}
}