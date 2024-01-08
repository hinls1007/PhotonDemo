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
            properties[PlayerLocation] = newLocation;
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
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

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("OnJoinedRoom");
        generateGameObj();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    private void generateGameObj()
    {
        playerData.Clear();
        var playerList = PhotonNetwork.PlayerList;
        foreach (var player in playerList)
        {
            var properties = player.CustomProperties;
            Vector3 location = new Vector3();
            properties.TryGetValue(PlayerLocation, out location);

            playerData[player.UserId] = createPlayer(playerID: player.UserId, location: location);


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

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (targetPlayer.UserId != PhotonNetwork.LocalPlayer.UserId
            && playerData.ContainsKey(targetPlayer.UserId))
        {
            Vector3 location = new Vector3();
            changedProps.TryGetValue(PlayerLocation, out location);

            var playerObj = playerData[targetPlayer.UserId];
            playerObj.transform.Translate(location);
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
