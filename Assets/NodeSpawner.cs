using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class NodeSpawner : MonoBehaviour {

	public GameObject[] prefabs;
	public GameObject startNodePrefab;

	public float lineWidth;
	private float prevLineWidth;
	public Material lineMaterial;
	
	struct Node {
		public long id;
		public string name;
		public float x;
		public float y;
		public int tier;
		public string[] desc;
	}
	
	struct Line {
		public long startNode;
		public long endNode;

		// 0 or 1; 0 = straight, 1 = curved
		public int type;
		
		// Type 0 (straight) variables
		public float start_x;
		public float start_y;
		public float end_x;
		public float end_y;
		
		// Type 1 (curved) variables
		public float start;
		public float delta;
		public float radius;
		public float x;
		public float y;

		// Graphics-related variables
		public Mesh mesh;
		public Quaternion rotation;
		public Vector3 position;
		public int layer;
	}
	
	// Keep track of the pertinent information about each node
	// Let the GameObject handle the rendering and such
	private Dictionary<long, Node> nodes;
	private Line[] lines;
	
	private string nodeDataFile = "Assets/node_data.json";
	private string startNodeDataFile = "Assets/start_node_data.json";
	private string lineDataFile = "Assets/line_data.json";


	void Start () {
	
		// Initialize dictionary of nodes
		nodes = new Dictionary<long, Node>();

		// Store previous line width
		prevLineWidth = lineWidth;

		// Read JSON data from file and instantiate nodes
		JSONObject data = new JSONObject(readDataFile(nodeDataFile));
		if (data.IsArray) {
			foreach(JSONObject node in data.list) {
				createNode (node);
			}
		}

		// Spawn Class starting nodes
		data = new JSONObject(readDataFile(startNodeDataFile));
		if (data.IsArray) {
			foreach(JSONObject startNode in data.list) {
				createStartNode(startNode);
			}
		}

		// Read line data and create line objects
		createGraphLines();	
	}


	void Update() {

		// Regenerate graph lines if line width has changed
		if (prevLineWidth != lineWidth) {
			prevLineWidth = lineWidth;
			createGraphLines();
		}

//		Line line = lines[2];
//
//		Vector3 startPos= new Vector3(line.start_x, line.start_y, 0);
//		Vector3 endPos = new Vector3(line.end_x, line.end_y, 0);
//
//		Graphics.DrawMesh (line.mesh, line.position, line.rotation, lineMaterial, line.layer);
//		Debug.DrawLine (startPos, endPos, Color.green, 10000f);

		foreach (Line line in lines) {
			if (line.type == 0) {
				Graphics.DrawMesh (line.mesh, line.position, line.rotation, lineMaterial, line.layer);
			}
		}
	}


	/**
	 * Creates a new skill node given the JSONObject containing the
	 * information about that node.
	 */
	private void createNode(JSONObject data) {
	
		Node node = new Node();
		
		// Get node ID
		node.id = (long)data.GetField("id").f;

		// Get node name
		node.name = data.GetField ("name").str;

		// Get description text
		JSONObject descs = data.GetField ("desc");
		node.desc = new string[descs.Count];
		int i = 0;
		foreach(JSONObject val in descs.list) {
			node.desc[i] = val.ToString();
			i++;
		}

		// Determine node tier (minor, major, keystone)
		node.tier = (int)data.GetField ("tier").f;

		// Determine node position
		JSONObject location = data.GetField("location");
		node.x = location.GetField("x").f / 100f;  // x & y are 100x larger than necessary
		node.y = location.GetField("y").f / -100f; // y coords are reversed
		
		// Add node to the list of nodes
		nodes.Add(node.id, node);
		
		// Create the game object, and instantiate its variables
		GameObject nodeObj = (GameObject)Instantiate(
			prefabs[node.tier],
			new Vector3(node.x, node.y, 0),
			Quaternion.identity);
		nodeObj.SendMessage ("InitiateParams", data);
	}

	private void createStartNode(JSONObject data) {

		JSONObject location = data.GetField("location");
		Vector3 pos = new Vector3(
			location.GetField("x").f / 100,
			location.GetField("y").f/100,
			0);
		GameObject node = (GameObject)Instantiate(startNodePrefab, pos, Quaternion.identity);
		node.SendMessage ("InitiateParams", data);
	}


	/**
	 * Helper function for reading JSON data files
	 */
	private string readDataFile(string filePath) {
		byte[] buffer;
		FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
		try {
			int len = (int)fs.Length;
			buffer = new byte[len];
			int count;
			int sum = 0;

			while((count = fs.Read(buffer, sum, len - sum)) > 0) {
				sum += count;
			}
		}
		finally {
			fs.Close();
		}

		return ASCIIEncoding.ASCII.GetString (buffer);
	}

	private void createGraphLines() {
		int i = 0;
		JSONObject data = new JSONObject(readDataFile(lineDataFile));
		if (data.IsArray) {
			lines = new Line[data.Count];
			foreach(JSONObject lineData in data.list) {
				Line line = createLine(lineData);
				lines[i] = line;
				i++;
			}
		}
	}

	private Line createLine(JSONObject lineData) {
		Line line = new Line();
		line.type = (int)lineData.GetField("type").f;
		line.startNode = (int)lineData.GetField("start_node").f;
		line.endNode = (int)lineData.GetField("end_node").f;
		if (line.type == 0) {
			line.start_x = lineData.GetField("start_x").f/100f;
			line.start_y = -lineData.GetField("start_y").f/100f;
			line.end_x = lineData.GetField("end_x").f/100f;
			line.end_y = -lineData.GetField("end_y").f/100f;
			createStraightLineMesh(ref line);
		} else {
			line.start = lineData.GetField("start").f;
			line.delta = lineData.GetField("delta").f;
			line.radius = lineData.GetField("radius").f;
			line.x = lineData.GetField("x").f/100f;
			line.y = -lineData.GetField("y").f/100f;
			createBezierLineMesh(ref line);
		}
		return line;
	}


	private void createStraightLineMesh(ref Line line) {

		// Gather start and end points for the line
		Vector3 startPos= new Vector3(line.start_x, line.start_y, 0);
		Vector3 endPos = new Vector3(line.end_x, line.end_y, 0);

		// Calculate line length and angle from start and end points
		Vector3 fromTo = endPos - startPos;
		float mag = Vector3.Distance(startPos, endPos);
		Vector3 direction = fromTo/mag;

		// Create quaternion from rotation angle from directional vector
		Quaternion rotation = Quaternion.FromToRotation(Vector3.up, direction);

		// Create vertices, normals, uv coords, and triangles for the line mesh
		Vector3[] verts = new Vector3[4];
		Vector3[] norms = new Vector3[4];
		Vector2[] uv = new Vector2[4];
		int[] tris = new int[6];

		verts[0] = new Vector3(-lineWidth/2, 0, 2);
		verts[1] = new Vector3(lineWidth/2, 0, 2);
		verts[2] = new Vector3(-lineWidth/2, mag, 2);
		verts[3] = new Vector3(lineWidth/2, mag, 2);

		norms[0] = Vector3.up;
		norms[1] = Vector3.up;
		norms[2] = Vector3.up;
		norms[3] = Vector3.up;

		tris[0] = 0;
		tris[1] = 2;
		tris[2] = 1;
		tris[3] = 2;
		tris[4] = 3;
		tris[5] = 1;

		// Create new mesh, and set properties
		Mesh lineMesh = new Mesh();
		lineMesh.vertices = verts;
		lineMesh.normals = norms;
		lineMesh.triangles = tris;
		lineMesh.uv = uv;

		// Save the mesh
		line.position = startPos;
		line.rotation = rotation;
		line.mesh = lineMesh;
		line.layer = LayerMask.NameToLayer("Default");

	}

	private void createBezierLineMesh(ref Line line) {
		// @TODO: Implement me!
	}

}
