using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class CommandContext
    {

        public string Name { get; private set; }
        public CommandReader Reader { get; private set; }
        public Room.ROOM_TYPE RoomType { get; private set; }
        public Actor Actor { get { return Actor.Instance; } }

        internal CommandContext(string name, CommandReader reader)
        {
            Name = name;
            Reader = reader;
            RoomType = RoomManager.Instance.CurrentRoomType;
        }

    }
}
