﻿using Graphics.GpuComputing;
using Graphics.GpuMemory;
using OpenTK.Graphics.OpenGL4;
using Resources.Components;
using System.Runtime.InteropServices;
using Graphics.GpuTextures;

namespace Graphics
{
    public enum BindingPoints : int
    {
        TEXTURE_HANDLES
    }
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
        public int TEX2D { get; set; }
    }

    public static class GraphicsDevice // turn into static class and make sure that it knows when openGL context is intialized.
    {
        private const int GL_GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX = 0x9049;
        private const int GL_TEXTURE_FREE_MEMORY_ATI = 0x87FC;

        private static GpuType type;
        private static GpuState currentState = new();
        private static GpuInfo gpuInfo;
        public static void Init() {
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
        public static long AvailableMemory() // butt fuck broken, have to fix.
        {
            return type switch
            {
                GpuType.NVIDIA => QueryAvailableMemory(GL_GPU_MEMORY_INFO_CURRENT_AVAILABLE_VIDMEM_NVX),
                GpuType.AMD => QueryAvailableMemory(GL_TEXTURE_FREE_MEMORY_ATI),
                _ => throw new NotSupportedException("Memory querying is not supported on this GPU.")
            };
        }
        public static GpuArrayBuffer<T> AllocateArray<T>(IEnumerable<T>? data, BufferUsageHint hint, BufferTarget bufferTarget) where T : struct
        {
            GL.CreateBuffers(1, out int pointer);

            if (pointer == 0 || !GL.IsBuffer(pointer))
                throw new Exception("Failed to allocate GL buffer of type: " + bufferTarget);

            var buffer = new GpuArrayBuffer<T>(hint, pointer, bufferTarget);

            if (data is not null)
                buffer.Upload(data);

            return buffer;
        }
        public static GpuShaderStorageBuffer<T> AllocateShaderBuffer<T>(int count, BufferUsageHint hint, BindingPoints bindingPoint) where T : struct
        {
            if (count * Marshal.SizeOf<T>() > gpuInfo.MaxSSBOSize || (int)bindingPoint > gpuInfo.MaxBindingPoints)
                throw new NotSupportedException("Unsupported SSBO interaction");

            GL.CreateBuffers(1, out int pointer);

            if (pointer == 0 || !GL.IsBuffer(pointer))
                throw new Exception("Failed to allocate GL buffer of type: SSBO");

            return new GpuShaderStorageBuffer<T>(pointer, count, hint, (int)bindingPoint);
        }
        public static GpuShaderStorageBuffer<T> AllocateShaderBuffer<T>(T[] data, BufferUsageHint hint, int bindingPoint) where T : struct
        {
            if (data.Length * Marshal.SizeOf<T>() > gpuInfo.MaxSSBOSize || bindingPoint > gpuInfo.MaxBindingPoints)
                throw new NotSupportedException("Unsupported SSBO interaction");

            GL.CreateBuffers(1, out int pointer);

            if (pointer == 0 || !GL.IsBuffer(pointer))
                throw new Exception("Failed to allocate GL buffer of type: SSBO");

            return new GpuShaderStorageBuffer<T>(pointer, data, hint, bindingPoint);
        }
        public static GpuBufferStructure AllocateArrayStructure()
        {
            return new GpuBufferStructure(GL.GenVertexArray());
        }
        public static Texture2D AllocateTexture(Bitmap? bitmap)
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out int pointer);

            if(pointer is 0)
                throw new Exception("Failed to allocate GL texture");

            Texture2D texture = new Texture2D(pointer);

            if(bitmap is not null)
                texture.Upload(bitmap);

            return texture;
        }
        public static TextureAtlas2D AllocateTextureAtlas(Bitmap? bitmap, TextureUnit unit)
        {
            GL.GenTextures(1, out int pointer);

            if (pointer is 0)
                throw new Exception("Failed to allocate GL texture");

            TextureAtlas2D textureAtlas = new TextureAtlas2D(unit, pointer);

            if(bitmap is not null)
                textureAtlas.Upload(bitmap);

            return textureAtlas;
        }
        
        public static void Bind<T>(GpuArrayBuffer<T> buffer) where T : struct
        {
            int pointer = buffer.GetPointer();
            BufferTarget target = buffer.GetTarget();

            if (target == BufferTarget.ArrayBuffer)
                if (currentState.VBO == pointer)
                    return;
                else
                {
                    currentState.VBO = pointer;
                    buffer.Bind();
                }
            else if (target == BufferTarget.ElementArrayBuffer)
                if (currentState.IBO == pointer)
                    return;
                else
                {
                    currentState.IBO = pointer;
                    buffer.Bind();
                }

            buffer.Bind();
        }
        public static void Bind(GpuBufferStructure buffer)
        {
            int pointer = buffer.GetPointer();

            if (currentState.VAO == pointer)
                return;

            currentState.VAO = pointer;
            buffer.Bind();
        }
        public static void Bind(Shader shader)
        {
            int pointer = shader.GetPointer();

            if (currentState.Program == pointer)
                return;

            currentState.Program = pointer;
            shader.Bind();
        }
        public static void Bind<T>(GpuShaderStorageBuffer<T> buffer) where T : struct // we're not caching which buffers we have now.
        {
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, buffer.GetBindingPoint(), buffer.GetPointer());
        }
        public static void Bind(Texture2D texture)
        {
            if(currentState.TEX2D == texture.GetPointer())
                return;

            currentState.TEX2D = texture.GetPointer();
            texture.Bind();
        }
        public static void Bind(TextureAtlas2D texture)
        {
            if (currentState.TEX2D == texture.GetPointer())
                return;

            currentState.TEX2D = texture.GetPointer();
            texture.Bind();
        }

        private static long QueryAvailableMemory(int queryCode)
        {
            GL.GetInteger((GetPName)queryCode, out int availableMemoryKb);
            return (long)availableMemoryKb * 1024;
        }
        private static int QueryValue(GetPName code)
        {
            GL.GetInteger(code, out var value);

            return value;
        }

    }
}
