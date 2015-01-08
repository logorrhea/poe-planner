using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ClassNode : MonoBehaviour {

	public Sprite activeSprite;
	public Sprite inactiveSprite;

	public bool active = false;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (active) {
			spriteRenderer.sprite = activeSprite;
		} else {
			spriteRenderer.sprite = inactiveSprite;
		}
	}
}
