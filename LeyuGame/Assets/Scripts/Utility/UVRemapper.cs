using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVRemapper : MonoBehaviour
{
	Mesh mesh;

	void Start ()
	{
		mesh = new Mesh {
			vertices = GetComponent<MeshFilter>().mesh.vertices,
			triangles = GetComponent<MeshFilter>().mesh.triangles,
			normals = GetComponent<MeshFilter>().mesh.normals
		};

		Vector2[] uvs = GetComponent<MeshFilter>().mesh.uv;

		for (int i = 0; i < uvs.Length; i++) {
			uvs[i] = uvs[i].Divide(transform.localScale);
		}

		mesh.uv = uvs;

		GetComponent<MeshFilter>().mesh = mesh;
	}
}