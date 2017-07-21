using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonConnect : Photon.PunBehaviour
{

    public Button Connect;
    const string VERSION = "v0.0.1";
    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.automaticallySyncScene = true;
        Connect.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.connected)
        {
            Debug.Log("Trying to connect");
            Connect.interactable = false;
        }
        else
        {
            Debug.Log("Done!");
            Connect.interactable = true;
        }
    }

    public void ConnectToRoom()
    {
        if (PhotonNetwork.connected)
        {
            Debug.Log("Connecting to a room");
            RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 2 };
            PhotonNetwork.JoinOrCreateRoom("Room1", roomOptions, TypedLobby.Default);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.playerList.Length == 2 && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.LoadLevel("MainScene");
        }
    }
}
