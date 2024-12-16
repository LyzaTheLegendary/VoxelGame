using Graphics.GpuMemory;
using OpenTK.Mathematics;
using System.Buffers;

namespace Voxels
{
    public class VoxelBatch : IDisposable // implement Idisposable?
    {
        private static ArrayPool<ushort> VoxelPool { get; } = ArrayPool<ushort>.Shared;
        public const int BATCH_SIZE = 32;
        public const int TOTAL_SIZE = BATCH_SIZE * BATCH_SIZE * BATCH_SIZE;
        public Vector3i Position { get; init; }
        public ushort[] Voxels { get; private set; }

        public VoxelBatch(int x, int y, int z)
        {
            Position = new Vector3i(x, y, z);
            Voxels = VoxelPool.Rent(TOTAL_SIZE + 1);

            for(int i = 0; i < TOTAL_SIZE; i++)
                Voxels[i] = (ushort)VoxelType.AIR;
        }
        public (int x, int y, int z) GetWorldPosition(int localeX, int localeY, int localeZ) => (Position.X * BATCH_SIZE + localeX, Position.Y * BATCH_SIZE + localeY, Position.Z * BATCH_SIZE + localeZ);
        
        public virtual VoxelType GetVoxel(Vector3i pos) => GetVoxel(pos.X, pos.Y, pos.Z);
        public virtual VoxelType GetVoxel(int x, int y, int z)
        {
            int index = GetIndex(x, y, z);

            if (index > TOTAL_SIZE || index < 0)
                throw new ArgumentOutOfRangeException("Voxel coordinates are out of bounds.");


            return (VoxelType)Voxels[index];
        }
        public virtual VoxelType GetVoxel(int index)
        {
            if (index >= TOTAL_SIZE)
                throw new ArgumentOutOfRangeException("Voxel index is out of bounds.");

            return (VoxelType)Voxels[index];
        }
        public virtual void SetVoxel(int x, int y, int z, VoxelType type)
        {
            if (x >= BATCH_SIZE || y >= BATCH_SIZE || z >= BATCH_SIZE)
                throw new ArgumentOutOfRangeException("Voxel coordinates are out of bounds.");

            int index = GetIndex(x, y, z);
            Voxels[index] = (ushort)type;
        }

        protected int GetIndex(int x, int y, int z) => x + (y * BATCH_SIZE) + (z * (BATCH_SIZE * BATCH_SIZE));

        public virtual void Dispose()
        {
            VoxelPool.Return(Voxels, false);
        }

    }
}
