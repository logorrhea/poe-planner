using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SkillFrame : MonoBehaviour {

	public Sprite unallocatedSprite;
	public Sprite allocatedSprite;

	public bool allocated;

	private SpriteRenderer renderer;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (allocated) {
			renderer.sprite = allocatedSprite;
		} else {
			renderer.sprite = unallocatedSprite;
		}
	}
}
