using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


public class Killzone : PunBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject player;
        if(col.tag == "Player")
        {
            Debug.Log("Entered Zone " + col.name);
            player = col.transform.root.gameObject;
            
        }
    }
}
