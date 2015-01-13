using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class ClassNode : MonoBehaviour {

	public Sprite inactiveSprite;
	public Sprite activeSprite;
	public bool allocated = false;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (allocated) {
			spriteRenderer.sprite = activeSprite;
		} else {
			spriteRenderer.sprite = inactiveSprite;
		}
	}

	public void InitiateParams(JSONObject data) {
		string assetPath = "Assets/images/" + data.GetField ("icon").str;
		Sprite bgImage = (Sprite)Resources.LoadAssetAtPath(assetPath, typeof(Sprite));
		activeSprite = bgImage;
	}
}
