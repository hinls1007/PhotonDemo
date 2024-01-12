using System;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace MultiPlayer
{
    public interface MultiPlayClient
    {
        public void init(MultiPlayClientCallback clientCallback);
        public string getUserID();
        public bool isHostPlayer();
        public List<PlayerInfo> getPlayerList();
        public void playerMove(PlayerInfo playerInfo);
        public void objectMove(string triggerByID, RoomItem item);
        public void connectServer();
        public void createRoom(string roomName, string password);
        public void joinRoom(string roomName, string password);
        public void createOrJoinRoom(string roomName, string password);
        public void initRoomInfo(RoomInfo roomInfo);
    }

    public interface MultiPlayClientCallback
    {
        public void onConnectedServer();
        public void onConnectServerError();
        public void onRoomCreated();
        public void onRoomCreatError();
        public void onRoomJoined(RoomInfo roomInfo);
        public void onRoomJoinFail();

        public void onOtherPlayerMove(PlayerInfo playerInfo);
        public void onOtherObjectMove(string triggerByID, RoomItem item);
        public void onPlayerJoinedRoom(PlayerInfo player);
        public void onPlayerLeftRoom(string userID);
    }
}
