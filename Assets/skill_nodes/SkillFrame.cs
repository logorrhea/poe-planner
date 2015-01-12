using UnityEngine;
using System.Collections;

public class SkillFrame : MonoBehaviour {

	public Sprite unallocatedIcon;
	public Sprite allocatedIcon;
	public Sprite unallocatedBorder;
	public Sprite allocatedBorder;

	public bool allocated;

	public GameObject iconObj;
	public GameObject borderObj;
	
	public string[] effects;
	public int nodeId;
	public string nodeName;

	/** References to the SpriteRenderers for the border and icon */
	private SpriteRenderer border;
//	private SpriteRenderer icon;

	// Use this for initialization
	void Start () {
		border = borderObj.GetComponent<SpriteRenderer>();
//		icon = iconObj.GetComponent<SpriteRenderer>();
		border.sprite = unallocatedBorder;
//		icon.sprite = unallocatedIcon;


		/** Establish visuals */
		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		mf.mesh = mesh;

		// Create mesh verts
		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3(0, 0, 0);
		verts[1] = new Vector3(10, 0, 0);
		verts[2] = new Vector3(0, 10, 0);
		verts[3] = new Vector3(10, 10, 0);
		mesh.vertices = verts;

		// Create mesh triangles
		int[] tris = new int[6];
		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;
		tris[3] = 2;
		tris[4] = 3;
		tris[5] = 1;
		mesh.triangles = tris;

		// Create normal map
		Vector3[] normals = new Vector3[4];
		normals[0] = Vector3.forward;
		normals[1] = Vector3.forward;
		normals[2] = Vector3.forward;
		normals[3] = Vector3.forward;
		mesh.normals = normals;
		
		// Create texture map
		Vector2[] uv = new Vector2[4];
		uv[0] = new Vector2(0, 0);
		uv[1] = new Vector2(1, 0);
		uv[2] = new Vector2(0, 1);
		uv[3] = new Vector2(1, 1);
		mesh.uv = uv;

	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown() {
		if (allocated) {
			allocated = false;
			border.sprite = unallocatedBorder;
		} else {
			allocated = true;
			border.sprite = allocatedBorder;
		}
	}
}
