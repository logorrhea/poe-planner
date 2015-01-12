using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class RenderNodeIcon : MonoBehaviour {

	public float sizeX;
	public float sizeY;

	public float[] texSize;
	public float[] texCoords;

	private float imageWidth = 693f;
	private float imageHeight = 764f;

	private float[] sizesX = new float[]{27f, 38f, 53f};
	private float[] sizesY = new float[]{27f, 38f, 54f};

	// Use this for initialization
	void Start () {
	}

	public void InitiateParams(JSONObject data) {
		 
		// Set tier, and mesh size
		int tier = (int)data.GetField("tier").f;
		sizeX = sizesX[tier] / 100f;
		sizeY = sizesY[tier] / 100f;

		// Set texture width and height
		texSize = new float[2];
		texSize[0] = sizesX[tier];
		texSize[1] = sizesY[tier];

		// Set texture offset
		JSONObject textureData = data.GetField("sprite");
		texCoords = new float[2];
		texCoords[0] = textureData.GetField("x").f;
		texCoords[1] = textureData.GetField("y").f;

		// Once params are in place, we can construct the mesh
		BuildMesh ();
	}

	private void BuildMesh() {


		// Generate the mesh data
		Vector3[] verts = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uv = new Vector2[4];

		int[] triangles = new int[6];

		verts[0] = new Vector3(-sizeX/2, -sizeY/2, 0);
		verts[1] = new Vector3(sizeX/2, -sizeY/2, 0);
		verts[2] = new Vector3(-sizeX/2, sizeY/2, 0);
		verts[3] = new Vector3(sizeX/2, sizeY/2, 0);

		normals[0] = Vector3.up;
		normals[1] = Vector3.up;
		normals[2] = Vector3.up;
		normals[3] = Vector3.up;

		float ux1 = texCoords[0] / imageWidth;
		float ux2 = (texCoords[0] + texSize[0]) / imageWidth;
		float uy1 = (imageHeight - texCoords[1] - texSize[1]) / imageHeight;
		float uy2 = (imageHeight - texCoords[1]) / imageHeight;

		uv[0] = new Vector2(ux1, uy1);
		uv[1] = new Vector2(ux2, uy1);
		uv[2] = new Vector2(ux1, uy2);
		uv[3] = new Vector2(ux2, uy2);

		triangles[0] = 0;
		triangles[1] = 2;
		triangles[2] = 1;

		triangles[3] = 2;
		triangles[4] = 3;
		triangles[5] = 1;

		// Create new Mesh and populate with data
		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.triangles = triangles;

		// Get relevant components
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

		// Assign new mesh to meshFilter and meshCollider
		meshFilter.mesh = mesh;

	}
}
