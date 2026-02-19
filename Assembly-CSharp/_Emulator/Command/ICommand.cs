using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public interface ICommand
    {
        string Description();

        void Execute(CommandContext context);

    }
}
