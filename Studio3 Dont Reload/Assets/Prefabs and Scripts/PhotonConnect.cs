using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonConnect : Photon.PunBehaviour
{

    public Text myText;
    public Button Connect;
    const string VERSION = "v0.0.1";
    RoomOptions myRoomDetails;
    private bool isConnected = false;

    public RoomInfo[] roomInformations;
    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.automaticallySyncScene = true;
        Connect.interactable = false;
        myRoomDetails = new RoomOptions();
        myRoomDetails.IsVisible = true;
        myRoomDetails.maxPlayers = 2;

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
            myText.text = "Connecting to a room";
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

   public void LobbyConnection()
    {
        isConnected = true;
        //   PhotonNetwork.playerName = myusername;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(VERSION);
        }
    }

    public override void OnConnectedToMaster()
    {
        if (isConnected)
        {
            // lobbymenue
            PhotonNetwork.JoinLobby();
        }
    }

    public void RefreshButton()
    {
        //textfield.text = "";
        roomInformations = PhotonNetwork.GetRoomList();
        for(int i = 0; i < roomInformations.Length; i++)
        {
            // text field . text += roominformations[i].name + "    " + rooom infrmaions[i].playercount + system.enviroment.newline

        }

    }

    public void newConnect()
    {

    }

    public void newCreateRoom()
    {

    }
}
