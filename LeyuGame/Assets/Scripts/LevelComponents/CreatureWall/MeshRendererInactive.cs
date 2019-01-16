using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshRendererInactive : MonoBehaviour {

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

}