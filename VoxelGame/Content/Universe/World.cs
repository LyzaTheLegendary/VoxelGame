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
        private WorldGeneratorService worldGenerator;
        public string WorldName { get; init; }
        public WorldType Type { get; init; }
        public World(string name, WorldType type = WorldType.EARTH)
        {
            WorldName = name;
            Type = type;
            worldGenerator = new FlatWorldGenerator(123);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            var chunkCoords = NormalizeCoordinates(x, y, z);

            if (!LoadedChunks.TryGetValue(chunkCoords, out Chunk? chunk))
            {
                chunk = new Chunk(chunkCoords.Item1, chunkCoords.Item2, chunkCoords.Item3);
                worldGenerator.Generate(chunk);

                if (LoadedChunks.TryAdd(chunkCoords, chunk))
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
        private (int, int, int) NormalizeCoordinates(int x, int y, int z)
        {
            x = x * VoxelBatch.BATCH_SIZE - x % VoxelBatch.BATCH_SIZE;
            y = y * VoxelBatch.BATCH_SIZE - y % VoxelBatch.BATCH_SIZE;
            z = z * VoxelBatch.BATCH_SIZE - z % VoxelBatch.BATCH_SIZE;
            return (x, y, z);
        }
    }
}
