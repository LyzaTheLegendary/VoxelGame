using OpenTK.Graphics.OpenGL4;
using Resources.Components;

namespace Graphics.GpuTextures
{
    public class Texture2D : IDisposable
    {
        public PixelFormat Format { get; private set; } = (PixelFormat)999;
        public int Height { get; private set; } = 0;
        public int Width { get; private set; } = 0;
        public int Size { get; private set; } = 0;
        public long Handle { get; private set; } = 0;
        private int pointer = 0;
        private bool disposedValue;
        public Texture2D(int pointer)
        {
            this.pointer = pointer;
        }

        public void Upload(Bitmap bitmap)
        {
            Format = bitmap.Format;
            Height = bitmap.Height;
            Width = bitmap.Width;
            Size = bitmap.Data.Length;

            // Upload the texture data directly to the texture object
            GL.TextureStorage2D(pointer, 1, SizedInternalFormat.Rgba8, Width, Height);
            GL.TextureSubImage2D(pointer, 0, 0, 0, Width, Height, Format, PixelType.UnsignedByte, bitmap.Data);

            // Set texture parameters directly on the texture object
            GL.TextureParameter(pointer, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(pointer, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(pointer, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(pointer, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
                
            GL.GenerateTextureMipmap(pointer);
        }
        public int GetPointer() => pointer;
        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, pointer);
            //GL.ActiveTexture(TextureUnit.Texture0);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    MakeNonResident();
                    GL.DeleteTexture(pointer);
                    disposedValue = true;
                }
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

            Handle = 0;
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
