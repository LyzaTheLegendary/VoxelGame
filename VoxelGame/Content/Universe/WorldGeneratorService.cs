using LibNoise.Primitive;
using OpenTK.Mathematics;
using Voxels;

namespace Content.Universe
{
    public abstract class WorldGeneratorService
    {
        protected ImprovedPerlin perlin;

        protected const int MOUNTAIN_PEAK_LEVEL = 50;
        protected const int MOUNTAIN_LEVEL = 20;
        protected const int GRASS_LEVEL = 0;
        protected const int STONE_LEVEL = -3;

        protected WorldGeneratorService(int seed)
        {
            perlin = new ImprovedPerlin()
            {
                Seed = seed,
            };
            
        }
        public virtual void Generate(Chunk chunk) => throw new NotImplementedException("Generate method not implemented");

        protected VoxelType GenerateVoxel(int x, int y, int z, Chunk chunk)
        {
            double noiseValue = perlin.GetValue(x, 0, z);

            (int worldX, int worldY, int worldZ) = chunk.GetWorldPosition(x, y, z);

            int height = (int)(((noiseValue + 1) / 2) * 20)- 10 + worldY;

            //Console.WriteLine($"Height: {height}");
            return VoxelType.DIRT;
            if (height == GRASS_LEVEL)
                return VoxelType.GRASS;
            else if (height > GRASS_LEVEL && height < STONE_LEVEL)
                return VoxelType.DIRT;
            else if (height <= STONE_LEVEL)
                return VoxelType.STONE;
            else
                return VoxelType.AIR;
        }
    }
}
