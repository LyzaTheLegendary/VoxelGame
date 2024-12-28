using OpenTK.Graphics.OpenGL4;
using Resources.Components;

namespace Graphics.GpuTextures
{
    public class Texture2D : IDisposable
    {
        public PixelFormat Format { get; private set; } = (PixelFormat)999;
        public TextureUnit TextureUnit { get; private set; }
        public int Height { get; private set; } = 0;
        public int Width { get; private set; } = 0;
        public int Size { get; private set; } = 0;
        public long Handle { get; private set; } = 0;
        private int pointer = 0;
        private bool disposedValue;
        public Texture2D(TextureUnit textureUnit, int pointer)
        {
            TextureUnit  = textureUnit;
            this.pointer = pointer;
        }

        public void Upload(Bitmap bitmap)
        {
            Height = bitmap.Height;
            Width = bitmap.Width;
            Size = bitmap.Data.Length;

            GL.ActiveTexture(TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, pointer);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, Format, PixelType.UnsignedByte, bitmap.Data);

            GL.TextureParameter(pointer, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(pointer, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
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
                {
                    MakeNonResident();
                    GL.DeleteTexture(pointer);
                }

                disposedValue = true;
            }
        }

        public void MakeResident()
        {
            long handle = GL.Arb.GetTextureHandle(pointer);
            
            GL.Arb.MakeTextureHandleResident(handle);

            Handle = handle;
        }

        public void MakeNonResident()
        {
            if(Handle != 0)
                GL.Arb.MakeTextureHandleNonResident(Handle);
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
