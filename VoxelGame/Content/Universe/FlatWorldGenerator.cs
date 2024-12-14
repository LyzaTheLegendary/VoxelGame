using OpenTK.Mathematics;
using Voxels;

namespace Content.Universe
{
    public class FlatWorldGenerator : IWorldGeneratorService
    {
        public void Generate(Chunk chunk)
        {
            Vector3 position = chunk.Position;

            if (position.Y < 0 || position.Y > 0)
                return;


            for(int x = 0; x < Chunk.BATCH_SIZE; x++)
                for(int z = 0; z < Chunk.BATCH_SIZE; z++)
                    chunk.SetVoxel(x, 0, z, VoxelType.DIRT);
        }
    }
}
