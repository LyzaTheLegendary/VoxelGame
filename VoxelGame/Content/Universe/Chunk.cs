using Graphics.GpuMemory;
using Voxels;
using OpenTK.Graphics.OpenGL4;
using Graphics;
using OpenTK.Mathematics;
using Utils;
using System.Runtime.InteropServices;

namespace Content.Universe
{
    public class Chunk : VoxelBatch
    {
        public Mesh<Vertex> Mesh { get; private set; }
        private bool isDirty;
        public int Players { get; set; } = 0;
        public Chunk(int x, int y, int z) : base(x, y, z)
        {
            Mesh = new Mesh<Vertex>()
            {
                VertexBuffer = GraphicsDevice.AllocateArray<Vertex>(null, BufferUsageHint.StaticDraw, BufferTarget.ArrayBuffer),
                IndexBuffer = GraphicsDevice.AllocateArray<uint>(null, BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer),
                BufferStructure = GraphicsDevice.AllocateArrayStructure()
            };

            Mesh.BufferStructure.SetVertexArray(Mesh.VertexBuffer);

            Mesh.BufferStructure.AddAttribute(3, VertexAttribType.Float, false, 0);
            Mesh.BufferStructure.AddAttribute(2, VertexAttribType.Float, false, Marshal.SizeOf<Vector3>());

            isDirty = false;
        }

        public void UpdateIfDirty()
        {
            if (isDirty)
            {
                (List<Vertex> vertices, List<uint> indices) = VoxelMeshHelper.CreateMesh(this);

                Mesh.VertexBuffer.Upload(vertices);
                Mesh.IndexBuffer.Upload(indices);
                isDirty = false;
            }
        }

        public override void SetVoxel(int x, int y, int z, VoxelType type)
        {
            isDirty = true;
            base.SetVoxel(x, y, z, type);
        }
        public override void Dispose()
        {
            Mesh.VertexBuffer.Dispose();
            Mesh.IndexBuffer.Dispose();
            Mesh.BufferStructure.Dispose();
            base.Dispose();
        }
    }
}
