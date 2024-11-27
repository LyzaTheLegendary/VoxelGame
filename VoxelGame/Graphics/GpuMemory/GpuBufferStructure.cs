using OpenTK.Graphics.OpenGL4;

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
        public void AddAttribute<T>(int count, VertexAttribType type, bool normalized = false, int stride = 0, int offset = 0) where T : struct
        {
            int index = attributes.Count;
            attributes.Add(index);


            GL.VertexArrayAttribFormat(pointer, index, count, type, normalized, offset);
            GL.EnableVertexArrayAttrib(pointer, index);
        }
        public void RemoveAttribute(int index)
        {
            attributes.RemoveAt(index);
            GL.DisableVertexArrayAttrib(pointer, index);
        }
        public void Bind() => GL.BindVertexArray(pointer);
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

        ~GpuBufferStructure()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
