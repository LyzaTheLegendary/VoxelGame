using Graphics;
using OpenTK.Graphics.OpenGL4;
using Resources;
using Utils;

namespace Graphics.GpuTextures
{
    public class Texture2D : IDisposable
    {
        public PixelFormat Format { get; private set; }
        public TextureUnit TextureUnit { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
        public int Size { get; private set; }
        private int pointer = 0;
        private bool disposedValue;
        public Texture2D(PixelFormat format, TextureUnit textureUnit, int pointer)
        {
            Format       = format;
            TextureUnit  = textureUnit;
            this.pointer = pointer;
            Size = 0;
        }

        public void Upload(byte[] bitmap, int width, int height)
        {
            Height = height;
            Width = width;
            Size = bitmap.Length;

            GL.ActiveTexture(TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, pointer);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, Format, PixelType.UnsignedByte, bitmap);

            GL.TextureParameter(pointer, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(pointer, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(pointer, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(pointer, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
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
