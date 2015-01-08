﻿using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	private Vector2 nil = new Vector2(-999999, -999999);
	private float moveRate = 0.1f;
	private float zoomRate = 100.0f;
	private float minOrthoSize = 1f;
	private float maxOrthoSize = 20f;

	Vector2 lastMousePos;

	// Use this for initialization
	void Start () {
		lastMousePos = nil;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 movement = Vector3.zero;

		// Pan the camera according to mouse movement
		if (Input.GetMouseButton (0)) {

			// If lastMousePos is null, record mouse position
			if (lastMousePos == nil) {
				lastMousePos = Input.mousePosition;
			
			// Otherwise, calculate difference and pan camera accordingly
			} else {
				Vector2 newMousePos = Input.mousePosition;
				// Multiply by -1 to get the inverse of the mouse/finger motion
				movement = lastMousePos - newMousePos;
			}

		// If mouse not pressed, set lastMousePos to null
		} else {
			lastMousePos = nil;
		}

		// Check for zoom in/out
		float wheel = Input.GetAxis("Mouse ScrollWheel");
		if (wheel != 0) {
			camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - wheel, minOrthoSize, maxOrthoSize);
		}

		// Move the camera if input given
		if (movement != Vector3.zero) {
			movement = movement * Time.deltaTime * moveRate;
			camera.transform.Translate(movement);
		}
	
	}

}
