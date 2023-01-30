using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    readonly string version = "1.0";
    string user_id = "Zack";

    public TMP_InputField user_input;
    public TMP_InputField room_input;

    Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    GameObject room_item_prefab;
    public Transform scrool_content;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //�� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.GameVersion = version;
        //PhotonNetwork.NickName = user_id;

        Debug.Log(PhotonNetwork.SendRate); //������ �ʴ� ���� Ƚ��

        room_item_prefab = Resources.Load<GameObject>("RoomItem");

        if(PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        user_id = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(0, 21):00}");
        user_input.text = user_id;
        PhotonNetwork.NickName = user_id;
    }

    public void SetUserId()
    {
        if (string.IsNullOrEmpty(user_input.text))
        {
            user_id = $"USER_{Random.Range(0, 21):00}";
        }
        else
        {
            user_id = user_input.text;
        }

        PlayerPrefs.SetString("USER_ID", user_id);
        PhotonNetwork.NickName = user_id;
    }

    string SetRoomName()
    {
        if (string.IsNullOrEmpty(room_input.text))
        {
            room_input.text = $"ROOM_{Random.Range(0, 101):000}";
        }

        return room_input.text;
    }

    //���� ������ ���� �� ȣ��Ǵ� �Լ�
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master!");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    //�κ� ���� �� ȣ��Ǵ� �Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom();
    }

    //������ �� ������ �������� ��� ȣ��Ǵ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode}:{message}");

        OnMakeRoomClick();

        //RoomOptions room = new RoomOptions();
        //room.MaxPlayers = 20;
        //room.IsOpen = true;
        //room.IsVisible = true;

        //PhotonNetwork.CreateRoom("MyRoom", room);
    }

    //�� ������ �Ϸ�� �� ȣ��Ǵ� �Լ�
    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    //�뿡 ������ �� ȣ��Ǵ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        //Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        //int idx = UnityEngine.Random.Range(0, points.Length);

        //PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }

    }

    public override void OnRoomListUpdate(List<RoomInfo> room_list)
    {
        GameObject temp_room = null;

        foreach(var room_info in room_list)
        {
            if(room_info.RemovedFromList == true) //���� ������ ���
            {
                rooms.TryGetValue(room_info.Name, out temp_room); 

                Destroy(temp_room);

                rooms.Remove(room_info.Name);
            }
            else //���� �����ǰų� ����� ���
            {
                if(rooms.ContainsKey(room_info.Name) == false) //���� ��Ͽ� ���� ��� ���� �߰�
                {
                    GameObject room_prefab = Instantiate(room_item_prefab, scrool_content);
                    room_prefab.GetComponent<RoomData>().RoomInfo = room_info;

                    rooms.Add(room_info.Name, room_prefab);
                }
                else //���� ��Ͽ� �ִ� ��� ����
                {
                    rooms.TryGetValue(room_info.Name, out temp_room);
                    temp_room.GetComponent<RoomData>().RoomInfo = room_info;
                }
            }

            Debug.Log($"Room={room_info.Name} ({room_info.PlayerCount}/{room_info.MaxPlayers})");
        }
    }

    #region UI_BUTTON_EVENT

    public void OnLoginClick()
    {
        SetUserId();

        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        SetUserId();

        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 20;
        room.IsOpen = true;
        room.IsVisible = true;

        PhotonNetwork.CreateRoom(SetRoomName(), room);
    }

    #endregion
}
