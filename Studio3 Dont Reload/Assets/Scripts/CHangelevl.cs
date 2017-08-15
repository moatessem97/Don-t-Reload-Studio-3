using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHangelevl : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.LoadLevel(Application.loadedLevel - 1);
        }
	}
}
