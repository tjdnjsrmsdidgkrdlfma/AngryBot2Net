using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    RoomInfo room_info;
    TMP_Text room_info_text;
    PhotonManager photon_manager;

    public RoomInfo RoomInfo
    {
        get { return room_info; }
        set
        {
            room_info = value;
            room_info_text.text = $"{room_info.Name} ({room_info.PlayerCount}/{room_info.MaxPlayers})";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(room_info.Name));
        }
    }

    void Awake()
    {
        room_info_text = GetComponentInChildren<TMP_Text>();
        photon_manager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    void OnEnterRoom(string room_name)
    {
        photon_manager.SetUserId();

        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 20;
        room.IsOpen = true;
        room.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(room_name, room, TypedLobby.Default);
    }
}
