using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

using MultiPlayer;

public class Launcher : MonoBehaviour, MultiPlayCallback
{
    public CinemachineVirtualCamera camera;


    public Dictionary<string, Player> playerData = new Dictionary<string, Player>();
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

    struct Test
    {
        public Vector3 location;
        public Test(Vector3 location)
        {
            this.location = location;
        }
    }

    private void FixedUpdate()
    {
        
    }

    public void onConnectedServer()
    {
        MultiPlayManager.Instance.createOrJoinRoom(roomName: "Ball Test");
    }

    public void onRoomJoined(RoomInfo roomInfo) {
        generateGameObj();
        generateBallObj(roomInfo: roomInfo);
        if (!MultiPlayManager.Instance.isHostPlayer())
        {
            Physics.IgnoreLayerCollision(6, 7, true);
            Physics.IgnoreLayerCollision(6, 6, true);
            Physics.IgnoreLayerCollision(7, 7, true);
        }
    }

    private void generateGameObj()
    {
        playerData.Clear();
        var playerList = MultiPlayManager.Instance.getPlayerList();
        Debug.Log("PlayerList : " + playerList.Count);
        
        foreach (var player in playerList)
        {
            Debug.Log("generateGameObj : " + player.userID);
            var playerObj = createPlayer(playerInfo: player);

            if (MultiPlayManager.Instance.isLocalPlayer(player.userID))
            {
                camera.Follow = playerObj.transform;
                //camera.LookAt = playerObj.transform;
            }
            playerData[player.userID] = playerObj;
        }
    }

    private Player createPlayer(PlayerInfo playerInfo)
    {
        var obj = Instantiate(playerObj, playerInfo.location, new Quaternion());
        var playerScript = obj.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.Init(playerInfo: playerInfo);
        }
        return playerScript;
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
        ballScript.setCurrentState(item: item);

        return obj;
    }

    public void onPlayerJoinedRoom(PlayerInfo player) {
        playerData[player.userID] = createPlayer(playerInfo: player);
    }

    public void onPlayerLeftRoom(string userID) {
        var playerObj = playerData[userID];
        Destroy(playerObj.gameObject);

        playerData.Remove(userID);
    }
}
