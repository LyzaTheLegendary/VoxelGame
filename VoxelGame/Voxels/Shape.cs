using Graphics;
using Graphics.GpuMemory;
using OpenTK.Mathematics;
using Resources;
using Utils;
using VoxelGame;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
namespace Voxels
{
    public class Shape() : IComponent, IDisposable
    {
        private bool disposedValue;

        public string Name { get; private set; }
        public GpuArrayBuffer<uint> ElementArray { get; private set; }
        public GpuArrayBuffer<Vertex> VertexArray { get; private set; }
        public GpuBufferStructure BufferStructure { get; private set; }

        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            using (MemoryStream stream = new MemoryStream(data as byte[] ?? data.ToArray()))
            {
                Name = stream.ReadString();
                int vertices = stream.Read<int>();
                int indices = stream.Read<int>();

                Vertex[] vertexArray = new Vertex[vertices];
                uint[] indiceArray = new uint[indices];

                for (int i = 0; i < vertices; i++)
                    vertexArray[i] = stream.Read<Vertex>();

                for (int i = 0; i < indices; i++)
                    indiceArray[i] = stream.Read<uint>();

                VertexArray = GraphicsDevice.AllocateArray<Vertex>(vertexArray, BufferUsageHint.StaticRead, BufferTarget.ArrayBuffer);
                ElementArray = GraphicsDevice.AllocateArray<uint>(indiceArray, BufferUsageHint.StaticRead, BufferTarget.ElementArrayBuffer);

                BufferStructure = GraphicsDevice.AllocateArrayStructure();
                BufferStructure.SetVertexArray(VertexArray);

                BufferStructure.AddAttribute(3, VertexAttribType.Float, false, 0);
                BufferStructure.AddAttribute(2, VertexAttribType.Float, false, Marshal.SizeOf<Vector3>());
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ElementArray.Dispose();
                    VertexArray.Dispose();
                    BufferStructure.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
