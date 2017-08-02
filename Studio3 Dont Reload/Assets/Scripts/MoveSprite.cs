using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSprite : MonoBehaviour {
	public Vector3 target;
	public float speed;
	private Vector3 position;
	// Use this for initialization
	void Start () {
		target = new Vector3 (90, 4, 0);
		position = gameObject.transform.position;
		 speed = 0.6f;
	}

	// Update is called once per frame
	void Update () {
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, target, step);
	}
}
