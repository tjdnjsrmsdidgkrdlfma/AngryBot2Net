using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    readonly string version = "1.0";
    string user_id = "Zack";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //씬 자동 동기화 옵션
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = user_id;

        Debug.Log(PhotonNetwork.SendRate); //데이터 초당 전송 횟수

        PhotonNetwork.ConnectUsingSettings();
    }

    //포톤 서버에 접속 후 호출되는 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    //로비에 접속 후 호출되는 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }

    //랜덤한 룸 입장이 실패했을 경우 호출되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 20;
        room.IsOpen = true;
        room.IsVisible = true;

        PhotonNetwork.CreateRoom("MyRoom", room);
    }
    
    //룸 생성이 완료된 후 호출되는 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    //룸에 입장한 후 호출되는 함수
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = UnityEngine.Random.Range(0, points.Length);

        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);
    }
}
