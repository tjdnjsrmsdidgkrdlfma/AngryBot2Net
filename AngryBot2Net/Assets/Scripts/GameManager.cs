using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text room_name;
    public TMP_Text connect_info;
    public TMP_Text msg_list;

    public Button exit_btn;

    void Awake()
    {
        CreatePlayer();
        SetRoomInfo();

        exit_btn.onClick.AddListener(() => OnExitClick());
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(0, points.Length);

        PhotonNetwork.Instantiate("Player",
                                  points[idx].position,
                                  points[idx].rotation,
                                  0);
    }

    void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        room_name.text = room.Name;
        connect_info.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }

    void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerEnteredRoom(Player new_player)
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{new_player.NickName}</color> is joined room";
        msg_list.text += msg;
    }

    public override void OnPlayerLeftRoom(Player other_player)
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{other_player.NickName}</color> is left room";
        msg_list.text += msg;
    }
}
