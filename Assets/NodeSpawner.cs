using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

public class NodeSpawner : MonoBehaviour {

	public GameObject[] prefabs;
	public GameObject startNodePrefab;

	private string nodeDataFile = "Assets/node_data.json";
	private string startNodeDataFile = "Assets/start_node_data.json";

	void Start () {

		// Read JSON data from file and instantiate nodes
		JSONObject nodeData = new JSONObject(readDataFile(nodeDataFile));
		if (nodeData.IsArray) {
			foreach(JSONObject node in nodeData.list) {
				createNode (node);
			}
		}

		// Spawn Class starting nodes
		nodeData = new JSONObject(readDataFile(startNodeDataFile));
		if (nodeData.IsArray) {
			foreach(JSONObject startNode in nodeData.list) {
				createStartNode(startNode);
			}
		}
	
	}


	/**
	 * Creates a new skill node given the JSONObject containing the
	 * information about that node.
	 */
	private void createNode(JSONObject data) {

		// Get node name
		string name = data.GetField("name").ToString ();

		// Get description text
		JSONObject descs = data.GetField ("desc");
		string[] desc = new string[descs.Count];
		int i = 0;
		foreach(JSONObject val in descs.list) {
			desc[i] = val.ToString();
			i++;
		}

		// Determine node tier (minor, major, keystone)
		int tierLevel = (int)data.GetField ("tier").f;

		// Determine node position
		JSONObject location = data.GetField("location");
		Vector3 pos = new Vector3(
			location.GetField("x").f / 100,  // x & y are 100x larger than necessary
			location.GetField("y").f / -100, // y coords are reversed
			0);


		GameObject node = (GameObject)Instantiate(prefabs[tierLevel], pos, Quaternion.identity);
		node.SendMessage ("InitiateParams", data);
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


}
