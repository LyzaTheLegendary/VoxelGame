using Graphics;
using Graphics.GpuMemory;
using OpenTK.Mathematics;
using Resources;
using Utils;
using VoxelGame;
using OpenTK.Graphics.OpenGL4;
namespace Voxels
{
    public class Shape : IResourceFactory, IDisposable
    {
        private bool disposedValue;

        public Shape() { }
        public string Name { get; private set; }
        public GpuArrayBuffer<uint> ElementArray { get; private set; }
        public GpuArrayBuffer<Vector3> VertexArray { get; private set; }
        public GpuBufferStructure BufferStructure { get; private set; }

        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            GraphicsDevice device = Application.Instance.GraphicsDevice;
            using (MemoryStream stream = new MemoryStream(data as byte[] ?? data.ToArray()))
            {
                Name = stream.ReadString();
                int elemLen = stream.Read<int>(); 

                uint[] elementArray = new uint[elemLen];

                for (int i = 0; i < elemLen; i++)
                    elementArray[i] = stream.Read<uint>();


                int vertexCount = stream.Read<int>();
                Vector3[] vertices = new Vector3[vertexCount];

                for(int i = 0; i < vertexCount; i++)
                    vertices[i] = stream.Read<Vector3>();

                ElementArray = device.AllocateArray<uint>(elementArray, BufferUsageHint.StaticRead, BufferTarget.ElementArrayBuffer);
                VertexArray = device.AllocateArray<Vector3>(vertices, BufferUsageHint.StaticRead, BufferTarget.ArrayBuffer);
                VertexArray.Bind();
                BufferStructure = device.AllocateArrayStructure();
                BufferStructure.Bind();
                BufferStructure.AddAttribute<Vector3>(1, VertexAttribType.Float, false, 0, 0);


                
                



                // TODO: think about VAO structure as now we just assume oh hey it's a vec3 enjoy
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
