using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class WhisperReplyCommand : ICommand
    {
        public string Description()
        {
            return "Reply whisper to another player";
        }

        public void Execute(string name, CommandReader reader)
        {
            WhisperCommand.IsWhisper = true;
            WhisperCommand.ExecuteWhisper(GlobalVars.Instance.whisperNickFrom, reader.SkipWhitespace().GetUnread());
        }
    }
}
