using LibNoise.Primitive;
using System.Collections.Concurrent;
using Voxels;

namespace Content.Universe
{
    public abstract class WorldGenerator
    {
        protected ImprovedPerlin perlin;

        protected ConcurrentQueue<Chunk> HighPriorityQueue { get; init; } = new ConcurrentQueue<Chunk>();
        protected ConcurrentQueue<Chunk> LowPriorityQueue { get; init; } = new ConcurrentQueue<Chunk>();
        public ConcurrentQueue<Chunk> OutChunks { get; init; } = new ConcurrentQueue<Chunk>();
        protected List<Thread> Threads { get; init; } = new List<Thread>();
        protected int threadCount = 0;
        //protected const int MOUNTAIN_PEAK_LEVEL = 50;
        //protected const int MOUNTAIN_LEVEL = 20;
        //protected const int GRASS_LEVEL = 0;
        //protected const int STONE_LEVEL = -3;

        protected WorldGenerator(int seed, int threadCount)
        {
            perlin = new ImprovedPerlin()
            {
                Seed = seed,
            };

            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(GenerateLoop) { Name = $"WorldGeneratorThread-{i}" };

                Threads.Add(thread);
                thread.Start();
            }
        }
        private void GenerateLoop()
        {
            while (true)
            {
                Chunk? chunk;
                if (HighPriorityQueue.TryDequeue(out chunk))
                {
                    Generate(chunk);
                }
                else if (LowPriorityQueue.TryDequeue(out chunk))
                {
                    Generate(chunk);
                }
                else
                {
                    Thread.Sleep(1);
                    continue;
                }

                chunk.UpdateIfDirty();
                OutChunks.Enqueue(chunk);

                if (threadCount != Threads.Count)
                {
                    if (Threads.Count > threadCount)
                        Threads.Remove(Thread.CurrentThread);
                    else
                    {
                        Thread thread = new Thread(GenerateLoop) { Name = $"WorldGeneratorThread-{Threads.Count}" };
                        Threads.Add(thread);
                        thread.Start();
                    }
                    return;
                }
            }
        }
        public virtual void GenerateThreaded(Chunk chunk, bool highPriority)
        {
            if(highPriority)
                HighPriorityQueue.Enqueue(chunk);
            else
                LowPriorityQueue.Enqueue(chunk);
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
