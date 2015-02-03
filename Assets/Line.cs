using UnityEngine;
using System.Collections;

public struct Line {
	public long startNode;
	public long endNode;
	
	// 0 = straight
	// 1 = curved
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