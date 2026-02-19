using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class SetOperation : IOperation
    {

        public readonly byte x1, y1, z1, x2, y2, z2;
        public readonly byte template, rotation;

        private bool executed = false;
        public SetOperation(BrickEditTool tool, byte template)
        {
            tool.GetPos1(out x1, out y1, out z1);
            tool.GetPos2(out x2, out y2, out z2);
            this.template = template;
            tool.GetRotation(out rotation, template);
        }

        public bool HasNextStep()
        {
            return !executed;
        }

        public OperationData NextStep()
        {
            executed = true;
            OperationData data = this.NewData();
            data.flag = data.flag.Set(OperationFlag.IncludeEmpty);
            data.targetTemplate = template;
            data.targetRotation = rotation;
            byte minX = Math.Min(x1, x2);
            byte minY = Math.Min(y1, y2);
            byte minZ = Math.Min(z1, z2);
            byte maxX = Math.Max(x1, x2);
            byte maxY = Math.Max(y1, y2);
            byte maxZ = Math.Max(z1, z2);
            byte[] coordinates = new byte[(maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1) * 3];
            int index = 0;
            for (byte x = minX; x <= maxX; x++)
            {
                for (byte y = minY; y <= maxY; y++)
                {
                    for (byte z = minZ; z <= maxZ; z++)
                    {
                        coordinates[index++] = x;
                        coordinates[index++] = y;
                        coordinates[index++] = z;
                    }
                }
            }
            data.coordinates = coordinates;
            return data;
        }

        public void Completed(ulong successCount)
        {
            Actor.Instance.SendChat($"Successfully set {successCount} brick(s)");
        }
    }
}
