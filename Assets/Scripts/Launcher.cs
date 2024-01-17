using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon.StructWrapping;

public delegate void SuccessCallback(string name);
public delegate void FailCallback(int errorCode, string errorMessage);
public delegate void DisconnectedCallback(string message);
public delegate void PlayerCallback(string player);

public class Launcher : MonoBehaviourPunCallbacks
{

    //public DictionaryData DictData;

    public Dictionary<string, GameObject> playerData = new Dictionary<string, GameObject>();
    //public Dictionary<Dictionary<int, GameObject>, Dictionary<int, GameObject>>[] playerData;

    public GameObject playerObj;
    public Rigidbody playerRb;

    private string PlayerLocation = "Player_Location";
    private string PlayerRotation = "Player_Rotation";

    public float movementSpeed = 10f;

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
            if (Input.GetKey(KeyCode.A))
                playerRb.AddForce(new Vector3(-movementSpeed, 0, 0));
            if (Input.GetKey(KeyCode.D))
                playerRb.AddForce(new Vector3(movementSpeed, 0, 0));
            if (Input.GetKey(KeyCode.W))
                playerRb.AddForce(new Vector3(0, 0, movementSpeed));
            if (Input.GetKey(KeyCode.S))
                playerRb.AddForce(new Vector3(0, 0, -movementSpeed));

            var newLocation = playerObj.transform.position;


            float rotationInput = 0f;
            if (Input.GetKey(KeyCode.Q))
            {
                rotationInput = 1f;
            }

            if (Input.GetKey(KeyCode.E))
            {
                rotationInput = -1f;
            }

            Quaternion newRotation = Quaternion.Euler(0f, rotationInput * 10f * Time.deltaTime, 0f);


            var localPlayer = playerData[PhotonNetwork.LocalPlayer.UserId];
            localPlayer.transform.Translate(newLocation);
            localPlayer.transform.Rotate(newRotation.eulerAngles);

            var properties = PhotonNetwork.LocalPlayer.CustomProperties;
            properties[PlayerLocation] = localPlayer.transform.position;
            properties[PlayerRotation] = localPlayer.transform.rotation;

            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

            Debug.Log($"Player:{PhotonNetwork.LocalPlayer.UserId}, location:{localPlayer.transform.position}, rotation:{localPlayer.transform.rotation} properties:{properties[PlayerLocation]}");
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
            Quaternion rotation = new Quaternion();

            properties.TryGetValue(PlayerLocation, out location);
            properties.TryGetValue(PlayerRotation, out rotation);

            Debug.Log($"player detail:{player}, location:{location}, rotation:{rotation}");

            playerData[player.UserId] = createPlayer(playerID: player.UserId, location: location, rotation:rotation);

        }
    }

    private GameObject createPlayer(string playerID, Vector3 location, Quaternion rotation)
    {
        Debug.Log($"playerID:{playerID}, location:{location}");
        var obj = Instantiate(playerObj, location, rotation);
        var playerScript = obj.GetComponent<Player>();
        playerRb = obj.GetComponent<Rigidbody>();

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
            Quaternion rotation = new Quaternion();

            changedProps.TryGetValue(PlayerLocation, out location);
            changedProps.TryGetValue(PlayerRotation, out rotation);

            var playerObj = playerData[targetPlayer.UserId];
            //playerObj.transform.Translate(location);
            playerObj.transform.SetPositionAndRotation(location, rotation);

            Debug.Log(changedProps);

            
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        var properties = newPlayer.CustomProperties;
        Vector3 location = new Vector3();
        Quaternion rotation = new Quaternion();

        properties.TryGetValue(PlayerLocation, out location);
        properties.TryGetValue(PlayerLocation, out rotation);

        playerData[newPlayer.UserId] = createPlayer(playerID: newPlayer.UserId, location: location, rotation:rotation);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        var playerObj = playerData[otherPlayer.UserId];
        Destroy(playerObj);

        playerData.Remove(otherPlayer.UserId);
    }
}
