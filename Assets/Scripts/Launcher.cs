using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MultiPlayer;

public class Launcher : MonoBehaviour, MultiPlayCallback
{
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

        Debug.Log("Test Json");
        var json = "{\"userId\":\"0baf6c6a-8dfe-49af-9701-80d174d8f729\",\"velocity\":{\"x\":-0.2058689147233963,\"y\":0.9021569490432739,\"z\":0.1355285346508026},\"angularVelocity\":{\"x\":-0.18757937848567964,\"y\":-0.07084726542234421,\"z\":0.47557929158210757}}";
        PhotonClientImpl.PlayerMove newObj = JsonUtility.FromJson<PhotonClientImpl.PlayerMove>(json); 
        //var test = new Test(new Vector3(10, 0, 10));
        //Debug.Log("Test Json obj Bef " + test.location);
        //var json = JsonUtility.ToJson(test);
        //Debug.Log("Test Json obj json" + json);
        //var newObj = JsonUtility.FromJson<Test>(json);
        Debug.Log("Test Json new obj json " + newObj.userId);
        Debug.Log("Test Json new obj json " + newObj.velocity);

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
        var userID = MultiPlayManager.Instance.getUserID();
        if (userID != null
            && playerData.ContainsKey(userID))
        {
            float x = Input.GetAxis("Horizontal");// * 10f * Time.deltaTime;
            float z = Input.GetAxis("Vertical"); // * 10f * Time.deltaTime;
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

    private Player createPlayer(string playerID, Vector3 location)
    {
        var obj = Instantiate(playerObj, location, new Quaternion());
        var playerScript = obj.GetComponent<Player>();

        if (playerScript != null)
        {
            playerScript.playerID = playerID;
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
        playerData[player.userID] = createPlayer(playerID: player.userID, location: player.location);
    }

    public void onPlayerLeftRoom(string userID) {
        var playerObj = playerData[userID];
        Destroy(playerObj.gameObject);

        playerData.Remove(userID);
    }
}
