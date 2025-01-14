using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImGuiNET;

namespace _Emulator
{
    public class ImGuiUtils
    {
        public static float SplitRemainingWidth(float splitBy)
        {
            return ImGui.GetContentRegionAvail().X / splitBy;
        }
    }
}
