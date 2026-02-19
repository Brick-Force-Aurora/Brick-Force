namespace _Emulator
{
    public interface IOperation
    {

        bool HasNextStep();

        // Returns the amount of changes made
        OperationData NextStep();

        void Completed(ulong successCount);

    }
}
