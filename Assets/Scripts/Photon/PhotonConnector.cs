using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class PhotonConnector : MonoBehaviourPunCallbacks
{
    string Username;
    private void ConnectToPhoton()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues(Username);
        PhotonNetwork.NickName = Username;
        PhotonNetwork.ConnectUsingSettings();
        Player[] a = PhotonNetwork.PlayerList;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("You are now connected to the Server");
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
}
