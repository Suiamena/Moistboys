using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Level4EndTransition : MonoBehaviour
{
	public float transitionLength = 2;
	public PostProcessVolume beforePostProcessing, afterPostProcessing;
	public Renderer[] renderers;
	List<Material> materials;
	bool running = false;

	private void Awake ()
	{
		beforePostProcessing.weight = 1;
		afterPostProcessing.weight = 0;
	}

	public IEnumerator Transition ()
	{
		List<float> transitionRate = new List<float>(), transitionFill = new List<float>();
		materials = new List<Material>();

		for (int i = 0; i < renderers.Length; ++i) {
			foreach (Renderer r in renderers) {
				foreach (Material m in r.materials) {
					if (m.HasProperty("_SurfaceSpreadTop"))
						materials.Add(m);
				}
			}
		}

		for (int i = 0; i < materials.Count; i++) {
			transitionFill.Add(materials[i].GetFloat("_SurfaceSpreadTop"));
			transitionRate.Add(transitionFill[i] / transitionLength);
		}

		for (float t = 0; t < transitionLength; t += Time.deltaTime) {
			for (int i = 0; i < materials.Count; ++i) {
				transitionFill[i] = Mathf.MoveTowards(transitionFill[i], 0, transitionRate[i] * Time.deltaTime);
				materials[i].SetFloat("_SurfaceSpreadTop", transitionFill[i]);
			}
			yield return null;
		}

		foreach (Renderer r in renderers)
			r.transform.tag = "Rock";
	}
}