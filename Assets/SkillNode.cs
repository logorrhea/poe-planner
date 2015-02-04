using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SkillNode : MonoBehaviour {

	public int id;
	public int tier;
	public string[] descriptions;
	public bool allocated;
	public bool isRootNode = false;

	public Vector2 size;
	public Vector2 location;
	public Vector2 texSize;
	public Vector2 texCoords;
	
	public Sprite[] allocatedBorders;
	public Sprite[] unallocatedBorders;

	public GameObject borderObj;

	public Material allocatedMaterial;
	public Material unallocatedMaterial;
	
	private float[] sizesX = new float[]{27f, 38f, 53f};
	private float[] sizesY = new float[]{27f, 38f, 54f};
	private float[] colliderSizes = new float[]{0.2f, 0.3f, 0.4f};

	private Sprite allocatedBorder;
	private Sprite unallocatedBorder;
	
	private Vector2 texMapSize = new Vector2(693f, 764f);
	
	private SpriteRenderer border;
	private MeshRenderer meshRenderer;

	public int[] connectedNodes;
	

	void Start() {
		// Grab reference to border/mesh
		border = borderObj.GetComponent<SpriteRenderer>();
		meshRenderer = GetComponent<MeshRenderer>();

		// Set default material/sprite
		meshRenderer.material = unallocatedMaterial;
		border.sprite = unallocatedBorder;
	}
	
	// Use this to initialize the object's data
	public void InitParams(JSONObject data) {

		// Get ID, tier, name, x, and y
		id = int.Parse(data.GetField("id").str);
		tier = int.Parse(data.GetField("tier").str);
		name = data.GetField("name").str;

		// Get connecting node ids
		List<JSONObject> connections = data.GetField("connections").list;
		connectedNodes = new int[connections.Count];
		int i = 0;
		foreach (JSONObject conn in connections) {
			connectedNodes[i++] = int.Parse(conn.str);
		}

		// Get description text
		JSONObject descs = data.GetField("desc");
		descriptions = new string[descs.Count];
		i = 0;
		foreach (JSONObject val in descs.list) {
			descriptions[i++] = val.ToString();
		}

		// Set texture width and height
		texSize = new Vector2(sizesX[tier], sizesY[tier]);

		// Set mesh size and height
		size = texSize / 100f;

		// Change collider radius according to node tier
		SphereCollider sphereCollider = (SphereCollider) collider;
		sphereCollider.radius = colliderSizes[tier];
		 
		// Set texture offset
		JSONObject textureData = data.GetField("sprite");
		texCoords = new Vector2(
				int.Parse(textureData.GetField("x").str),
		    	int.Parse(textureData.GetField("y").str));

		// Set border sprites
		allocatedBorder = allocatedBorders[tier];
		unallocatedBorder = unallocatedBorders[tier];

		// Get node location
		JSONObject locationData = data.GetField("location");
		location = new Vector2(
			(float)decimal.Parse(locationData.GetField("x").str) / 100f,   // location x & y are 100x larger than necessary
			(float)decimal.Parse(locationData.GetField("y").str) / -100f); // y is inverted

		// Build the mesh
		BuildMesh();

		// Move node to proper location
		transform.position = new Vector3(location.x, location.y, 0);
	}
	
	// Creates a new mesh based on the node data
	public void BuildMesh() {
	
		// Generate the mesh data
		Vector3[] verts = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		
		int[] triangles = new int[6];
		
		verts[0] = new Vector3(-size.x/2, -size.y/2, 0);
		verts[1] = new Vector3(size.x/2, -size.y/2, 0);
		verts[2] = new Vector3(-size.x/2, size.y/2, 0);
		verts[3] = new Vector3(size.x/2, size.y/2, 0);
		
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

	public void Toggle() {
		if (allocated) {
			allocated = false;
			border.sprite = unallocatedBorder;
			meshRenderer.material = unallocatedMaterial;

		} else {
			allocated = true;
			border.sprite = allocatedBorder;
			meshRenderer.material = allocatedMaterial;
		}
	}

}
