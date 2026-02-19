using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
