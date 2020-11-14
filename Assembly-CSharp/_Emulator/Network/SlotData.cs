namespace _Emulator
{
    class SlotData
    {
        public ClientReference client;
        public bool isUsed = false;
        public bool isLocked = false;
        public int slotIndex;
        public bool isRed = false;

        public SlotData(int _slotIndex)
        {
            slotIndex = _slotIndex;
        }

        public void ToggleLock(bool value)
        {
            if (!isUsed)
                isLocked = value;
        }
    }
}
