using OpenTK.Mathematics;

namespace Voxels
{
    public struct Voxel
    {
        // Do I need this on the gpu? I don't think I do?
        public byte x;
        public byte y;
        public byte z;

        public VoxelType Type { get; set; }
    }
}
