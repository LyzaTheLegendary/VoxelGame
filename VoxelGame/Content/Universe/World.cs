using System.Collections.Concurrent;
using Voxels;

namespace Content.Universe
{
    public enum WorldType: byte
    {
        EARTH,
    }
    public class World
    {
        ConcurrentDictionary<(int, int, int), Chunk> LoadedChunks { get; init; } = new ConcurrentDictionary<(int, int, int), Chunk>();
        WorldGenerator worldGenerator;
        public WorldType Type { get; init; }
        public World(WorldType type = WorldType.EARTH)
        {
            Type = type;
            worldGenerator = new FlatWorldGenerator(123);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            (int chunkX, int chunkY, int chunkZ) = NormalizeCoordinates(x, y, z);

            if (!LoadedChunks.TryGetValue((chunkX, chunkY, chunkZ), out Chunk? chunk))
            {
                chunk = new Chunk(chunkX, chunkY, chunkZ);
                worldGenerator.Generate(chunk);

                if (LoadedChunks.TryAdd((chunkX, chunkY, chunkZ), chunk))
                    return chunk;
                else
                    throw new Exception("Failed to add chunk to world");
            }

            return chunk;
        }
        public VoxelType GetBlockAt(int x, int y, int z)
        {
            (int chunkX, int chunkY, int chunkZ) = NormalizeCoordinates(x, y, z);

            return GetChunk(chunkX, chunkY, chunkZ).GetVoxel(x - chunkX, y - chunkY, z - chunkZ);
        }
        public void SetBlockAt(int x, int y, int z, VoxelType type)
        {
            (int chunkX, int chunkY, int chunkZ) = NormalizeCoordinates(x, y, z);

            GetChunk(chunkX, chunkY, chunkZ).SetVoxel(x - chunkX, y - chunkY, z - chunkZ, type);
        }
        private (int, int, int) NormalizeCoordinates(int x, int y, int z)
        {
            x = x * VoxelBatch.BATCH_SIZE - x % VoxelBatch.BATCH_SIZE;
            y = y * VoxelBatch.BATCH_SIZE - y % VoxelBatch.BATCH_SIZE;
            z = z * VoxelBatch.BATCH_SIZE - z % VoxelBatch.BATCH_SIZE;
            return (x, y, z);
        }
    }
}
