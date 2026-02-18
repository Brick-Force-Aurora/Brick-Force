using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class WhisperCommand : ICommand
    {

        public static bool IsWhisper { get; set; }

        public string Description()
        {
            return "Whisper to another player";
        }

        public void Execute(string name, CommandReader reader)
        {
            IsWhisper = true;
            string nickName = reader.ReadToken();
            GlobalVars.Instance.whisperNickTo = nickName;
            ExecuteWhisper(nickName, reader.SkipWhitespace().GetUnread());
        }

        internal static void ExecuteWhisper(string nickName, string message)
        {
            if (message.Length == 0)
            {
                return;
            }
            // TODO: IMPLEMENT WHISPER
        }
    }
}
