using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public float moveRate;
	public float zoomRate;
	public float minOrthoSize;
	public float maxOrthoSize;
	public float minMovement;
	public float cameraBound;


#if UNITY_EDITOR

	private Vector2 nil = new Vector2(-99999f, -99999f);
	private Vector2 lastMousePos;

#endif

	// Use this for initialization
	void Start () {
//		lastMousePos = nil;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 movement = Vector3.zero;

		// Pan the camera according to mouse movement

#if UNITY_IOS || UNITY_ANDROID

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
		 * 1 Finger input; movement and tapping
		 */
		if (Input.touchCount == 1) {

			// Store touch
			Touch finger = Input.GetTouch(0);

			// Calculate distance traveled between finger positions
			// to differentiate between tap and drag
			Vector2 fingerPrevPosition = finger.position - finger.deltaPosition;
			float dist = (finger.position - fingerPrevPosition).magnitude;

			/**
			 * TOUCH DRAG
			 */
			if (dist >= minMovement) {
				// Find velocity, apply to camera rigidbody
				Vector2 velocity = -finger.deltaPosition/finger.deltaTime;
				float zoomFactor = moveRate * (camera.orthographicSize / maxOrthoSize);
				rigidbody2D.AddForce(velocity * zoomFactor);

			/**
			 * TAP NODE
			 */
			} else {
				RaycastHit hit;
				if (finger.phase == TouchPhase.Ended) {
					Ray ray = camera.ScreenPointToRay(finger.position);
					if (Physics.Raycast(ray, out hit)) {
						Debug.DrawLine (ray.origin, hit.point, Color.red);
						hit.collider.SendMessage("Toggle");
					} else {
						Debug.DrawLine (ray.origin, ray.origin + ray.direction*500, Color.yellow);
					}
				}
			}

		}
		
#elif UNITY_EDITOR

		Vector2 newMousePos = Input.mousePosition;
		// Multiply by -1 to get the inverse of the mouse/finger motion
		if (lastMousePos != Vector2.zero) {
			movement = lastMousePos - newMousePos;
		}
		
		// Capture starting mouse position
		if (Input.GetMouseButtonDown(0)) {
			lastMousePos = Input.mousePosition;
		}
		
		// If movement is large enough, move the camera
		if (Input.GetMouseButton(0)) {
			if (movement.magnitude >= minMovement) {
				Vector2 velocity = movement/Time.deltaTime;
				float zoomFactor = moveRate * (camera.orthographicSize / maxOrthoSize);
				rigidbody2D.AddForce(velocity * zoomFactor);
			}
			lastMousePos = newMousePos;
		}
		
		if (Input.GetMouseButtonUp(0)) {
			// If it isn't see if we clicked a node
			if (movement.magnitude < minMovement) {
				Ray ray = camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) {
					hit.collider.SendMessage("Toggle");
				}		
			}
			lastMousePos = Vector2.zero;
		}

		// Check for zoom in/out
		float wheel = Input.GetAxis("Mouse ScrollWheel");
		if (wheel != 0) {
			camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - wheel, minOrthoSize, maxOrthoSize);
		}
#endif

		// Prevent the camera from scrolling far beyond the meaningful part of the graph
		Vector3 clampedPosition = camera.transform.position;
		if (camera.transform.position.x > cameraBound) {
			clampedPosition.x = cameraBound;
		} else if (camera.transform.position.x < -cameraBound) {
			clampedPosition.x = -cameraBound;
		}
		if (camera.transform.position.y > cameraBound) {
			clampedPosition.y = cameraBound;
		} else if (camera.transform.position.y < -cameraBound) {
			clampedPosition.y = -cameraBound;
		}
		camera.transform.position = clampedPosition;
	}

}
