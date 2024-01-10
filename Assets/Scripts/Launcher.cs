using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Photon.Pun;
//using Photon.Realtime;
//using ExitGames.Client.Photon.StructWrapping;

using MultiPlayer;

public class Launcher : MonoBehaviour, MultiPlayCallback
{

    //public DictionaryData DictData;

    public Dictionary<string, GameObject> playerData = new Dictionary<string, GameObject>();
    //public Dictionary<Dictionary<int, GameObject>, Dictionary<int, GameObject>>[] playerData;

    public GameObject playerObj;
    public GameObject ballObj;
    public PhotonClientImpl client;

    private string PlayerLocation = "Player_Location";

    private void Start()
    {
        MultiPlayManager.init(client);
        MultiPlayManager.Instance.registerCallback(this);
        MultiPlayManager.Instance.connectServer();
        //PhotonNetwork.ConnectUsingSettings();

        //Hashtable props = new Hashtable();
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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
            var localPlayer = playerData[userID];
            localPlayer.transform.Translate(newLocation);
        }
        //if (PhotonNetwork.LocalPlayer.UserId != null
        //    && playerData.ContainsKey(PhotonNetwork.LocalPlayer.UserId))
        //{
        //    float x = Input.GetAxis("Horizontal") * 10f * Time.deltaTime;
        //    float z = Input.GetAxis("Vertical") * 10f * Time.deltaTime;
        //    var newLocation = new Vector3(x, 0, z);
        //    var localPlayer = playerData[PhotonNetwork.LocalPlayer.UserId];
        //    localPlayer.transform.Translate(newLocation);

        //    var properties = PhotonNetwork.LocalPlayer.CustomProperties;
        //    properties[PlayerLocation] = newLocation;
        //    PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        //}
    }

    public void onConnectedServer()
    {
        MultiPlayManager.Instance.createOrJoinRoom(roomName: "Ball Test");
    }

    public void onRoomJoined() {
        generateGameObj();
    }
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("Connected to Master");
    //    RoomOptions options = new RoomOptions();
    //    options.MaxPlayers = 5;
    //    options.PublishUserId = true;
    //    options.CleanupCacheOnLeave = true;
    //    PhotonNetwork.JoinOrCreateRoom("RacingRoom", options, TypedLobby.Default);
    //}

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    Debug.Log("OnJoinedRoom");
    //    generateGameObj();
    //}

    //public override void OnCreatedRoom()
    //{
    //    base.OnCreatedRoom();
    //    Debug.Log("OnCreatedRoom");
    //}

    private void generateGameObj()
    {
        playerData.Clear();
        var playerList = MultiPlayManager.Instance.getPlayerList();
        
        foreach (var player in playerList)
        {
            Debug.Log("generateGameObj : " + player.userID);

            playerData[player.userID] = createPlayer(playerID: player.userID, location: new Vector3());


        }
    }

    private GameObject createPlayer(string playerID, Vector3 location)
    {
        var obj = Instantiate(playerObj, location, new Quaternion());
        var playerScript = obj.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.playerID = playerID;
        }
        return obj;
    }

    public void onPlayerJoinedRoom(PlayerInfo player) {
        playerData[player.userID] = createPlayer(playerID: player.userID, location: player.location);
    }

    public void onPlayerLeftRoom(string userID) {
        var playerObj = playerData[userID];
        Destroy(playerObj);

        playerData.Remove(userID);
    }

    //public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    //{
    //    base.OnPlayerEnteredRoom(newPlayer);

    //    var properties = newPlayer.CustomProperties;
    //    Vector3 location = new Vector3();
    //    properties.TryGetValue(PlayerLocation, out location);

    //    playerData[newPlayer.UserId] = createPlayer(playerID: newPlayer.UserId, location: location);
    //}

    //public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    //{
    //    base.OnPlayerLeftRoom(otherPlayer);

    //    var playerObj = playerData[otherPlayer.UserId];
    //    Destroy(playerObj);

    //    playerData.Remove(otherPlayer.UserId);
    //}
}
