using Graphics.GpuMemory;
using Voxels;
using OpenTK.Graphics.OpenGL4;

namespace Content.Universe
{
    public class Chunk : VoxelBatch
    {
        public GpuShaderStorageBuffer<VoxelType> Buffer { get; init; }
        private bool isDirty;
        public Chunk(int x, int y, int z) : base(x, y, z)
        {
            Buffer = new GpuShaderStorageBuffer<VoxelType>(GL.GenBuffer(), Voxels, BufferUsageHint.DynamicDraw, 0);
            isDirty = false;
        }

        public void UpdateIfDirty()
        {
            if (isDirty)
            {
                Buffer.Upload(0, Voxels);
                isDirty = false;
            }
        }
    }
}
