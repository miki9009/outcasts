using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.MonoBehaviour
{
    public static bool IsMultiplayer { get; private set; }

    public int spawnPointIndex;
    public string version = "1.0.0";
    public string roomName = "Room";
    public string playerPrefabName = "Cube";
    public Transform spawnPoint;

    private void Awake()
    {
        IsMultiplayer = true;
    }

    private void OnDestroy()
    {
        IsMultiplayer = false;
    }

    private void Start()
    {
        if (PhotonNetwork.ConnectUsingSettings(version))
        {
            Debug.Log("Connected");
        }
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = 4 };
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        PhotonNetwork.Instantiate(playerPrefabName, spawnPoint.position, spawnPoint.rotation, 0);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //print ("writing");
            //stream.SendNext(transform.position);
            //stream.SendNext(transform.rotation);
            Debug.Log("Send");
        }
        else
        {

        }
    }

}
