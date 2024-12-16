using Content.Universe;
using Graphics;
using OpenTK.Mathematics;
using Voxels;

namespace Utils
{
    public static class VoxelMeshHelper
    {
        const int CELL_SIZE = 16;
        const int ATLAS_SIZE = 2048;
        const int ATLAS_ROWS = ATLAS_SIZE / CELL_SIZE;
        static public (List<Vertex>, List<uint>) CreateMesh(Chunk chunk)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();

            for (int x = 0; x < Chunk.BATCH_SIZE; x++)
            {
                for (int z = 0; z < Chunk.BATCH_SIZE; z++)
                {
                    for (int y = 0; y < Chunk.BATCH_SIZE; y++)
                    {
                        VoxelType currentVoxel = chunk.GetVoxel(x, y, z);

                        if (currentVoxel == VoxelType.AIR) continue;

                        if (y == 0 || chunk.GetVoxel(x, y - 1, z) != currentVoxel) // Bottom face (y-1)
                            VoxelMeshHelper.ProcessFace(vertices, indices, x, y, z, currentVoxel, -Vector3i.UnitY);

                        if (y == Chunk.BATCH_SIZE - 1 || chunk.GetVoxel(x, y + 1, z) != currentVoxel) // Top face (y+1)
                            VoxelMeshHelper.ProcessFace(vertices, indices, x, y, z, currentVoxel, Vector3i.UnitY);

                        if (x == 0 || chunk.GetVoxel(x - 1, y, z) != currentVoxel) // Left face (x-1)
                            VoxelMeshHelper.ProcessFace(vertices, indices, x, y, z, currentVoxel, -Vector3i.UnitX);

                        if (x == Chunk.BATCH_SIZE - 1 || chunk.GetVoxel(x + 1, y, z) != currentVoxel) // Right face (x+1)
                            VoxelMeshHelper.ProcessFace(vertices, indices, x, y, z, currentVoxel, Vector3i.UnitX);

                        if (z == 0 || chunk.GetVoxel(x, y, z - 1) != currentVoxel) // Back face (z-1)
                            VoxelMeshHelper.ProcessFace(vertices, indices, x, y, z, currentVoxel, -Vector3i.UnitZ);

                        if (z == Chunk.BATCH_SIZE - 1 || chunk.GetVoxel(x, y, z + 1) != currentVoxel) // Front face (z+1)
                            VoxelMeshHelper.ProcessFace(vertices, indices, x, y, z, currentVoxel, Vector3i.UnitZ);
                    }
                }
            }

            return (vertices, indices);
        }
        static public void ProcessFace(List<Vertex> vertices, List<uint> indices, int x, int y, int z, VoxelType voxelType, Vector3i direction)
        {
            int faceIndex = GetFaceIndex(direction);
            Vector2 texCoord = GetTextureFromAtlas(voxelType, faceIndex);
            
            // Generate a face and add it to the mesh
            int startIndex = vertices.Count;
            Vector3i position = new(x, y, z);

            // Add the face based on the direction (top, bottom, left, right, front, back)
            AddFace(vertices, indices, position + new Vector3i(0, 1, 0), texCoord, startIndex, direction);
        }

        static public void AddFace(List<Vertex> vertices, List<uint> indices, Vector3i position, Vector2 texCoord, int startIndex, Vector3i direction)
        {
            int offset = vertices.Count;

            if (direction == Vector3i.UnitY) // Top face
            {
                vertices.Add(new Vertex { Position = position + new Vector3i(0, 0, 0), TexCoord = texCoord });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, 0, 0), TexCoord = texCoord + new Vector2i(1, 0) });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, 0, 1), TexCoord = texCoord + new Vector2i(1, 1) });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, 0, 1), TexCoord = texCoord + new Vector2i(0, 1) });
            }
            else if (direction == -Vector3i.UnitY) // Bottom face
            {
                vertices.Add(new Vertex { Position = position + new Vector3i(0, -1, 0), TexCoord = texCoord });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, -1, 0), TexCoord = texCoord + new Vector2i(1, 0) });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, -1, 1), TexCoord = texCoord + new Vector2i(1, 1) });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, -1, 1), TexCoord = texCoord + new Vector2i(0, 1) });
            }
            else if (direction == Vector3i.UnitX) // Right face
            {
                vertices.Add(new Vertex { Position = position + new Vector3i(1, 0, 0), TexCoord = texCoord });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, -1, 0), TexCoord = texCoord + new Vector2i(1, 0) });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, -1, 1), TexCoord = texCoord + new Vector2i(1, 1) });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, 0, 1), TexCoord = texCoord + new Vector2i(0, 1) });
            }
            else if (direction == -Vector3i.UnitX) // Left face
            {
                vertices.Add(new Vertex { Position = position + new Vector3i(0, 0, 0), TexCoord = texCoord });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, -1, 0), TexCoord = texCoord + new Vector2i(1, 0) });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, -1, 1), TexCoord = texCoord + new Vector2i(1, 1) });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, 0, 1), TexCoord = texCoord + new Vector2i(0, 1) });
            }
            else if (direction == Vector3i.UnitZ) // Front face
            {
                vertices.Add(new Vertex { Position = position + new Vector3i(0, 0, 1), TexCoord = texCoord });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, 0, 1), TexCoord = texCoord + new Vector2i(1, 0) });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, -1, 1), TexCoord = texCoord + new Vector2i(1, 1) });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, -1, 1), TexCoord = texCoord + new Vector2i(0, 1) });
            }
            else if (direction == -Vector3i.UnitZ) // Back face
            {
                vertices.Add(new Vertex { Position = position + new Vector3i(0, 0, 0), TexCoord = texCoord });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, 0, 0), TexCoord = texCoord + new Vector2i(1, 0) });
                vertices.Add(new Vertex { Position = position + new Vector3i(1, -1, 0), TexCoord = texCoord + new Vector2i(1, 1) });
                vertices.Add(new Vertex { Position = position + new Vector3i(0, -1, 0), TexCoord = texCoord + new Vector2i(0, 1) });
            }

            // Define the two triangles (6 indices) forming the face
            indices.Add((uint)(startIndex + 0));
            indices.Add((uint)(startIndex + 1));
            indices.Add((uint)(startIndex + 2));

            indices.Add((uint)(startIndex + 2));
            indices.Add((uint)(startIndex + 3));
            indices.Add((uint)(startIndex + 0));
        }
        static public Vector2 GetTextureFromAtlas(VoxelType type, int face)
        {
            int index = (ushort)type * 6 + face;

            int xCell = index % ATLAS_ROWS;
            int yCell = (int)Math.Floor((float)(index / ATLAS_ROWS));

            return new Vector2(xCell, yCell);
        }

        static public int GetFaceIndex(Vector3i direction)
        {
            if (direction == Vector3i.UnitY) return 0; // Top face
            if (direction == -Vector3i.UnitY) return 1; // Bottom face
            if (direction == Vector3i.UnitX) return 2; // Right face
            if (direction == -Vector3i.UnitX) return 3; // Left face
            if (direction == Vector3i.UnitZ) return 4; // Front face
            if (direction == -Vector3i.UnitZ) return 5; // Back face
            return -1; // Invalid direction
        }
    }
}
