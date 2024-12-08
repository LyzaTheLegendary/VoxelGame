using Graphics.GpuMemory;
using OpenTK.Mathematics;
using System.Buffers;

namespace Voxels
{
    public class VoxelBatch
    {
        private static ArrayPool<VoxelType> VoxelPool { get; } = ArrayPool<VoxelType>.Shared;
        public const int BATCH_SIZE = 16;
        public Vector3i Position { get; init; }
        public VoxelType[] Voxels { get; private set; }

        public VoxelBatch(int x, int y, int z)
        {
            Position = new Vector3i(x, y, z);
            Voxels = VoxelPool.Rent(BATCH_SIZE * BATCH_SIZE * BATCH_SIZE);
        }

        public VoxelType GetVoxel(int x, int y, int z)
        {
            if (x >= BATCH_SIZE || y >= BATCH_SIZE || z >= BATCH_SIZE)
                throw new ArgumentOutOfRangeException("Voxel coordinates are out of bounds.");

            int index = GetIndex(x, y, z);
            return Voxels[index];
        }

        public void SetVoxel(int x, int y, int z, VoxelType type)
        {
            if (x >= BATCH_SIZE || y >= BATCH_SIZE || z >= BATCH_SIZE)
                throw new ArgumentOutOfRangeException("Voxel coordinates are out of bounds.");

            int index = GetIndex(x, y, z);
            Voxels[index] = type;
        }

        private int GetIndex(int x, int y, int z) => x + (y * BATCH_SIZE) + (z * BATCH_SIZE * BATCH_SIZE);
        
        ~VoxelBatch()
        {
            VoxelPool.Return(Voxels, true);
        }
    }
}
