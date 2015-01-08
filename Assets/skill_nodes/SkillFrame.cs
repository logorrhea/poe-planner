using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SkillFrame : MonoBehaviour {

	public Sprite unallocatedIcon;
	public Sprite allocatedIcon;
	public Sprite unallocatedBorder;
	public Sprite allocatedBorder;

	public bool allocated;
	private bool previouslyAllocated = false;

	public GameObject iconObj;
	public GameObject borderObj;

	/** References to the SpriteRenderers for the border and icon */
	private SpriteRenderer border;
	private SpriteRenderer icon;

	// Use this for initialization
	void Start () {
		border = borderObj.GetComponent<SpriteRenderer>();
		icon = iconObj.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

		// Only change the sprites if the node changed
		if (allocated && !previouslyAllocated) {
			border.sprite = allocatedBorder;
			icon.sprite = allocatedIcon;
		} else if (!allocated && previouslyAllocated) {
			border.sprite = unallocatedBorder;
			icon.sprite = unallocatedIcon;
		}
	
	}
}
