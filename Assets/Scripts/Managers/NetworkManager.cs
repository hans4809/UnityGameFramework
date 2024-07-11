using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager _instance = null;
    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();
            }
            return _instance;
        }
    }

    private string _nickName = "DefaultPlayer";
    public string NickName
    {
        get { return _nickName; }
        set
        {
            _nickName = value;
            PhotonNetwork.NickName = _nickName;
        }
    }

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        GameObject go = GameObject.Find("@NetworkManager");
        if (go == null)
        {
            go = new GameObject { name = "@NetworkManager" };
            go.AddComponent<NetworkManager>();
        }
        DontDestroyOnLoad(go);
        _instance = go.GetComponent<NetworkManager>();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnConnected()
    {
        Debug.Log("Called OnConnected");
        base.OnConnected();
        Debug.Log("Finished OnConnected");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Called OnConnectedToMaster");
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
        Debug.Log("Finished OnConnectedToMaster");
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Called OnJoinedLobby");
        base.OnJoinedLobby();

        //Managers.Scene.LoadScene(Define.Scene.Lobby);
        Debug.Log("Finished OnJoinedLobby");
    }

    public void CreateRoom(string roomName,int MaxPlayer, bool isVisible = true, bool isOpen = true)
    {
        RoomOptions ro = new RoomOptions { IsVisible = isVisible, IsOpen = isOpen, MaxPlayers = MaxPlayer };
        
        PhotonNetwork.CreateRoom(roomName, ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Called OnCreatedRoom");
        base.OnCreatedRoom();
        Debug.Log("Finished OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Called OnCreateRoomFailed");
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log($"OnCreatedRoomFailed, {returnCode}, {message}");
        Debug.Log("Finished OnCreateRoomFailed");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Called OnJoinedRoom");
        base.OnJoinedRoom();
        Debug.Log("Finished OnJoinedRoom");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Called OnJoinRoomFailed");
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log($"OnJoinRoomFailed, {returnCode}, {message}");
        Debug.Log("Finished OnJoinRoomFailed");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
