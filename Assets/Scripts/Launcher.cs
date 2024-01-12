using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiPlayer;

public class Launcher : MonoBehaviour, MultiPlayCallback
{
    public Dictionary<string, GameObject> playerData = new Dictionary<string, GameObject>();
    //public Dictionary<string, GameObject> ballData = new Dictionary<string, GameObject>();
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

    public void onRoomJoined(RoomInfo roomInfo) {
        generateGameObj();
        generateBallObj(roomInfo: roomInfo);
    }

    private void generateGameObj()
    {
        playerData.Clear();
        var playerList = MultiPlayManager.Instance.getPlayerList();
        Debug.Log("PlayerList : " + playerList.Count);
        
        foreach (var player in playerList)
        {
            Debug.Log("generateGameObj : " + player.userID);
            

            playerData[player.userID] = createPlayer(playerID: player.userID, location: player.location);
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

    private void generateBallObj(RoomInfo roomInfo)
    {
        RoomInfo room = roomInfo;
        Debug.Log("generateBallObj : " + room);
        if (MultiPlayManager.Instance.isHostPlayer())
        {
            room = new RoomInfo();

            for (int index = 0; index < 2; index++)
            {
                var itemID = Guid.NewGuid().ToString();
                var roomItem = new RoomItem(
                    itemID: itemID,
                    itemTypeID: "ball",
                    location: new Vector3(index, 1, index),
                    rotation: new Quaternion(),
                    currentVelocity: Vector3.zero,
                    currentAngularVelocity: Vector3.zero
                    );
                room.roomItemList.Add(roomItem);
            }
        }
        if (room != null)
        {
            foreach(var item in room.roomItemList)
            {
                createBallObj(item: item);
            }
        }
    }

    private GameObject createBallObj(RoomItem item)
    {
        var obj = Instantiate(ballObj, item.location, item.rotation);
        var ballScript = obj.GetComponent<BallTest>();
        ballScript.itemID = item.itemID;
        ballScript.setCurrentState(velocity: item.currentVelocity, angularVelocity: item.currentAngularVelocity);

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
