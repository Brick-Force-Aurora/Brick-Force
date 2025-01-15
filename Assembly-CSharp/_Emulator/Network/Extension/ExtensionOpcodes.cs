namespace _Emulator
{
    class ExtensionOpcodes
    {
        public const int opConnectedAck = 1000;
        public const int opSlotDataAck = 1001;
        public const int opPostLoadInitAck = 1002;
        public const int opInventoryReq = 1003;
        public const int opInventoryAck = 1004;
        public const int opCustomMessageAck = 1005;
        public const int opDisconnectReq = 1006;
        public const int opDisconnectAck = 1007;
        public const int opBeginChunkedBufferReq = 1008;
        public const int opChunkedBufferReq = 1009;
        public const int opEndChunkedBufferReq = 1010;
        public const int opBeginChunkedBufferAck= 1011;
        public const int opChunkedBufferAck = 1012;
        public const int opEndChunkedBufferAck = 1013;
        public const int opChunkedBufferThumbnailReq = 1014;
        public const int opRendezvousInfoSteamAck = 1015;
        public const int opEnterSteamAck = 1016;
        public const int opSlotDataSteamAck = 1017;
    }
}
