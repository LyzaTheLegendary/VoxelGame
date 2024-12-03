using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Graphics.GpuMemory
{
    public class GpuBufferStructure : IDisposable
    {
        private List<int> attributes;
        private readonly int pointer;
        private bool disposedValue;

        public GpuBufferStructure(int pointer) {
            this.pointer = pointer;
            attributes = new List<int>();
        }
        public int GetPointer() => pointer;
        public int Attributes() => attributes.Count;

        public void SetVertexArray<T>(GpuArrayBuffer<T> buffer) where T : struct
        {
            Bind();
            buffer.Bind();
            GL.VertexArrayVertexBuffer(pointer, 0, buffer.GetPointer(), IntPtr.Zero, Marshal.SizeOf<T>());
        }
        public void AddAttribute(int elementCount, VertexAttribType type, bool normalized = false, int offset = 0)
        {
            int index = attributes.Count;
            attributes.Add(index);

            GL.EnableVertexArrayAttrib(pointer, index);
            GL.VertexArrayAttribBinding(pointer, index, 0);
            GL.VertexArrayAttribFormat(pointer, index, elementCount, type, normalized, offset);
            

        }
        public void RemoveAttribute(int index)
        {
            attributes.RemoveAt(index);
            GL.DisableVertexArrayAttrib(pointer, index);
        }
        public void Bind() => GL.BindVertexArray(pointer);
        public void UnBind() => GL.BindVertexArray(0);
        public void Delete()
        {
            if (pointer != -1)
                GL.DeleteVertexArray(pointer);
        }
        public void Clear()
        {
            for (int index = 0; index < attributes.Count; index++)
                GL.DisableVertexArrayAttrib(pointer, index);

            attributes.Clear();
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

        ~GpuBufferStructure() => UnBind();
        

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
