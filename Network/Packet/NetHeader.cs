using System.Runtime.InteropServices;

namespace Network.Packet
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NetHeader
    {
        public ushort PacketId { get; init; }
        public ushort Length { get; init; }
        public byte Checksum { get; private set; }

        public NetHeader(ClientBound id, ushort length)
        {
            PacketId = (ushort)id;
            Length = length;
            CreateChecksum();
        }
        public NetHeader(ServerBound id, ushort length)
        {
            PacketId = (ushort)id;
            Length = length;
            CreateChecksum();
        }

        public void CreateChecksum() => Checksum = (byte)(Length << 8);
        
        public bool ValidateChecksum() => Checksum == (byte)(Length << 8); 
    }
}
