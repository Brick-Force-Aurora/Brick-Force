using System;
using System.Collections.Generic;
using System.Text;

namespace _Emulator
{
    class ChannelManager
    {
        //public static ChannelManager instance = new ChannelManager();
        public List<ChannelReference> channels;

        public ChannelManager()
        {
            channels = new List<ChannelReference>();
            SetupDefaultChannels();
        }

        public ChannelReference GetChannelByID(int id)
        {
            return channels.Find(x => x.channel.Id == id);
        }

        public ChannelReference GetDefaultChannel()
        {
            return channels[0];
        }

        public void AddChannel(Channel channel)
        {
            channels.Add(new ChannelReference(channel));
        }

        public void AddChannel(int id, Channel.MODE mode, string name)
        {
            channels.Add((new ChannelReference(new Channel(_id: id, _mode: (int)mode, _name: name, _ip: "", _port: 5000, _userCount: 1, _maxUserCount: 16, _country: 1, _minLvRank: 0, _maxLvRank: 66, _xpBonus: 0, _fpBonus: 0, _limitStarRate: 0))));
        }

        public void SetupDefaultChannels()
        {
            AddChannel(1, Channel.MODE.BATTLE, "Play");
            AddChannel(2, Channel.MODE.MAPEDIT, "Build");
        }

        public void Shutdown()
        {
            foreach (ChannelReference channel in channels)
                channel.Shutdown();
            channels.Clear();
        }
    }
}
