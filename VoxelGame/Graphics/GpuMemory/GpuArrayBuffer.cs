using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Graphics.GpuMemory
{
    public class GpuArrayBuffer<T>: IDisposable where T: struct
    {
        public readonly BufferUsageHint usage;
        private readonly BufferTarget bufferTarget;
        private int pointer;
        private int elements;
        private bool disposedValue;

        public GpuArrayBuffer(BufferUsageHint usage, int pointer, BufferTarget bufferTarget)
        {
            this.usage = usage;
            this.pointer = pointer;
            elements = 0;
            this.bufferTarget = bufferTarget;
        }

        ~GpuArrayBuffer() => UnBind();
        public int GetPointer() => pointer;
        public int Count() => elements;
        public BufferTarget GetTarget() => bufferTarget;
        public void Bind() => GL.BindBuffer(bufferTarget, pointer);
        public void UnBind() => GL.BindBuffer(bufferTarget, 0);
        public void Upload(IEnumerable<T> data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            T[] dataArray = data as T[] ?? data.ToArray();

            GL.NamedBufferData(pointer, dataArray.Length * Marshal.SizeOf<T>(), dataArray, usage);
            elements = dataArray.Length; // Update only after the operation
        }

        public void Delete()
        {
            GL.DeleteBuffer(pointer);
            pointer = -1;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Delete();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
