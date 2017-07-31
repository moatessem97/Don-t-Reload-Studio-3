using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;


public class Killzone : PunBehaviour {

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject player;

        if(col.tag == "Player")
        {
            Debug.Log("Entered Zone " + col.name);
            player = col.transform.root.gameObject;
            player.GetComponent<Platformer2DUserControl>().photonView.RPC("Damaged", PhotonTargets.All, 60, 30);
        }
    }
}
