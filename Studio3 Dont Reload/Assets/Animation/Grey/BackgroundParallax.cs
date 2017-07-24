using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackgroundParallax : MonoBehaviour {
	public float Speed = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		Vector2 offset = new Vector2 (0,Time.time * Speed);
		GetComponent<Renderer>().material.mainTextureOffset = offset;

	}
}
