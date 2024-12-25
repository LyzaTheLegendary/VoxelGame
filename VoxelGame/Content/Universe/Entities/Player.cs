using OpenTK.Mathematics;
namespace Content.Universe.Entities
{
    public class Player : Entity
    {
        private List<(int, int, int)> LoadedChunks { get; init; } = new();
        public string Name { get; init; }
        public int Radius { get; set; } = 5 * Chunk.BATCH_SIZE;
        public Player(World world, Vector3 position, string username) : base(HandleAllocator.AllocateHandle(), world, position, EntityType.Player)
        {
            Name = username;
        }

        public void OnMove()
        {
            //calculate which ones should be loaded in a radius and check IF they are loaded otherwise load them in the queue

            for (int x = -Radius; x < Radius; x++)
            {
                for(int  y = -Radius; y < Radius; y++)
                {
                    for (int z = -Radius; z < Radius; z++)
                    {
                        if (!LoadedChunks.Contains((x, y, z)))
                        {
                            LoadedChunks.Add((x, y, z));
                            Chunk chunk = World.GetChunk((int)position.X + x, (int)position.Y + y, (int)position.Z + z);

                            chunk.Players++;
                        }
                    }
                }
            }

            foreach((int x, int y, int z) in LoadedChunks)
            {
                if (Math.Sqrt(x * y * z) > Radius)
                {
                    Chunk chunk = World.GetChunk(x, y, z);

                    chunk.Players--;

                    LoadedChunks.Remove((x,y,z));
                }
            }

            // figure out the unloading of the old chunks
        }
    }
}
