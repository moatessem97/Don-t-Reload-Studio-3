using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class myNetworkManager : PunBehaviour
{
    public GameObject[] spawns;
    public string[] Charrs;
    private string Charr0, Charr1, Charr2, Charr3;

    private void Awake()
    {
        PlayerPrefs.SetString("Charr0", "PlayerBlue");
        PlayerPrefs.SetString("Charr1", "PlayerGrey");
        PlayerPrefs.SetString("Charr2", "PlayerBlue");
        PlayerPrefs.SetString("Charr3", "PlayerGrey");
    }
    // Use this for initialization
    void Start()
    {
        Charr0 = PlayerPrefs.GetString("Charr0");
        Charr1 = PlayerPrefs.GetString("Charr1");
        Charr2 = PlayerPrefs.GetString("Charr2");
        Charr3 = PlayerPrefs.GetString("Charr3");
        Charrs[0] = PlayerPrefs.GetString("Charr0");
        Charrs[1] = PlayerPrefs.GetString("Charr1");
        Charrs[2] = PlayerPrefs.GetString("Charr2");
        Charrs[3] = PlayerPrefs.GetString("Charr3");
        PhotonNetwork.Instantiate(Charrs[PhotonNetwork.player.ID - 1], spawns[PhotonNetwork.player.ID - 1].transform.position, Quaternion.identity, 0);
    }

    public void menuButton()
    {
        OnLeaveRoom();
        PhotonNetwork.LoadLevel(1);
    }

    public void OnLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}