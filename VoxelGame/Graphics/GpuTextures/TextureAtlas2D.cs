using OpenTK.Graphics.OpenGL4;
using Resources.Components;

namespace Graphics.GpuTextures
{
    public class TextureAtlas2D : IDisposable
    {
        public PixelFormat Format { get; private set; } = (PixelFormat)999;
        public TextureUnit TextureUnit { get; private set; }
        public int Height { get; private set; } = 0;
        public int Width { get; private set; } = 0;
        public int Size { get; private set; } = 0;
        public int Rows { get; private set; } = 0;
        public int Columns { get; private set; } = 0;

        private int pointer;
        private bool disposedValue = false;
        public TextureAtlas2D(TextureUnit textureUnit, int pointer)
        {
            TextureUnit = textureUnit;
            this.pointer = pointer;
        }

        public void Upload(Bitmap bitmap)
        {
            Format = bitmap.Format;
            Height = bitmap.Height;
            Width = bitmap.Width;
            Size = bitmap.Width * bitmap.Height;
            Rows = bitmap.Rows;
            Columns = bitmap.Columns;

            GL.ActiveTexture(TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, pointer);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, Format, PixelType.UnsignedByte, bitmap.Data);

            GL.TextureParameter(pointer, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TextureParameter(pointer, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TextureParameter(pointer, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(pointer, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        }
        public int GetPointer() => pointer;
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, pointer);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    GL.DeleteTexture(pointer);

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
