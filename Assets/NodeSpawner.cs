using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class NodeSpawner : MonoBehaviour {

	public GameObject[] prefabs;
	public GameObject startNodePrefab;
	
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
		public float x;
		public float y;
		
		// 0 or 1; 0 = straight, 1 = curved
		public int type;
		
		// Type 0 (straight) variables
		public float start_x;
		public float start_y;
		public float end_x;
		public float end_y;
		
		// Type 1 (curved) variables
		public float delta;
		public float radius;
	}
	
	// Keep track of the pertinent information about each node
	// Let the GameObject handle the rendering and such
	private Dictionary<long, Node> nodes;
	
	private string nodeDataFile = "Assets/node_data.json";
	private string startNodeDataFile = "Assets/start_node_data.json";
	private string lineDataFile = "Assets/line_data.json";

	void Start () {
	
		// Initialize dictionary of nodes
		nodes = new Dictionary<long, Node>();

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
		
		// Draw lines connecting graph nodes
		data = new JSONObject(readDataFile(lineDataFile));
		if (data.IsArray) {
			foreach(JSONObject line in data.list) {
				drawLine(line);
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


	/**
	 * Private helper function for drawing graph lines
	 * @TODO: Implement drawing functionality for type 2 (curved) lines
	 */
	 private void drawLine(JSONObject data) {
		 
	 }

}
