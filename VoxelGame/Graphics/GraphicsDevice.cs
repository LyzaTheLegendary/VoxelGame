using Graphics.GpuComputing;
using Graphics.GpuMemory;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Graphics
{
    public enum GpuType
    {
        NVIDIA,
        AMD,
        INTEL,
        NOT_SUPPORTED,
    }
    public struct GpuInfo
    {
        public int MaxBindingPoints { get; init; }
        public int MaxSSBOSize { get; init; }
    }
    public struct GpuState()
    {
        public int Program { get; set; }
        public int VAO { get; set; }
        public int VBO { get; set; }
        public int IBO { get; set; }
    }

    public class GraphicsDevice
    {
        private const int GL_GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX = 0x9049;
        private const int GL_TEXTURE_FREE_MEMORY_ATI = 0x87FC;

        private GpuType type;
        private GpuState currentState = new();
        private GpuInfo gpuInfo;
        public GraphicsDevice() {
            string renderer = GL.GetString(StringName.Renderer) ?? "Unknown Renderer";

            if (renderer.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
                type = GpuType.NVIDIA;
            else if (renderer.Contains("AMD", StringComparison.OrdinalIgnoreCase))
                type = GpuType.AMD;
            else if (renderer.Contains("Intel", StringComparison.OrdinalIgnoreCase))
                type = GpuType.INTEL;
            else
            {
                type = GpuType.NOT_SUPPORTED;
                throw new NotSupportedException("Gpu is not supported");
            }

            int majorVersion = QueryValue(GetPName.MajorVersion);
            int minorVersion = QueryValue(GetPName.MinorVersion);

            if (majorVersion < 4 || (majorVersion == 4 && minorVersion < 5))
                throw new NotSupportedException("OpenGL versions below 4.5 are not supported");

            int maxBindingPoints = QueryValue((GetPName)0x90DF);
            int maxSizeInBytes = QueryValue((GetPName)0x90DE);

            gpuInfo = new GpuInfo() { 
                MaxBindingPoints = maxBindingPoints,
                MaxSSBOSize = maxSizeInBytes,
            };

        }
        public long AvailableMemory() // butt fuck broken, have to fix.
        {
            return type switch
            {
                GpuType.NVIDIA => QueryAvailableMemory(GL_GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX),
                GpuType.AMD => QueryAvailableMemory(GL_TEXTURE_FREE_MEMORY_ATI),
                _ => throw new NotSupportedException("Memory querying is not supported on this GPU.")
            };
        }
        public GpuArrayBuffer<T> AllocateArray<T>(IEnumerable<T>? data, BufferUsageHint hint, BufferTarget bufferTarget) where T : struct
        {
            var buffer = new GpuArrayBuffer<T>(hint, GL.GenBuffer(), bufferTarget);

            if (data is not null)
                buffer.Upload(data);

            return buffer;
        }
        public GpuShaderStorageBuffer<T> AllocateShaderBuffer<T>(int count, BufferUsageHint hint, int bindingPoint) where T : struct
        {
            if (count * Marshal.SizeOf<T>() > gpuInfo.MaxSSBOSize || bindingPoint > gpuInfo.MaxBindingPoints)
                throw new NotSupportedException("Unsupported SSBO interaction");

            return new GpuShaderStorageBuffer<T>(GL.GenBuffer(), count, hint, bindingPoint);
        }
        public GpuBufferStructure AllocateArrayStructure()
        {
            return new GpuBufferStructure(GL.GenVertexArray());
        }
        public void Bind<T>(GpuArrayBuffer<T> buffer) where T : struct
        {
            int pointer = buffer.GetPointer();
            BufferTarget target = buffer.GetTarget();

            //if (target == BufferTarget.ArrayBuffer)
            //    if (currentState.VBO == pointer)
            //        return;
            //    else
            //    {
            //        currentState.VBO = pointer;
            //        buffer.Bind();
            //    }
            //else if (target == BufferTarget.ElementArrayBuffer)
            //    if (currentState.IBO == pointer)
            //        return;
            //    else
            //    {
            //        currentState.IBO = pointer;
            //        buffer.Bind();
            //    }
            buffer.Bind();
        }
        public void Bind(GpuBufferStructure buffer)
        {
            int pointer = buffer.GetPointer();

            //if (currentState.VAO == pointer)
            //    return;

            currentState.VAO = pointer;
            buffer.Bind();
        }
        public void Bind(Shader shader)
        {
            int pointer = shader.GetPointer();

            if (currentState.Program == pointer)
                return;

            currentState.Program = pointer;
            shader.Bind();
        }
        public void Bind<T>(GpuShaderStorageBuffer<T> buffer) where T : struct // we're not caching which buffers we have now.
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, buffer.GetBindingPoint(), buffer.GetPointer());
        }
        private long QueryAvailableMemory(int queryCode)
        {
            GL.GetInteger((GetPName)queryCode, out int availableMemoryKb);
            return (long)availableMemoryKb * 1024;
        }
        private int QueryValue(GetPName code)
        {
            GL.GetInteger(code, out var value);

            return value;
        }

    }
}
