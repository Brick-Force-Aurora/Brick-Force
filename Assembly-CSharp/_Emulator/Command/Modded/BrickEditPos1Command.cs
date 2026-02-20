using System;
using UnityEngine;

namespace _Emulator
{
    public class BrickEditPos1Command : ICommand
    {
        public string Description()
        {
            return "Sets the first position of the selection tool";
        }

        public void Execute(CommandContext context)
        {
            if (!context.IsInAuthorizedBuildRoom())
            {
                return;
            }
            context.Reader.ReadTokens(0, 3, out string[] tokens);
            byte[] coordinates = new byte[3];
            EditHelper.PlayerPosition.ToCoords(ref coordinates);
            tokens.AsCoords(0, ref coordinates);
            EditHelper.BrickEditTool.SetPos1(coordinates[0], coordinates[1], coordinates[2]);
        }
    }
}
