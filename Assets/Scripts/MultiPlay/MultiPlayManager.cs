using System;
using System.Collections.Generic;

using UnityEngine;

namespace MultiPlayer
{
    public class MultiPlayManager: MultiPlayClientCallback
    {
        public static MultiPlayManager Instance;

        private HashSet<MultiPlayCallback> callbackSet = new HashSet<MultiPlayCallback>();
        private MultiPlayClient client;

        private MultiPlayManager(MultiPlayClient client)
        {
            this.client = client;
        }

        public static void init(MultiPlayClient client)
        {
            Instance = new MultiPlayManager(client: client);
            client.init(Instance);
        }

        public void registerCallback(MultiPlayCallback callback)
        {
            callbackSet.Add(callback);
        }

        public void unregisterCallback(MultiPlayCallback callback)
        {
            callbackSet.Remove(callback);
        }

        public bool isLocalPlayer(string userID) {
            return getUserID() == userID;
        }

        public bool isHostPlayer()
        {
            return client.isHostPlayer();
        }

        public string getUserID()
        {
            return client.getUserID();
        }

        public void connectServer()
        {
            client.connectServer();
        }

        public void createRoom(string roomName, string password = "")
        {
            client.createRoom(roomName: roomName, password: password);
        }

        public void joinRoom(string roomName, string password = "")
        {
            client.joinRoom(roomName: roomName, password: password);
        }

        public void createOrJoinRoom(string roomName, string password = "")
        {
            client.createOrJoinRoom(roomName: roomName, password: password);
        }

        public List<PlayerInfo> getPlayerList()
        {
            return client.getPlayerList();
        }

        public void initRoomInfo(RoomInfo roomInfo)
        {
            client.initRoomInfo(roomInfo: roomInfo);
        }

        public void playerMove(PlayerInfo playerInfo)
        {
            client.playerMove(playerInfo: playerInfo);
        }

        public void objectMove(string triggerByID, RoomItem item)
        {
            client.objectMove(
                triggerByID: triggerByID,
                item: item);
        }

        public void onConnectedServer()
        {
            foreach(var callback in callbackSet)
            {
                callback.onConnectedServer();
            }
        }

        public void onConnectServerError()
        {
            foreach (var callback in callbackSet)
            {
                callback.onConnectServerError();
            }
        }

        public void onOtherObjectMove(string triggerByID, RoomItem item)
        {
            foreach (var callback in callbackSet)
            {
                callback.onOtherObjectMove(triggerByID: triggerByID, item: item);
            }
        }

        public void onOtherPlayerMove(PlayerInfo playerInfo)
        {
            foreach (var callback in callbackSet)
            {
                callback.onOtherPlayerMove(playerInfo: playerInfo);
            }
        }

        public void onRoomCreated()
        {
            foreach (var callback in callbackSet)
            {
                callback.onRoomCreated();
            }
        }

        public void onRoomCreatError()
        {
            foreach (var callback in callbackSet)
            {
                callback.onRoomCreatError();
            }
        }

        public void onRoomJoined(RoomInfo roomInfo)
        {
            Debug.Log("Manager OnJoinedRoom");
            foreach (var callback in callbackSet)
            {
                callback.onRoomJoined(roomInfo: roomInfo);
            }
        }

        public void onRoomJoinFail()
        {
            foreach (var callback in callbackSet)
            {
                callback.onRoomJoinFail();
            }
        }


        public void onPlayerJoinedRoom(PlayerInfo player)
        {
            foreach (var callback in callbackSet)
            {
                callback.onPlayerJoinedRoom(player: player);
            }
        }

        public void onPlayerLeftRoom(string userID)
        {
            foreach (var callback in callbackSet)
            {
                callback.onPlayerLeftRoom(userID: userID);
            }
        }
    }

    public interface MultiPlayCallback
    {
        public virtual void onConnectedServer() { }
        public virtual void onConnectServerError() { }
        public virtual void onRoomCreated() { }
        public virtual void onRoomCreatError() { }
        public virtual void onRoomJoined(RoomInfo roomInfo) { }
        public virtual void onRoomJoinFail() { }

        public virtual void onOtherPlayerMove(PlayerInfo playerInfo) { }
        public virtual void onOtherObjectMove(string triggerByID, RoomItem item) { }

        public virtual void onPlayerJoinedRoom(PlayerInfo player) { }
        public virtual void onPlayerLeftRoom(string userID) { }
    }

    public class PlayerInfo
    {
        public string userID;
        public Vector3 location;
        public Quaternion rotation;
        public Vector3 currentVelocity;
        public Vector3 currentAngularVelocity;

        public PlayerInfo(string userID, Vector3 location, Quaternion rotation, Vector3 currentVelocity, Vector3 currentAngularVelocity)
        {
            this.userID = userID;
            this.location = location;
            this.rotation = rotation;
            this.currentVelocity = currentVelocity;
            this.currentAngularVelocity = currentAngularVelocity;
        }
    }

    public class RoomInfo
    {
        public List<RoomItem> roomItemList;

        public RoomInfo()
        {
            this.roomItemList = new List<RoomItem>();
        }

        public RoomInfo(List<RoomItem> roomItemList)
        {
            this.roomItemList = roomItemList;
        }
    }

    public class RoomItem
    {
        public string itemID;
        public string itemTypeID;
        public Vector3 location;
        public Quaternion rotation;
        public Vector3 currentVelocity;
        public Vector3 currentAngularVelocity;

        public RoomItem(string itemID, string itemTypeID, Vector3 location = new Vector3(), Quaternion rotation = new Quaternion(), Vector3 currentVelocity = new Vector3(), Vector3 currentAngularVelocity = new Vector3())
        {
            this.itemID = itemID;
            this.itemTypeID = itemTypeID;
            this.location = location;
            this.rotation = rotation;
            this.currentVelocity = currentVelocity;
            this.currentAngularVelocity = currentAngularVelocity;
        }
    }
}

