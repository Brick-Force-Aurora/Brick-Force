namespace _Emulator
{
    public interface IOperation
    {

        bool HasNextStep();

        // Returns the amount of changes made
        OperationData NextStep();

        void Completed(ulong successCount);

    }
    public enum OperationFlag : ushort
    {
        None = 0x0,
        Delete = 0x1,
        OnlySource = 0x2,
        SourceWithRotation = 0x4,
        IncludeEmpty = 0x8,
        ExcludeSourceType = 0x10,
    }

    public struct OperationData
    {
        public ushort flag;
        public byte sourceTemplate;
        public byte sourceRotation;
        public byte targetTemplate;
        public byte targetRotation;
        public byte[] coordinates;
    }

    public static class OperationExtensions
    {

        private static readonly byte[] EmptyByteArray = new byte[0];

        public static bool IsSet(this ushort value, OperationFlag flag)
        {
            return (value & ((ushort)flag)) == ((ushort)flag);
        }

        public static ushort Set(this ushort value, OperationFlag flag, bool state = true)
        {
            if (value.IsSet(flag) == state)
            {
                return (ushort)(value & ~(ushort)flag);
            }
            return (ushort)(value | (ushort)flag);
        }

        public static OperationData NewData(this IOperation _)
        {
            OperationData data = new OperationData();
            data.flag = 0;
            data.sourceTemplate = 0;
            data.sourceRotation = 0;
            data.targetTemplate = 0;
            data.targetRotation = 0;
            data.coordinates = EmptyByteArray;
            return data;
        }
    }
}
