using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace _Emulator
{
    public class WhisperReplyCommand : ICommand
    {
        public string Description()
        {
            return "Reply whisper to another player";
        }

        public void Execute(CommandContext context)
        {
            WhisperCommand.IsWhisper = true;
            WhisperCommand.ExecuteWhisper(GlobalVars.Instance.whisperNickFrom, context.Reader.SkipWhitespace().GetUnread());
        }
    }
}
