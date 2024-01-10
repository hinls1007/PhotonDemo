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

        public void playerMove(string userID, Vector3 location, Vector3 velocity = new Vector3(), Vector3 angularVelocity = new Vector3())
        {
            client.playerMove(userID: userID, location: location, velocity: velocity, angularVelocity: angularVelocity);
        }

        public void objectMove(string triggerByID, string targetObjectID, Vector3 velocity = new Vector3(), Vector3 angularVelocity = new Vector3())
        {
            client.objectMove(
                triggerByID: triggerByID,
                targetObjectID: targetObjectID,
                velocity: velocity,
                angularVelocity: angularVelocity);
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

        public void onOtherObjectMove(string triggerByID, string targetObjectID, Vector3 velocity = default, Vector3 angularVelocity = default)
        {
            foreach (var callback in callbackSet)
            {
                callback.onOtherObjectMove(triggerByID: triggerByID, targetObjectID: targetObjectID, velocity: velocity, angularVelocity: angularVelocity);
            }
        }

        public void onOtherPlayerMove(string userID, Vector3 velocity = default, Vector3 angularVelocity = default)
        {
            foreach (var callback in callbackSet)
            {
                callback.onOtherPlayerMove(userID: userID, velocity: velocity, angularVelocity: angularVelocity);
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

        public void onRoomJoined()
        {
            foreach (var callback in callbackSet)
            {
                callback.onRoomJoined();
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
        public virtual void onRoomJoined() { }
        public virtual void onRoomJoinFail() { }

        public virtual void onOtherPlayerMove(string userID, Vector3 velocity = new Vector3(), Vector3 angularVelocity = new Vector3()) { }
        public virtual void onOtherObjectMove(string triggerByID, string targetObjectID, Vector3 velocity = new Vector3(), Vector3 angularVelocity = new Vector3()) { }

        public virtual void onPlayerJoinedRoom(PlayerInfo player) { }
        public virtual void onPlayerLeftRoom(string userID) { }
    }

    public class PlayerInfo
    {
        public string userID;
        public Vector3 location;

        public PlayerInfo(string userID, Vector3 location)
        {
            this.userID = userID;
            this.location = location;
        }
    }
}

