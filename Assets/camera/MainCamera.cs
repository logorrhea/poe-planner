using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public float moveRate;
	public float zoomRate;
	public float minOrthoSize;
	public float maxOrthoSize;


	// Use this for initialization
	void Start () {
//		lastMousePos = nil;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 movement = Vector3.zero;

		// Pan the camera according to mouse movement

#if UNITY_IOS

		/**
		 * PINCH ZOOM
		 */
		if (Input.touchCount == 2) {

			// Store both touches
			Touch finger1 = Input.GetTouch(0);
			Touch finger2 = Input.GetTouch(1);

			// Find the previous positions of each touch
			Vector2 finger1Prev = finger1.position - finger1.deltaPosition;
			Vector2 finger2Prev = finger2.position - finger2.deltaPosition;

			// Find the magnitude of the vector between the touches in each frame
			float prevTouchDeltaMag = (finger1Prev - finger2Prev).magnitude;
			float touchDeltaMag = (finger1.position - finger2.position).magnitude;

			// Find the difference in the distances between each frame
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			// Update the zoominess
			camera.orthographicSize += deltaMagnitudeDiff * zoomRate;
			camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minOrthoSize, maxOrthoSize);
		}

		/**
		 * TOUCH DRAG
		 */
		if (Input.touchCount == 1) {

			// Store touch
			Touch finger = Input.GetTouch(0);

			// Find velocity, apply to camera rigidbody
			Vector2 velocity = -finger.deltaPosition/finger.deltaTime;
			float zoomFactor = moveRate * (camera.orthographicSize / maxOrthoSize);
			rigidbody2D.AddForce(velocity * zoomFactor);
		}


	
#else

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
#endif


		// Move the camera if input given
		if (movement != Vector3.zero) {
			movement = movement * Time.deltaTime * moveRate;
			camera.transform.Translate(movement);
		}
	
	}

}
