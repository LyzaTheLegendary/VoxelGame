﻿using OpenTK.Graphics.OpenGL4;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Graphics.GpuMemory
{
    public class GpuShaderStorageBuffer<T> : IDisposable where T : struct
    {
        private int elementCount;
        private int pointer;
        private int bindingPoint;
        private bool disposedValue;

        public GpuShaderStorageBuffer(int pointer, int elementCount, BufferUsageHint hint, int bindingPoint)
        {
            this.elementCount = elementCount;
            this.pointer = pointer;
            GL.NamedBufferData(pointer, elementCount * Marshal.SizeOf<T>(), IntPtr.Zero, hint);
            this.bindingPoint = bindingPoint;
        }
        public GpuShaderStorageBuffer(int pointer, [NotNull] T[] data, BufferUsageHint hint, int bindingPoint)
        {
            if (data is null) 
                throw new ArgumentNullException(nameof(data));

            this.elementCount = data.Length;
            this.pointer = pointer;
            GL.NamedBufferData(pointer, elementCount * Marshal.SizeOf<T>(), data, hint);
            this.bindingPoint = bindingPoint;
        }
        ~GpuShaderStorageBuffer() {
            Dispose(disposing: false);
        }
        public int GetPointer() => pointer;
        public int GetBindingPoint() => bindingPoint;
        public int Count() => elementCount;
        public void Upload(int elementOffset, [NotNull] T[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.Length + elementOffset > elementCount)
                throw new ArgumentOutOfRangeException(nameof(data), " exceeds the buffer capacity.");

            GL.NamedBufferSubData<T>(pointer, (elementOffset + data.Length) * Marshal.SizeOf<T>(), data.Length * Marshal.SizeOf<T>(), data);
        }
        public void Bind() => GL.BindBuffer(BufferTarget.ShaderStorageBuffer, pointer);

        public void Delete()
        {
            if (pointer != -1)
            {
                GL.DeleteBuffer(pointer);
                pointer = -1;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    Delete();
                
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