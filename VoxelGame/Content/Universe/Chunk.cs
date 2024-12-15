﻿using Graphics.GpuMemory;
using Voxels;
using OpenTK.Graphics.OpenGL4;
using Graphics;
using OpenTK.Mathematics;
using Utils;
using System.Runtime.InteropServices;

namespace Content.Universe
{
    public struct ChunkMesh
    {
        //public GpuShaderStorageBuffer<ushort> Buffer { get; init; }
        public GpuArrayBuffer<Vertex> VertexBuffer { get; init; }
        public GpuArrayBuffer<uint> IndexBuffer { get; init; }
        public GpuBufferStructure BufferStructure { get; init; }
    }
    public class Chunk : VoxelBatch, IDisposable
    {
        
        public ChunkMesh Mesh { get; init; }
        private bool isDirty;
        public Chunk(int x, int y, int z) : base(x, y, z)
        {
            Mesh = new ChunkMesh()
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
        public void Dispose()
        {
            //Mesh.Buffer.Dispose();
            Mesh.VertexBuffer.Dispose();
            Mesh.IndexBuffer.Dispose();
            Mesh.BufferStructure.Dispose();
        }
    }
}
