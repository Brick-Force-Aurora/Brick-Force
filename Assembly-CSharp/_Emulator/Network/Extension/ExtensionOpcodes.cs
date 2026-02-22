namespace _Emulator
{
    public enum ExtensionOpcodes : ushort
    {
        opConnectedAck = 1000,
        opSlotDataAck = 1001,
        opPostLoadInitAck = 1002,
        opInventoryReq = 1003,
        opInventoryAck = 1004,
        opCustomMessageAck = 1005,
        opDisconnectReq = 1006,
        opDisconnectAck = 1007,

        opBeginChunkedBufferReq = 1008,
        opChunkedBufferReq = 1009,
        opEndChunkedBufferReq = 1010,

        opRendezvousInfoSteamAck = 1015,
        opEnterSteamAck = 1016,
        opSlotDataSteamAck = 1017,
        opVersionCheckReq = 1018,
        opVersionCheckAck = 1019,
        opBulkBrickReq = 1020,
        opBulkBrickAck = 1021,
        opBulkBrickFailAck = 1022,

        opAmIConnectedReq = 1023,
        opAmIConnectedAck = 1024,

        opRequestUserSlotMapAck = 1025,
        opRequestUserSlotMapSuccessReq = 1026,
        opRequestUserSlotMapFailedReq = 1027,
    }
}
