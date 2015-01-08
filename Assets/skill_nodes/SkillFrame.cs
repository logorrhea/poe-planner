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

	/** References to the SpriteRenderers for the border and icon */
	private SpriteRenderer border;
	private SpriteRenderer icon;

	// Use this for initialization
	void Start () {
		border = borderObj.GetComponent<SpriteRenderer>();
		icon = iconObj.GetComponent<SpriteRenderer>();
		border.sprite = unallocatedBorder;
		icon.sprite = unallocatedIcon;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnMouseDown() {
		if (allocated) {
			allocated = false;
			border.sprite = unallocatedBorder;
			icon.sprite = unallocatedIcon;
		} else {
			allocated = true;
			border.sprite = allocatedBorder;
			icon.sprite = allocatedIcon;
		}
	}
}
