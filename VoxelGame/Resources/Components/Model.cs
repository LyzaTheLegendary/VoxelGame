using Graphics.GpuMemory;
using Graphics;
using OpenTK.Mathematics;
using Utils;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using Graphics.GpuTextures;
using Resources.Creators;

namespace Resources.Components
{
    public record struct Bone()
    {
        public string Name { get; init; }
        public int ParentIndex { get; init; }
        public int Index { get; init; }
    }
    // TODO: load in their texture using bindless textures.
    public class Model : IComponent, IDisposable
    {
        public string Name { get; private set; }
        public Mesh<BonedVertex> Mesh { get; private set; }
        public Bone[] Skeleton { get; private set; }
        public Animation[] Animations { get; private set; }
        public Animation? GetAnimation(string name) => Animations.FirstOrDefault(a => a.Name == name);
        public Bone[] GetBones() => Skeleton;
        private long textureHandle = -1;
        
        public Model(Stream stream) => Read(stream);
        public void CreateResourceFromData(Stream stream) => Read(stream);
        

        private void Read(Stream stream)
        {
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
            
            Array.Sort(Skeleton, (bone1, bone2) => bone1.Index.CompareTo(bone2.Index));

            for(int i = 0; i <  vertexBufferLength; i++)
                vertexArray[i] = stream.Read<BonedVertex>();
            
            for(int i = 0; i < indexBufferLength; i++)
                indices[i] = stream.Read<uint>();

            int animationCount = stream.Read<int>();

            Animations = new Animation[animationCount];

            for (int i = 0; i < animationCount; i++)
                Animations[i] = new Animation(stream);
            
            bool hasTexture = stream.ReadByte() == 1;
            
            if (hasTexture)
                textureHandle = TextureManager.AddTexture(new Bitmap(stream));
            
            Mesh = new Mesh<BonedVertex>()
            {
                BufferStructure = GraphicsDevice.AllocateArrayStructure(),
                IndexBuffer = GraphicsDevice.AllocateArray<uint>(indices, BufferUsageHint.StaticDraw, BufferTarget.ElementArrayBuffer),
                VertexBuffer = GraphicsDevice.AllocateArray<BonedVertex>(vertexArray, BufferUsageHint.StaticDraw, BufferTarget.ArrayBuffer)
            };

            Mesh.BufferStructure.SetVertexArray(Mesh.VertexBuffer);

            Mesh.BufferStructure.AddAttribute(3, VertexAttribType.Float, false, 0);
            Mesh.BufferStructure.AddAttribute(2, VertexAttribType.Float, false, Marshal.SizeOf<Vector3>());
            Mesh.BufferStructure.AddAttribute(1, VertexAttribType.Int, false, Marshal.SizeOf<Vector3>() + Marshal.SizeOf<Vector2>());
            Mesh.BufferStructure.AddAttribute(1, VertexAttribType.Float, false, Marshal.SizeOf<Vector3>() + Marshal.SizeOf<Vector2>() + sizeof(int));
        }

        public void Dispose()
        {
            //TODO: potential idea: it wouldn't be a bad idea to make a vertex buffer pool?
            Mesh.IndexBuffer.Dispose();
            Mesh.VertexBuffer.Dispose();
            Mesh.BufferStructure.Dispose();
            TextureManager.UnloadTexture(textureHandle);
            GC.SuppressFinalize(this);
        }
    }
}
