using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiPlayer;

public class Launcher : MonoBehaviour, MultiPlayCallback
{
    public Dictionary<string, GameObject> playerData = new Dictionary<string, GameObject>();
    //public Dictionary<Dictionary<int, GameObject>, Dictionary<int, GameObject>>[] playerData;

    public GameObject playerObj;
    public GameObject ballObj;
    public PhotonClientImpl client;

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
            var localPlayer = playerData[userID];
            localPlayer.transform.Translate(newLocation);
        }
    }

    public void onConnectedServer()
    {
        MultiPlayManager.Instance.createOrJoinRoom(roomName: "Ball Test");
    }

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
}
