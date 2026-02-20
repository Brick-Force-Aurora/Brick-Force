using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _Emulator
{
    public class SetHollowOperation : IOperation
    {

        public readonly byte x1, y1, z1, x2, y2, z2;
        public readonly byte template, rotation;

        private bool executed = false;
        public SetHollowOperation(BrickEditTool tool, byte template)
        {
            tool.GetPos1(out x1, out y1, out z1);
            tool.GetPos2(out x2, out y2, out z2);
            this.template = template;
            tool.GetRotation(out rotation, template);
        }
        public SetHollowOperation(BrickEditTool tool, byte x1, byte y1, byte z1, byte x2, byte y2, byte z2, byte template)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.z1 = z1;
            this.x2 = x2;
            this.y2 = y2;
            this.z2 = z2;
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

            byte minXInner = (byte)(minX == 255 ? 255 : minX + 1);
            byte maxXInner = Math.Max((byte)(maxX == 0 ? 0 : maxX - 1), minXInner);

            byte minYInner = (byte)(minY == 255 ? 255 : minY + 1);
            byte maxYInner = Math.Max((byte)(maxY == 0 ? 0 : maxY - 1), minYInner);

            byte minZInner = (byte)(minZ == 255 ? 255 : minZ + 1);
            byte maxZInner = Math.Max((byte)(maxZ == 0 ? 0 : maxZ - 1), minZInner);

            byte[] coordinates = new byte[((maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1) * 3) - ((maxXInner - minXInner + 1) * (maxYInner - minYInner + 1) * (maxZInner - minZInner + 1) * 3)];
            int index = 0;
            for (byte x = minX; x <= maxX; x++)
            {
                for (byte y = minY; y <= maxY; y++)
                {
                    coordinates[index++] = x;
                    coordinates[index++] = y;
                    coordinates[index++] = minZ;
                    if (minZ != maxZ)
                    {
                        coordinates[index++] = x;
                        coordinates[index++] = y;
                        coordinates[index++] = maxZ;
                    }
                }
                if (minZInner != maxZInner)
                {
                    for (byte z = minZInner; z <= maxZInner; z++)
                    {
                        coordinates[index++] = x;
                        coordinates[index++] = minY;
                        coordinates[index++] = z;
                        if (minY != maxY)
                        {
                            coordinates[index++] = x;
                            coordinates[index++] = maxY;
                            coordinates[index++] = z;
                        }
                    }
                }
            }
            if (minZInner != maxZInner)
            {
                for (byte z = minZInner; z <= maxZInner; z++)
                {
                    for (byte y = minY; y <= maxY; y++)
                    {
                        coordinates[index++] = minXInner;
                        coordinates[index++] = y;
                        coordinates[index++] = z;
                        if (minXInner != maxXInner)
                        {
                            coordinates[index++] = maxXInner;
                            coordinates[index++] = y;
                            coordinates[index++] = z;
                        }
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
