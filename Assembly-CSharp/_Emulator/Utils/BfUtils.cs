using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    class BfUtils
    {
        public static string RoomTypeToString(Room.ROOM_TYPE roomType)
        {
            switch (roomType)
            {
                case Room.ROOM_TYPE.MAP_EDITOR:
                    return "Build";
                case Room.ROOM_TYPE.TEAM_MATCH:
                    return "TDM";
                case Room.ROOM_TYPE.INDIVIDUAL:
                    return "DM";
                case Room.ROOM_TYPE.CAPTURE_THE_FLAG:
                    return "CTF";
                case Room.ROOM_TYPE.EXPLOSION:
                    return "Defusion";
                case Room.ROOM_TYPE.MISSION:
                    return "Defense";
                case Room.ROOM_TYPE.BND:
                    return "BND";
                case Room.ROOM_TYPE.BUNGEE:
                    return "Freefall";
                case Room.ROOM_TYPE.ESCAPE:
                    return "Escape";
                case Room.ROOM_TYPE.ZOMBIE:
                    return "Zombie";
                default:
                    return "None";
            }
        }

        public static string RoomStatusToString(Room.ROOM_STATUS roomStatus)
        {
            switch (roomStatus)
            {
                case Room.ROOM_STATUS.NONE:
                default:
                    return "None";
                case Room.ROOM_STATUS.WAITING:
                    return "Waiting";
                case Room.ROOM_STATUS.PENDING:
                    return "Pending";
                case Room.ROOM_STATUS.PLAYING:
                    return "Playing";
                case Room.ROOM_STATUS.MATCHING:
                    return "Matching";
                case Room.ROOM_STATUS.MATCH_END:
                    return "Match End";
            }
        }

        public static string BrickManStatusToString(BrickManDesc.STATUS status)
        {
            switch (status)
            {
                default:
                    return "None";
                case BrickManDesc.STATUS.PLAYER_WAITING:
                    return "Waiting";
                case BrickManDesc.STATUS.PLAYER_READY:
                    return "Ready";
                case BrickManDesc.STATUS.PLAYER_LOADING:
                    return "Loading";
                case BrickManDesc.STATUS.PLAYER_P2PING:
                    return "P2Ping";
                case BrickManDesc.STATUS.PLAYER_PLAYING:
                    return "Playing";
                case BrickManDesc.STATUS.PLAYER_SHOP:
                    return "Shop";
                case BrickManDesc.STATUS.PLAYER_INVEN:
                    return "Inventory";
            }
        }
    }
}
