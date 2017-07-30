using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {
	public float speed = 2.5f;
	public Transform child;



	// Use this for initialization
	void Start () {
		


	
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		child.transform.rotation = Quaternion.Euler (0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
		transform.Rotate(Vector3.forward, 45 * Time.deltaTime * speed);
	}
}
