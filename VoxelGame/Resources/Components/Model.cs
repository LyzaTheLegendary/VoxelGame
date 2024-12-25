using Graphics.GpuMemory;
using Graphics;
using OpenTK.Mathematics;
using Utils;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Resources.Components
{
    public struct Bone()
    {
        public string Name { get; init; }
        public int ParentIndex { get; init; }
        public int Index { get; init; }
    }
    public struct Mesh
    {
        public GpuArrayBuffer<uint> ElementArray { get; set; }
        public GpuArrayBuffer<BonedVertex> VertexArray { get; set; }
        public GpuBufferStructure BufferStructure { get; set; }
    }
    public class Model() : IComponent, IDisposable
    {
        public string Name { get; private set; }
        public Mesh mesh = new();
        public Bone[] Skeleton { get; private set; }
        public Animation[] Animations { get; private set; }
        public Animation? GetAnimation(string name) => Animations.FirstOrDefault(a => a.Name == name);
        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            using (MemoryStream stream = new(data as byte[] ?? data.ToArray())) {

                Name = stream.ReadString();

                int vertexBufferLength = stream.Read<int>();
                int indexBufferLength = stream.Read<int>();
                int skeletonLength = stream.Read<int>();

                if(skeletonLength == 0)
                    Skeleton = Array.Empty<Bone>();
                else
                    Skeleton = new Bone[skeletonLength];

                BonedVertex[] vertexArray = new BonedVertex[vertexBufferLength];
                uint[] indices = new uint[indexBufferLength];

                for (int i = 0; i < skeletonLength; i++)
                {
                    string name = stream.ReadString();
                    int parentIndex = stream.Read<int>();
                    int index = stream.Read<int>();
                    Skeleton[i] = new Bone() { Name = name, ParentIndex = parentIndex, Index = index };
                }

                for(int i = 0; i <  vertexBufferLength; i++)
                    vertexArray[i] = stream.Read<BonedVertex>();
                
                for(int i = 0; i < indexBufferLength; i++)
                    indices[i] = stream.Read<uint>();

                int animationCount = stream.Read<int>();

                Animation[] Animations = new Animation[animationCount];

                for (int i = 0; i < animationCount; i++)
                    Animations[i] = new Animation(stream);

                mesh.BufferStructure = GraphicsDevice.AllocateArrayStructure();
                mesh.ElementArray = GraphicsDevice.AllocateArray<uint>(indices, BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer);
                mesh.VertexArray = GraphicsDevice.AllocateArray<BonedVertex>(vertexArray, BufferUsageHint.StaticDraw, BufferTarget.ArrayBuffer);

                mesh.BufferStructure.SetVertexArray(mesh.VertexArray);

                mesh.BufferStructure.AddAttribute(3, VertexAttribType.Float, false, 0);
                mesh.BufferStructure.AddAttribute(2, VertexAttribType.Float, false, Marshal.SizeOf<Vector3>());
                mesh.BufferStructure.AddAttribute(1, VertexAttribType.Int, false, Marshal.SizeOf<Vector3>() + Marshal.SizeOf<Vector2>());
                mesh.BufferStructure.AddAttribute(1, VertexAttribType.Float, false, Marshal.SizeOf<Vector3>() + Marshal.SizeOf<Vector2>() + sizeof(int));
            }
        }

        public void Dispose()
        {
            mesh.ElementArray.Dispose();
            mesh.VertexArray.Dispose();
            mesh.BufferStructure.Dispose();
        }
    }
}
