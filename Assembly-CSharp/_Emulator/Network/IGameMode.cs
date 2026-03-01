using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public interface IGameMode
    {
        void RegisterNetworkHandlers(Action<MessageId, Action<MsgReference>> register, Action<ExtensionOpcodes, Action<MsgReference>> registerCustom);
        void HandleRoomCreation(ClientReference clientRef, MatchData match, Room room, int[] parameters);
        void HandleMatchEnd(MatchData match);

    }
}
