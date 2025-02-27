using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomField;
    public RoomInfo room;
    NetworkingManager networkingManager;

    private void Start()
    {
        networkingManager = FindObjectOfType<NetworkingManager>();
    }
    public void SetRoomName(RoomInfo room)
    {   
        this.room=room;
        roomField.text = room.Name+"\t "+room.PlayerCount+"/6";
    }

    public void OnClickItem()
    {
        networkingManager.JoinRoom(room.Name);
        Debug.Log("Unit a la sala des de RoomItem: "+room.Name);
    }
}
