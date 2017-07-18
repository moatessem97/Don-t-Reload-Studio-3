﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class myNetworkManager : PunBehaviour
{
    public GameObject[] spawns;
    private string Charr;

    private void Awake()
    {
        PlayerPrefs.SetString("Charr", "Player2Done");
    }
    // Use this for initialization
    void Start()
    {
        Charr = PlayerPrefs.GetString("Charr");
        PhotonNetwork.Instantiate(Charr, spawns[PhotonNetwork.player.ID - 1].transform.position, Quaternion.identity, 0);
    }

    public void menuButton()
    {
        OnLeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }

    private void OnLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}