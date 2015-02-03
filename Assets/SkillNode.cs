using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class SkillNode : MonoBehaviour {

	public long id;
	public string name;
	public float x;
	public float y;
	public int tier;
	public string[] descriptions;
	
	public Vector2 texSize;
	public Vector2 texCoords;
	
	public Sprite[] allocatedBorders;
	public Sprite[] unallocatedBorders;
	
	private float[] sizesX = new float[]{27f, 38f, 53f};
	private float[] sizesY = new float[]{27f, 38f, 54f};

	private Sprite allocatedBorder;
	private Sprite unallocatedBorder;
	
	private Vector2 texMapSize = new Vector2(693f, 764f);
	
	private SpriteRenderer border;
	private MeshRenderer meshRenderer;
	
	
	void Start() {
		
	}
	
	// Use this to initialize the object's data
	public void InitParams(JSONObject data) {
		 
		 // Set tier
		 tier = (int)data.GetField("tier").f;
		 
		 // Set texture width and height
		 texSize = new Vector2(sizesX[tier], sizesY[tier]);
		 
		 // Set texture offset
		 JSONObject textureData = data.GetField("sprite");
		 texCoords = new Vector2(textureData.GetField("x").f, textureData.GetField("y").f);
		 
		 // Set border sprites
		 allocatedBorder = allocatedBorders[tier];
		 unallocatedBorder = unallocatedBorders[tier];
		 
		 // Build the mesh
		 BuildMesh();
	}
	
	// Creates a new mesh based on the node data
	public void BuildMesh() {
	
		// Generate the mesh data
		Vector3[] verts = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		
		int[] triangles = new int[6];
		
		verts[0] = new Vector3(-texSize.x/2, -texSize.y/2, 0);
		verts[1] = new Vector3(texSize.x/2, -texSize.y/2, 0);
		verts[2] = new Vector3(-texSize.x/2, texSize.y/2, 0);
		verts[3] = new Vector3(texSize.x/2, texSize.y/2, 0);
		
		normals[0] = Vector3.up;
		normals[1] = Vector3.up;
		normals[2] = Vector3.up;
		normals[3] = Vector3.up;
		
		float ux1 = texCoords[0] / texMapSize.x;
		float ux2 = (texCoords[0] + texSize[0]) / texMapSize.x;
		float uy1 = (texMapSize.y - texCoords[1] - texSize[1]) / texMapSize.y;
		float uy2 = (texMapSize.y - texCoords[1]) / texMapSize.y;
		
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
		
		// Assign new mesh to meshFilter and meshCollider
		meshFilter.mesh = mesh;
	}
}
