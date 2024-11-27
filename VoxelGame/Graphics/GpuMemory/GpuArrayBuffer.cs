using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Graphics.GpuMemory
{
    public class GpuArrayBuffer<T>: IDisposable where T: struct
    {
        public readonly BufferUsageHint usage;
        private readonly BufferTarget bufferTarget;
        private int pointer;
        private int size;
        private bool disposedValue;

        public GpuArrayBuffer(BufferUsageHint usage, int pointer, BufferTarget bufferTarget)
        {
            this.usage = usage;
            this.pointer = pointer;
            size = 0;
            this.bufferTarget = bufferTarget;
        }

        ~GpuArrayBuffer() => Dispose(disposing: false);
        public int GetPointer() => pointer;
        public BufferTarget GetTarget() => bufferTarget;
        public void Bind() => GL.BindBuffer(bufferTarget, pointer);
        
        public void Upload(IEnumerable<T> data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            T[] dataArray = data as T[] ?? data.ToArray();
            size = dataArray.Length;

            GL.NamedBufferData(pointer, size * Marshal.SizeOf<T>(), dataArray, usage);
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
