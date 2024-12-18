using LibNoise.Primitive;
using System.Collections.Concurrent;
using Voxels;

namespace Content.Universe
{
    public abstract class WorldGenerator
    {
        protected ImprovedPerlin perlin;

        //protected const int MOUNTAIN_PEAK_LEVEL = 50;
        //protected const int MOUNTAIN_LEVEL = 20;
        //protected const int GRASS_LEVEL = 0;
        //protected const int STONE_LEVEL = -3;

        protected WorldGenerator(int seed)
        {
            perlin = new ImprovedPerlin()
            {
                Seed = seed,
            };
        }

        public virtual void Generate(Chunk chunk) => throw new NotImplementedException("Generate method not implemented");
        protected virtual int GetHeight(int x, int y, int z, Chunk chunk)
        {
            double noiseValue = perlin.GetValue(x,z);

            int height = (int)(((noiseValue + 1) / 2) * 20) - 10 + chunk.GetWorldPositionY(y);

            return y;
        }
        protected virtual VoxelType GenerateVoxel(int x, int y, int z, Chunk chunk)
        {
            return VoxelType.AIR;
        }
    }
}
