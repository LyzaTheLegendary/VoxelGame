using Graphics;
using Graphics.GpuMemory;
using OpenTK.Mathematics;
using Resources;
using Utils;
using VoxelGame;
using OpenTK.Graphics.OpenGL4;
namespace Voxels
{
    public class Shape() : IComponent, IDisposable
    {
        private bool disposedValue;

        public string Name { get; private set; }
        public GpuArrayBuffer<uint> ElementArray { get; private set; }
        public GpuArrayBuffer<Vector3> VertexArray { get; private set; }
        public GpuArrayBuffer<Vector2> UniformArray { get; private set; }
        public GpuBufferStructure BufferStructure { get; private set; }

        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            GraphicsDevice device = Application.Instance.GraphicsDevice;
            using (MemoryStream stream = new MemoryStream(data as byte[] ?? data.ToArray()))
            {
                Name = stream.ReadString();
                int elementCount = stream.Read<int>();

                uint[] elementArray = new uint[elementCount];

                for (int i = 0; i < elementCount; i++)
                    elementArray[i] = stream.Read<uint>();

                int vertexCount = stream.Read<int>();
                Vector3[] vertices = new Vector3[vertexCount];

                for (int i = 0; i < vertexCount; i++)
                    vertices[i] = stream.Read<Vector3>();

                int uniformCount = stream.Read<int>();

                Vector2[] uniforms = new Vector2[uniformCount];

                for(int i = 0; i < uniformCount; i++)
                    uniforms[i] = stream.Read<Vector2>();

                BufferStructure = device.AllocateArrayStructure();
                
                VertexArray = device.AllocateArray<Vector3>(vertices, BufferUsageHint.StaticRead, BufferTarget.ArrayBuffer);
                ElementArray = device.AllocateArray<uint>(elementArray, BufferUsageHint.StaticRead, BufferTarget.ElementArrayBuffer);
                UniformArray = device.AllocateArray<Vector2>(uniforms, BufferUsageHint.StaticRead, BufferTarget.ArrayBuffer);

                BufferStructure.SetVertexArray(VertexArray);
                BufferStructure.AddAttribute(3, VertexAttribType.Float, false, 0, 0);
                BufferStructure.SetVertexArray(UniformArray);
                BufferStructure.AddAttribute(2, VertexAttribType.Float, false, 0, 0);

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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Shape()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
