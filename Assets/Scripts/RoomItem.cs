using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;
    NetworkingManager networkingManager;

    private void Start()
    {
        networkingManager = FindObjectOfType<NetworkingManager>();
    }
    public void SetRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }

    public void OnClickItem()
    {
        networkingManager.JoinRoom(roomName.text);
    }
}
