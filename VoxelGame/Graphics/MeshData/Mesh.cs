using Graphics.GpuMemory;

namespace Graphics
{
    public struct Mesh<T> where T : struct
    {
        public GpuArrayBuffer<T> VertexBuffer { get; init; }
        public GpuArrayBuffer<uint> IndexBuffer { get; init; }
        public GpuBufferStructure BufferStructure { get; init; }
    }
}
