using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static TItem;
using System.Text.RegularExpressions;

namespace _Emulator
{
    public class BuildMapEdit : IGameMode
    {
        private readonly ServerEmulator emulator;

        public BuildMapEdit(ServerEmulator emulator)
        {
            this.emulator = emulator;
        }

        public void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom)
        {
            registerCustom(ExtensionOpcodes.opRequestUserSlotMapFailedReq, HandleUserSlotMapFailed);
            registerCustom(ExtensionOpcodes.opRequestUserSlotMapSuccessReq, HandleUserSlotMapSuccess);
        }

        public void HandleRoomCreation(ClientReference clientRef, MatchData match, Room room, int[] parameters)
        {
            bool loadMap = Convert.ToBoolean(parameters[0]);
            int slot = parameters[1];
            int landscapeIndex = parameters[2];
            int skyboxIndex = parameters[3];
            sbyte premium = (sbyte)parameters[4];
            if (loadMap)
            {
                if (clientRef.isHost)
                {
                    match.CacheMapFromSlot(UserMapInfoManager.Instance.Get(slot));
                    if (clientRef.buildModeRequestedMap)
                    {
                        clientRef.buildModeRequestedMap = false;
                        emulator.SendCacheBrick(clientRef, match);
                        emulator.SendCacheBrickDone(clientRef, match);
                    }
                } else
                {
                    UserMapInfo userMapInfo = new UserMapInfo(slot, premium);
                    match.cachedUMI = userMapInfo;
                    SendRequestUserMap(clientRef, match, slot);
                }
            } else
            {
                match.CacheMapGenerate(slot, landscapeIndex, skyboxIndex, room.CurMapAlias);
                if (clientRef.buildModeRequestedMap)
                {
                    clientRef.buildModeRequestedMap = false;
                    emulator.SendCacheBrick(clientRef, match);
                    emulator.SendCacheBrickDone(clientRef, match);
                }
            }
            match.room.map = slot;
            match.room.isBreakInto = true;
            match.useBuildGun = true;
        }
        public void SendRequestUserMap(ClientReference clientRef, MatchData match, int slot)
        {
            if (match == null)
            {
                match = clientRef.matchData;
            }
            match.cachedMap = new UserMap();
            match.cachedMap.isLoaded = false;
            MsgBody body = new MsgBody();
            body.Write(slot);
            emulator.Say(new MsgReference(ExtensionOpcodes.opRequestUserSlotMapAck, body, clientRef));
        }

        public void HandleMatchEnd(MatchData match)
        {
            emulator.playDeathMatch.HandleMatchEnd(match);
        }

        public void HandleUserSlotMapFailed(MsgReference msgRef)
        {
            if (msgRef.client.seq != msgRef.matchData.masterSeq)
            {
                return;
            }
            msgRef.msg._msg.Read(out int slot);
            msgRef.matchData.CacheMapGenerate(slot, 0, 0, msgRef.matchData.room.CurMapAlias);
            if (msgRef.client.buildModeRequestedMap)
            {
                msgRef.client.buildModeRequestedMap = false;
                emulator.SendCacheBrick(msgRef.client);
                emulator.SendCacheBrickDone(msgRef.client);
            }
        }

        public void HandleUserSlotMapSuccess(MsgReference msgRef)
        {
            if (msgRef.client.seq != msgRef.matchData.masterSeq)
            {
                return;
            }
            MatchData match = msgRef.matchData;
            MsgBody body = msgRef.msg._msg;
            body.Read(out int brickCount);

            match.cachedMap.map = match.room.map;
            match.cachedUMI.BrickCount = brickCount;

            if (msgRef.client.buildModeRequestedMap)
            {
                msgRef.client.buildModeRequestedMap = false;
                emulator.SendCacheBrick(msgRef.client);
                emulator.SendCacheBrickDone(msgRef.client);
            }
        }
    }
}
