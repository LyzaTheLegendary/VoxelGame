using OpenTK.Mathematics;
using Voxels;

namespace Content.Universe
{
    public class FlatWorldGenerator : WorldGeneratorService
    {
        public FlatWorldGenerator(int seed) : base(seed)
        {

        }
        public override void Generate(Chunk chunk)
        {
            Vector3 position = chunk.Position;

            for (int x = 0; x < Chunk.BATCH_SIZE; x++)
                for (int z = 0; z < Chunk.BATCH_SIZE; z++)
                    for(int y = 0; y < Chunk.BATCH_SIZE; y++)
                    {
                        VoxelType voxel = GenerateVoxel(x, y, z, chunk);
                        
                        chunk.SetVoxel(x, y, z, voxel);
                    }
            
        }
    }
}
