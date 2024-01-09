using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public class Launcher : MonoBehaviourPunCallbacks
{

    //public DictionaryData DictData;

    public Dictionary<string, GameObject> playerData = new Dictionary<string, GameObject>();
    //public Dictionary<Dictionary<int, GameObject>, Dictionary<int, GameObject>>[] playerData;

    public GameObject playerObj;

    private string PlayerLocation = "Player_Location";

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        //Hashtable props = new Hashtable();
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void FixedUpdate()
    {
        if (PhotonNetwork.LocalPlayer.UserId != null
            && playerData.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
        {
            float x = Input.GetAxis("Horizontal") * 10f * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * 10f * Time.deltaTime;
            var newLocation = new Vector3(x, 0, z);


            var localPlayer = playerData[PhotonNetwork.LocalPlayer.UserId];
            localPlayer.transform.Translate(newLocation);

            var properties = PhotonNetwork.LocalPlayer.CustomProperties;
            properties[PlayerLocation] = localPlayer.transform.position;

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

            Debug.Log($"Player:{PhotonNetwork.LocalPlayer.UserId}, location:{localPlayer.transform.position}, properties:{properties[PlayerLocation]}");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5;
        options.PublishUserId = true;
        options.CleanupCacheOnLeave = true;
        PhotonNetwork.JoinOrCreateRoom("RacingRoom", options, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("OnCreatedRoom failed");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("OnJoinedRoom");

        generateGameObj();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("OnJoinRandomFailed failed");
    }

    

    private void generateGameObj()
    {
        //playerData.Clear();
        var playerList = PhotonNetwork.PlayerList;

        Debug.Log("Player count: " + playerList.Length);

        foreach (var player in playerList)
        {
            Debug.Log("Player ID: " + player.ActorNumber);

            var properties = player.CustomProperties;
            Debug.Log(properties);
            Vector3 location = new Vector3();

            properties.TryGetValue(PlayerLocation, out location);

            Debug.Log($"player detail:{player}, location:{location}");

            playerData[player.UserId] = createPlayer(playerID: player.UserId, location: location);

        }
    }

    private GameObject createPlayer(string playerID, Vector3 location)
    {
        Debug.Log($"playerID:{playerID}, location:{location}");
        var obj = Instantiate(playerObj, location, new Quaternion());
        var playerScript = obj.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.playerID = playerID;
        }
        return obj;
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer.UserId != PhotonNetwork.LocalPlayer.UserId
            && playerData.ContainsKey(targetPlayer.UserId))
        {
            Debug.Log($"OnPlayerPropertiesUpdate player:{targetPlayer.UserId}");

            Vector3 location = new Vector3();
            changedProps.TryGetValue(PlayerLocation, out location);

            var playerObj = playerData[targetPlayer.UserId];
            //playerObj.transform.Translate(location);
            playerObj.transform.SetPositionAndRotation(location, new Quaternion());

            Debug.Log(changedProps);

            
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);


        var properties = newPlayer.CustomProperties;
        Vector3 location = new Vector3();
        properties.TryGetValue(PlayerLocation, out location);

        playerData[newPlayer.UserId] = createPlayer(playerID: newPlayer.UserId, location: location);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        var playerObj = playerData[otherPlayer.UserId];
        Destroy(playerObj);

        playerData.Remove(otherPlayer.UserId);
    }
}
