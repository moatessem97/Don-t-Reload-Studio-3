using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditSceneExit : MonoBehaviour {

	void Update () {
		if(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Application.LoadLevel("Menu");
        }
	}
}
