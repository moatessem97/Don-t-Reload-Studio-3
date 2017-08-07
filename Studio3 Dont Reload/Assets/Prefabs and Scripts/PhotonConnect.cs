using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonConnect : Photon.PunBehaviour
{

    public Text myText,LobbyText;
    public Button Connect;
    const string VERSION = "v0.0.1";
    RoomOptions myRoomDetails;
    private bool isConnected = false;
    public Text myUsername,createOrJoinRoomText,joinRoomText;
    public GameObject LoginCanvas, LobbyCanvas;

    public RoomInfo[] roomInformations;
    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(VERSION);
        PhotonNetwork.automaticallySyncScene = true;
        Connect.interactable = false;
        myRoomDetails = new RoomOptions() { isVisible = true, maxPlayers = 2 };

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
            //RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 2 };
            PhotonNetwork.JoinOrCreateRoom(createOrJoinRoomText.text, myRoomDetails, TypedLobby.Default);
            Debug.Log("Room Created");
            RefreshButton();
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.playerList.Length == myRoomDetails.maxPlayers && PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsOpen = false;
            PhotonNetwork.LoadLevel("MainScene");
        }

    }

   public void LobbyConnection()
    {
        PhotonNetwork.playerName = myUsername.text;
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinLobby();
            myText.text = "Joined Lobby";
            LobbyCanvas.SetActive(true);
            LoginCanvas.SetActive(false);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(VERSION);
        }
    }

    public void RefreshButton()
    {
        LobbyText.text = "";
        roomInformations = PhotonNetwork.GetRoomList();
        for(int i = 0; i < roomInformations.Length; i++)
        {
            LobbyText.text += roomInformations[i].Name + "          " + roomInformations[i].PlayerCount + System.Environment.NewLine;
        }
        Debug.Log("refreshed");
    }
}
