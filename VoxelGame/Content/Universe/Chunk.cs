using Graphics.GpuMemory;
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
    public class Chunk : VoxelBatch, IDisposable // TODO: implement greedy meshing
    {
        
        public ChunkMesh Mesh { get; init; }
        private bool isDirty;
        public Chunk(int x, int y, int z) : base(x, y, z)
        {
            Mesh = new ChunkMesh()
            {
                //Buffer = GraphicsDevice.AllocateShaderBuffer<ushort>(Voxels.Length, BufferUsageHint.StaticDraw, 0),
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
                BuildGreedyMesh();
                //Mesh.Buffer.Upload(0, Voxels, Voxels.Length);
                isDirty = false;
            }
        }
        public void BuildGreedyMesh()
        {
            Vector3i dimensions = new(BATCH_SIZE, BATCH_SIZE, BATCH_SIZE);

            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();

            int[] mask = new int[dimensions.X * dimensions.Y];

            for (int d = 0; d < 3; d++) // dimensions x y z
            {
                int u = (d + 1) % 3; 
                int v = (d + 2) % 3;

                Vector3i pos = new Vector3i();
                Vector3i normal = new Vector3i();
                normal[d] = 1;

                for (pos[d] = -1; pos[d] < dimensions[d];)
                {
                    int n = 0;
                    for (pos[v] = 0; pos[v] < dimensions[v]; pos[v]++)
                    {
                        for (pos[u] = 0; pos[u] < dimensions[u]; pos[u]++)
                        {
                            VoxelType voxelCurrent = (pos[d] >= 0) ? GetVoxel(pos) : VoxelType.AIR;
                            Vector3i nextPos = pos + normal;
                            VoxelType voxelNext = (nextPos[d] < dimensions[d]) ? GetVoxel(nextPos) : VoxelType.AIR;

                            if (voxelCurrent != VoxelType.AIR && voxelNext == VoxelType.AIR)
                            {
                                mask[n++] = (int)voxelCurrent; // front face
                            }
                            else if (voxelCurrent == VoxelType.AIR && voxelNext != VoxelType.AIR)
                            {
                                mask[n++] = -(int)voxelNext; // back face
                            }
                            else
                            {
                                mask[n++] = 0; // no face needed
                            }
                        }
                    }

                    pos[d]++;


                    n = 0;
                    for (int j = 0; j < dimensions[v]; j++)
                    {
                        for (int i = 0; i < dimensions[u];)
                        {
                            if (mask[n] != 0)
                            {
                                int voxel = mask[n];
                                int width, height;
                                for (width = 1; i + width < dimensions[u] && mask[n + width] == voxel; width++) ;

                                bool done = false;
                                for (height = 1; j + height < dimensions[v]; height++)
                                {
                                    for (int k = 0; k < width; k++)
                                    {
                                        if (mask[n + k + height * dimensions[u]] != voxel)
                                        {
                                            done = true;
                                            break;
                                        }
                                    }
                                    if (done) break;
                                }

                                pos[u] = i;
                                pos[v] = j;

                                Vector3i du = new Vector3i();
                                Vector3i dv = new Vector3i();
                                du[u] = width;
                                dv[v] = height;

                                Vector3 basePos = new Vector3(pos.X, pos.Y, pos.Z);
                                Vector3 normalDir = voxel > 0 ? normal : -normal;

                                vertices.Add(new Vertex(basePos, new Vector2(0, 0)));
                                vertices.Add(new Vertex(basePos + du, new Vector2(1, 0)));
                                vertices.Add(new Vertex(basePos + dv, new Vector2(0, 1)));
                                vertices.Add(new Vertex(basePos + du + dv, new Vector2(1, 1)));

                                int idx = vertices.Count;
                                if (voxel > 0)
                                {
                                    indices.Add((uint)(idx - 4));
                                    indices.Add((uint)(idx - 3));
                                    indices.Add((uint)(idx - 2));
                                    indices.Add((uint)(idx - 2));
                                    indices.Add((uint)(idx - 3));
                                    indices.Add((uint)(idx - 1));
                                }
                                else
                                {
                                    indices.Add((uint)(idx - 4));
                                    indices.Add((uint)(idx - 2));
                                    indices.Add((uint)(idx - 3));
                                    indices.Add((uint)(idx - 3));
                                    indices.Add((uint)(idx - 2));
                                    indices.Add((uint)(idx - 1));
                                }

                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        mask[n + x + y * dimensions[u]] = 0;
                                    }
                                }

                                i += width;
                            }
                            else
                            {
                                i++;
                            }

                            n++;
                        }
                    }
                }
            }

            // Upload generated mesh data
            Mesh.VertexBuffer.Upload(vertices);
            Mesh.IndexBuffer.Upload(indices);
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
