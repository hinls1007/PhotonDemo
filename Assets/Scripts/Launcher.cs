using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiPlayer;

public class Launcher : MonoBehaviour, MultiPlayCallback
{
    public Dictionary<string, GameObject> playerData = new Dictionary<string, GameObject>();
    //public Dictionary<Dictionary<int, GameObject>, Dictionary<int, GameObject>>[] playerData;

    public GameObject playerObj;
<<<<<<< HEAD

    private string PlayerLocation = "Player_Location";
    private string PlayerRotation = "Player_Rotation";

=======
    public GameObject ballObj;
    public PhotonClientImpl client;
>>>>>>> a2b970875aa978ba20bca89d6861e55bf868934c

    private void Start()
    {
        MultiPlayManager.init(client);
        MultiPlayManager.Instance.registerCallback(this);
        MultiPlayManager.Instance.connectServer();
    }

    private void FixedUpdate()
    {
        var userID = MultiPlayManager.Instance.getUserID();
        if (userID != null
            && playerData.ContainsKey(userID))
        {
            float x = Input.GetAxis("Horizontal") * 10f * Time.deltaTime;
            float z = Input.GetAxis("Vertical") * 10f * Time.deltaTime;
            var newLocation = new Vector3(x, 0, z);
<<<<<<< HEAD


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
=======
            var localPlayer = playerData[userID];
            localPlayer.transform.Translate(newLocation);
>>>>>>> a2b970875aa978ba20bca89d6861e55bf868934c
        }
    }

    public void onConnectedServer()
    {
        MultiPlayManager.Instance.createOrJoinRoom(roomName: "Ball Test");
    }

<<<<<<< HEAD
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
=======
    public void onRoomJoined() {
        generateGameObj();
    }

    private void generateGameObj()
    {
        playerData.Clear();
        var playerList = MultiPlayManager.Instance.getPlayerList();
        
        foreach (var player in playerList)
        {
            Debug.Log("generateGameObj : " + player.userID);
>>>>>>> a2b970875aa978ba20bca89d6861e55bf868934c

            playerData[player.userID] = createPlayer(playerID: player.userID, location: new Vector3());
        }
    }

    private GameObject createPlayer(string playerID, Vector3 location, Quaternion rotation)
    {
        Debug.Log($"playerID:{playerID}, location:{location}");
        var obj = Instantiate(playerObj, location, rotation);
        var playerScript = obj.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.playerID = playerID;
        }
        return obj;
    }

<<<<<<< HEAD
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
=======
    public void onPlayerJoinedRoom(PlayerInfo player) {
        playerData[player.userID] = createPlayer(playerID: player.userID, location: player.location);
    }

    public void onPlayerLeftRoom(string userID) {
        var playerObj = playerData[userID];
>>>>>>> a2b970875aa978ba20bca89d6861e55bf868934c
        Destroy(playerObj);

        playerData.Remove(userID);
    }
}
