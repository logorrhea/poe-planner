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

	public int steps;
	private int prevSteps;

	public Material lineMaterial;
	
	struct Node {
		public long id;
		public string name;
		public float x;
		public float y;
		public int tier;
		public string[] desc;
	}

	// Keep track of the pertinent information about each node
	// Let the GameObject handle the rendering and such
	private Dictionary<long, Node> nodes;
	private Line[] lines;

	void Start () {
	
		// Initialize dictionary of nodes
		nodes = new Dictionary<long, Node>();

		// Store previous line width & steps
		prevLineWidth = lineWidth;
		prevSteps = steps;

		// Read JSON data from file and instantiate nodes
		TextAsset fileData = (TextAsset) Resources.Load("node_data");
		JSONObject data = new JSONObject(fileData.text);
		if (data.IsArray) {
			foreach(JSONObject node in data.list) {
				createNode (node);
			}
		}

		// Spawn Class starting nodes
		fileData = (TextAsset) Resources.Load("start_node_data");
		data = new JSONObject(fileData.text);
		if (data.IsArray) {
			foreach(JSONObject startNode in data.list) {
				createStartNode(startNode);
			}
		}

		// Read line data and create line objects
		createGraphLines();	
	}


	void Update() {

		// If line width or steps has changed, flag for
		// line regeneration
		bool recreateLines = false;
		if (prevLineWidth != lineWidth) {
			prevLineWidth = lineWidth;
			recreateLines = true;
		}
		if (prevSteps != steps) {
			prevSteps = steps;
			recreateLines = true;
		}

		// Regenerate lines if necessary
		if (recreateLines) {
			createGraphLines();
		}

		// Render all lines
		if (lines != null) {
			foreach (Line line in lines) {
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


	private void createGraphLines() {
		int i = 0;

		// Read line data from file
		TextAsset fileData = (TextAsset) Resources.Load("line_data");
		JSONObject data = new JSONObject(fileData.text);

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
			line.radius = lineData.GetField("radius").f/100f;
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

		// Start position for the curve
		Vector3 startPos = new Vector3(line.x, line.y, 1);

		// Vertices should be twice the number of points around the curve
		Vector3[] verts = new Vector3[(steps + 1) * 2];

		// Create a new mesh for this line
		Mesh lineMesh = new Mesh();

		// Unity's 0 point on a circle seems to be reversed (or theirs was)
		// Probably has to do with flipping the y-axis
		float start = - line.start; 
		float delta = line.delta;
		float stepSize = delta/steps;

		// Create vertices
		float x, y, rad;
		float[] radii = new float[]{line.radius + (lineWidth/2), line.radius - (lineWidth/2)};
		for (int i = 0; i <= steps; i++) {
			rad = start - stepSize * i;
			for (int r = 0; r < radii.Length; r++) {
				x = radii[r] * Mathf.Cos (rad);
				y = radii[r] * Mathf.Sin (rad);
				verts[2*i + r] = new Vector3(x, y, 1);
			}
		}
		lineMesh.vertices = verts;


		// Set up mesh normals
		Vector3[] norms = new Vector3[(steps + 1) * 2];
		for(int i = 0; i < norms.Length; i++) {
			norms[i] = Vector3.up;
		}

		lineMesh.normals = norms;

		// Set up triangles
		int t;
		int[] tris = new int[steps * 2 * 3];
		for (int s = 0; s < steps; s++) {
			t = 2*s;
			tris[3*t]     = 2*s;
			tris[3*t + 1] = 2*s + 2;
			tris[3*t + 2] = 2*s + 1;
			t = 2*s + 1;
			tris[3*t]     = 2*s + 2;
			tris[3*t + 1] = 2*s + 3;
			tris[3*t + 2] = 2*s + 1;
		}
		lineMesh.triangles = tris;

		// Set up UV coordinates
		// @TODO: Actually construct correct UV coordinates
		lineMesh.uv = new Vector2[verts.Length];

		line.rotation = Quaternion.identity; // Rotation is already accounted for in construction of the line
		line.position = startPos;
		line.mesh = lineMesh;

		Node endNode = nodes[line.endNode];
		Node startNode = nodes[line.startNode];
		line.end_x = endNode.x;
		line.end_y = endNode.y;
		line.start_x = startNode.x;
		line.start_y = startNode.y;
		line.layer = LayerMask.NameToLayer("Default");


	}

}
