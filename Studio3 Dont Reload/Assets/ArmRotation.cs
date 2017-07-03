using UnityEngine;
using System.Collections;

public class ArmRotation : Photon.PunBehaviour
{	
	public int rotationOffset = 90;
    private GameObject arm;
    private void Start()
    {
        arm = gameObject.transform.GetChild(3).gameObject;
    }
    // Update is called once per frame
    void Update ()
	{
        if (!photonView.isMine)
        {
            return;
        }
        // subtracting the position of the player from the mouse position
        Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - transform.position;
		difference.Normalize ();	// normalizing the vector.  Meaning that all sums of the vector will be equal to 1.

		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;	// find the angle in degrees
		arm.transform.rotation = Quaternion.Euler (0f, 0f, rotZ + this.rotationOffset);
	}
}
