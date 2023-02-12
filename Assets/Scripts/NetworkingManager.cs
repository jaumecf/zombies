using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkingManager : MonoBehaviourPunCallbacks
{

    public Button multiplayerButton;
    
    void Start()
    {
        Debug.Log("Connexió a un servidor");

        // Connexió amb el servidor
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Unir-mos a un Lobby");
        PhotonNetwork.JoinLobby();
        //base.OnConnectedToMaster();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Estam apunt per jugar!");
        multiplayerButton.interactable = true;
        //base.OnJoinedLobby();
    }

    public void FindMatch()
    {
        Debug.Log("Cercant una sala");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        MakeRoom();
        //base.OnJoinRandomFailed(returnCode, message);
    }

    private void MakeRoom()
    {
        int randomRoomName = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true
        };
        PhotonNetwork.CreateRoom($"RoomName_{randomRoomName}", roomOptions);
        Debug.Log($"Sala creada{randomRoomName}");

    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();
        Debug.Log("Carregant escena del joc");
        // Cream una nova escena, còpia de Game, i li deim "Game Online" amb codi d'escena 2
        //SceneManager.LoadScene(2);
        // Ara ja no carregar l'escena en local per aquest client,
        // sinò que carregam la nova escena des del Photon Network
        PhotonNetwork.LoadLevel(1);
    }
}
