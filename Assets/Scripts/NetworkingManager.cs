using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField userNameInput;
    public TMP_Text statusText;

    public GameObject roomOptionsPanel;
    public GameObject playersListPanel;

    public Button multiplayerButton;
    public Button leaveRoom;

    public TMP_InputField roomNameInput;
    public RoomItem roomItemPrefab;
    public List<RoomItem> roomList = new List<RoomItem>();
    public Transform contentRooms;

    public TMP_Text playerItemPrefab;
    public Transform contentPlayers;
    
    void Start()
    {
        roomOptionsPanel.SetActive(false);
        playersListPanel.SetActive(false);
        leaveRoom.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(DisconnectPlayer());
        }
    }

    IEnumerator DisconnectPlayer()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
            yield return null;
    }

    public void OnClickConnect()
    {
        if(userNameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = userNameInput.text;
            statusText.text = "Connecting...";
            Debug.Log("Connexió a un servidor");
            // Connexió amb el servidor
            PhotonNetwork.ConnectUsingSettings();
            // Sincronitzar escenes del master amb tots els clients
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        roomOptionsPanel.SetActive(true);
        statusText.text = "Connected";
        Debug.Log("Unir-mos a un Lobby");
        PhotonNetwork.JoinLobby();
        //base.OnConnectedToMaster();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Estam apunt per jugar!");
        //multiplayerButton.interactable = true;
    }

    // Connexió a una sala!

    public void OnClickCreate()
    {
        if (roomNameInput.text.Length >= 1)
        {
            MakeRoom(roomNameInput.text);
        }
        // Esborram el contingut
        roomNameInput.text = "";
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }
    private void ClearRoomList()
    {
        foreach (RoomItem item in roomList)
        {
            DestroyImmediate(item.gameObject);
        }
        roomList.Clear();
    }

    private void UpdateRoomList(List<RoomInfo> list)
    {
        ClearRoomList();
        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentRooms);
            newRoom.SetRoomName(room.Name+room.PlayerCount+"/6");
            roomList.Add(newRoom);
        }
    }

    /*
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
    */

    private void MakeRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true
        };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Sala creada: {roomName}");

    }

    public void JoinRoom(string roomName)
    {
        Debug.Log("Joined Room");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        //TODO: Eliminar room que hem creat de la lliusta (ens apareix buida)
        ClearRoomList();
        playersListPanel.SetActive(false);
        leaveRoom.interactable = false;
    }
    public override void OnJoinedRoom()
    {
        leaveRoom.interactable = true;
        ClearRoomList();
        statusText.text = $"On room: {PhotonNetwork.CurrentRoom.Name}";
        playersListPanel.SetActive(true);
        UpdatePlayerList();

        //Debug.Log("Carregant escena del joc");
        // Cream una nova escena, còpia de Game, i li deim "Game Online" amb codi d'escena 2
        //SceneManager.LoadScene(2);
        // Ara ja no carregar l'escena en local per aquest client,
        // sinò que carregam la nova escena des del Photon Network
        //PhotonNetwork.LoadLevel(2);
        multiplayerButton.interactable = true;
    }
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void UpdatePlayerList()
    {
        //TODO: clear and update List
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            TMP_Text newPlayer = Instantiate(playerItemPrefab, contentPlayers);
            newPlayer.text = p.NickName;
        }
    }

    public void LoadMPGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    public void LoadMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
}
