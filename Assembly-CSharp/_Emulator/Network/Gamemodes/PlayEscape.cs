using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class PlayEscape : IGameMode
    {
        private readonly ServerEmulator emulator;

        public PlayEscape(ServerEmulator emulator)
        {
            this.emulator = emulator;
        }

        public void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom)
        {

        }

        public void HandleRoomCreation(ClientReference clientRef, MatchData match, Room room, int[] parameters)
        {
            room.goal = parameters[0];
            room.timelimit = parameters[1];
            room.weaponOption = parameters[2];
            room.map = parameters[3];
            room.isBreakInto = Convert.ToBoolean(parameters[4]);
            match.isBalance = Convert.ToBoolean(parameters[5]);
            room.isWanted = Convert.ToBoolean(parameters[6]);
            room.isDropItem = /*Convert.ToBoolean(parameters[7]);*/ false;
            match.useBuildGun = false;
        }

        public void HandleMatchEnd(MatchData match)
        {
            emulator.playDefense.HandleMatchEnd(match);
        }
    }
}
