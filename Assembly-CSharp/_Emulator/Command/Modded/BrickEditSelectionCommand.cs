using System;
using UnityEngine;

namespace _Emulator
{
    public class BrickEditSelectionCommand : ICommand
    {
        public string Description()
        {
            return "Clears the selection of the selection tool";
        }

        public void Execute(CommandContext context)
        {
            if (!context.IsInAuthorizedBuildRoom())
            {
                return;
            }
            EditHelper.BrickEditTool.ClearSelection();
        }
    }
}
