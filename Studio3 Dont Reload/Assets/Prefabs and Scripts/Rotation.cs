using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
	public float speed = 2.5f;

	// Use this for initialization
	void Start () {
		


	
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate(Vector3.forward, 45 * Time.deltaTime * speed);
	}
}
