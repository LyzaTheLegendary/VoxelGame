using Graphics.GpuMemory;
using OpenTK.Mathematics;
using System.Buffers;

namespace Voxels
{
    public class VoxelBatch : IDisposable
    {
        //TODO:
        //1. introduce greedy meshing
        //2. Serializing for both saves / Networking.
        private const int SIZE = 16; // chunksize 4096 CANNOT be bigger than 255

        private static readonly ArrayPool<Voxel> arrayPool = ArrayPool<Voxel>.Shared;
        private readonly GpuShaderStorageBuffer<Voxel> shaderStorage;
        private bool shouldUpdate = false;
        private readonly object lockObj = new();
        private bool disposedValue;
        
        public Vector3i Position { get; init; }
        private Voxel[] Voxels { get; init; }
        public VoxelBatch(Vector3i position, GpuShaderStorageBuffer<Voxel> shaderStorage)
        {
            Position = position;
            Voxels = arrayPool.Rent(SIZE * SIZE * SIZE);
            this.shaderStorage = shaderStorage;
        }
        public void SetVoxel(Voxel voxel, byte x, byte y,  byte z)
        {
            lock (lockObj)
            {
                Voxels[VoxelPosToBatchPos(x, y, z)] = voxel;
                shouldUpdate = true;
            }
        }
        public void PrepareRenderIfNeeded()
        {
            lock (lockObj)
            {
                if (shouldUpdate)
                    shaderStorage.Upload(0, Voxels);
            }
        }
        public VoxelType GetVoxelType(Vector3i position)
        {
            return GetVoxel(position).Type;
        }
        public Voxel GetVoxel(Vector3i position)
        {
            lock (lockObj)
            {
                Vector3i voxelPos = Position - position;

                if (voxelPos.X < 0 || voxelPos.X >= SIZE ||
                    voxelPos.Y < 0 || voxelPos.Y >= SIZE ||
                    voxelPos.Z < 0 || voxelPos.Z >= SIZE)
                {
                    throw new ArgumentOutOfRangeException(nameof(position), "Position is outside the bounds of the voxel batch.");
                }

                (byte x, byte y, byte z) = ((byte)voxelPos.X, (byte)voxelPos.Y, (byte)voxelPos.Z);

                int batchPos = VoxelPosToBatchPos(x, y, z);

                Voxel? voxel = Voxels[batchPos];

                if (voxel is not null)
                    return voxel.Value;

                voxel = new Voxel()
                {
                    x = x,
                    y = y,
                    z = z,
                    Type = VoxelType.AIR
                };

                Voxels[batchPos] = voxel.Value;

                return voxel.Value;
            }
        }
        public GpuShaderStorageBuffer<Voxel> GetGpuBuffer() => shaderStorage;
        public void SetVoxel(Voxel voxel) 
            => Voxels[VoxelPosToBatchPos(voxel.x, voxel.y, voxel.z)] = voxel;
        
        public int VoxelPosToBatchPos(byte x, byte y, byte z)
            => x + SIZE * (y + SIZE * z);
        
        ~VoxelBatch()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        private protected virtual void Dispose(bool disposing)
        {
            lock (lockObj)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        arrayPool.Return(Voxels, true);
                        shaderStorage.Dispose();
                    }
                    disposedValue = true;
                }
            }
        }
    }
}
