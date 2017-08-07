using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualiser : MonoBehaviour {

	public GameObject prefab;
	public int numberofObjects = 20;
	public float radius = 5f;
	public GameObject [] cubes;
	// Use this for initialization
	void Start () {

		for (int i = 0; i < numberofObjects; i++) {
			float angle = i * Mathf.PI * 2 / numberofObjects;
			Vector3 pos = new Vector3(Mathf.Sin (angle), 0, Mathf.Cos (angle)) * radius;
			Instantiate (prefab, pos, Quaternion.identity);

		}
		cubes = GameObject.FindGameObjectsWithTag ("cubes");
	}
	
	// Update is called once per frame
	void Update () {
		float[] spectrum = AudioListener.GetSpectrumData (1024, 0, FFTWindow.Hamming);
		for (int i = 0; i < numberofObjects; i++) {
			Vector3 previousScale = cubes [i].transform.localScale;
			previousScale.y = spectrum [i] * 20;
			cubes [i].transform.localScale = previousScale;
		}
		
	}
}
