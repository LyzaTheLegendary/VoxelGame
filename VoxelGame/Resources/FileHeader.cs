using System.Runtime.InteropServices;

namespace Resources
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FileHeader
    {
        public FileType Type;
        public ushort Version;
        public int Flags;
    }
}
