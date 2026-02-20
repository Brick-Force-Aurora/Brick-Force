using System;

namespace _Emulator
{
    public class DeleteOperation : IOperation
    {

        public readonly byte x1, y1, z1, x2, y2, z2;
        public readonly byte sourceTemplate;
        public readonly bool withSource;

        private bool executed = false;
        public DeleteOperation(BrickEditTool tool, byte sourceTemplate = 0, bool withSource = false)
        {
            tool.GetPos1(out x1, out y1, out z1);
            tool.GetPos2(out x2, out y2, out z2);
            this.sourceTemplate = sourceTemplate;
            this.withSource = withSource;
        }
        public DeleteOperation(byte x1, byte y1, byte z1, byte x2, byte y2, byte z2, byte sourceTemplate = 0, bool withSource = false)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.z1 = z1;
            this.x2 = x2;
            this.y2 = y2;
            this.z2 = z2;
            this.sourceTemplate = sourceTemplate;
            this.withSource = withSource;
        }

        public bool HasNextStep()
        {
            return !executed;
        }

        public OperationData NextStep()
        {
            executed = true;
            OperationData data = this.NewData();
            data.flag = data.flag.Set(OperationFlag.Delete);
            if (withSource)
            {
                data.sourceTemplate = sourceTemplate;
                data.flag = data.flag.Set(OperationFlag.OnlySource);
            }
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
            Actor.Instance.SendChat($"Successfully deleted {successCount} brick(s)");
        }
    }
}
