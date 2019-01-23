using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Level4EndTransition : MonoBehaviour
{
    public float transitionLength = 2;
    public PostProcessVolume beforePostProcessing, afterPostProcessing;
    float postProcessingRate;
    public Renderer[] renderers;
    List<Material> materials;
    bool running = false;

    private void Awake()
    {
        beforePostProcessing.weight = 1;
        afterPostProcessing.weight = 0;
        postProcessingRate = 1 / transitionLength;
    }

	public void Transition ()
	{
		if (!running) {
			running = true;
			StartCoroutine(TransitionRoutine());
		}
	}

    public IEnumerator TransitionRoutine()
    {
        List<float> transitionRate = new List<float>(), transitionFill = new List<float>();
        materials = new List<Material>();

        //RETRIEVE CORRECT MATERIALS
        for (int i = 0; i < renderers.Length; ++i)
        {
            foreach (Renderer r in renderers)
            {
                foreach (Material m in r.materials)
                {
                    if (m.HasProperty("_SurfaceSpreadTop"))
                        materials.Add(m);
                }
            }
        }

        //RETRIEVE TRANSITION VALUES
        for (int i = 0; i < materials.Count; i++)
        {
            transitionFill.Add(materials[i].GetFloat("_SurfaceSpreadTop"));
            transitionRate.Add(transitionFill[i] / transitionLength);
        }

        //DO THINGS OVER TIME
        for (float t = 0; t < transitionLength; t += Time.deltaTime)
        {
            for (int i = 0; i < materials.Count; ++i)
            {
                transitionFill[i] = Mathf.MoveTowards(transitionFill[i], 0, transitionRate[i] * Time.deltaTime);
                materials[i].SetFloat("_SurfaceSpreadTop", transitionFill[i]);
            }

            beforePostProcessing.weight += -postProcessingRate * Time.deltaTime;
            afterPostProcessing.weight += postProcessingRate * Time.deltaTime;
            yield return null;
        }

		//FINALIZE AND CHANGE TAG FOR CORRECT SOUNDS
        beforePostProcessing.weight = 0;
        afterPostProcessing.weight = 1;

        foreach (Renderer r in renderers)
            r.transform.tag = "Rock";
    }
}