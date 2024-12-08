using System.Collections.Concurrent;

namespace Content.Universe
{
    public enum WorldType: byte
    {
        EARTH,
    }
    public class World
    {
        ConcurrentDictionary<(int, int, int), Chunk> LoadedChunks { get; init; } = new ConcurrentDictionary<(int, int, int), Chunk>();
        IWorldGeneratorService worldGenerator;
        public string WorldName { get; init; }
        public WorldType Type { get; init; }
        public World(string name, WorldType type = WorldType.EARTH)
        {
            WorldName = name;
            Type = type;
            worldGenerator = new FlatWorldGenerator();
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            if (!LoadedChunks.TryGetValue((x, y, z), out Chunk? chunk))
            {
                chunk = new Chunk(x, y, z);
                worldGenerator.Generate(chunk);

                if(LoadedChunks.TryAdd((x, y, z), chunk))
                    return chunk;
                else
                    throw new Exception("Failed to add chunk to world");
            }

            return chunk;
        }
    }
}
