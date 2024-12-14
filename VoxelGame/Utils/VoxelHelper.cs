using Voxels;

namespace Utils
{
    public static class VoxelHelper
    {
        public static bool IsSolid(this VoxelType type)
        {
            return type != VoxelType.AIR;
        }
    }
}
