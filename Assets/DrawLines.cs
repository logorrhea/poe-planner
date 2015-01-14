using UnityEngine;
using System.Collections;

public class DrawLines : MonoBehaviour {

	public Material lineMaterial;

	public int steps;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Mesh mesh = new Mesh();
		Vector3 position0 = new Vector3(0, 0, 0);
		Vector3 position1 = new Vector3(10, 0, 0);
		Vector3 position2 = new Vector3(-10, 0, 0);
		Quaternion rotation0 = Quaternion.identity;
		Quaternion rotation1 = Quaternion.AngleAxis(135f, new Vector3(0, 0, 1));
		Quaternion rotation2 = Quaternion.AngleAxis(225f, new Vector3(0, 0, 1));
		int layer = 0;

		mesh = new Mesh();
		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3(0, 0, 0);
		verts[1] = new Vector3(1, 0, 0);
		verts[2] = new Vector3(0, 10, 0);
		verts[3] = new Vector3(1, 10, 0);

		Vector3[] norms = new Vector3[4];
		norms[0] = Vector3.up;
		norms[1] = Vector3.up;
		norms[2] = Vector3.up;
		norms[3] = Vector3.up;

		Vector2[] uv = new Vector2[4];
		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1, 0);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(1, 1);

//		Vector3[] verts = findBezierVerts(0, 10, 0, 10, 5);

		int[] tris = new int[6];

		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;

		tris[3] = 2;
		tris[4] = 3;
		tris[5] = 1;

		// Set mesh properties
		mesh.vertices = verts;
		mesh.normals = norms;
		mesh.uv = uv;
		mesh.triangles = tris;

		// Draw line
		Graphics.DrawMesh (mesh, position0, rotation0, lineMaterial, layer);
		Graphics.DrawMesh (mesh, position1, rotation1, lineMaterial, layer);
		Graphics.DrawMesh (mesh, position2, rotation2, lineMaterial, layer);

		
	}

	private Vector3[] findBezierVerts(float x0, float x1, float y0, float y1) {
		Vector3[] verts = new Vector3[steps];
		verts[0] = new Vector3(x0, y0, 0);
		for (int i = 1; i < steps - 1; i++) {
			verts[i] = new Vector3(
				Mathf.Lerp (x0, x1, i),
				Mathf.Lerp (y0, y1, i),
				0);
		}
		verts[steps - 1] = new Vector3(x1, y1, 0);
		return verts;
	}
}
