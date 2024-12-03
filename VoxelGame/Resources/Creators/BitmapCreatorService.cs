using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using Utils;

namespace Resources.Creators
{
    public class BitmapCreatorService : ICreatorService
    {
        public string Filename { get; init; }
        public FileType FileType => FileType.TEXTURE;
        private PixelInternalFormat pixelFormat;
        private int height;
        private int width;
        private byte[] bitmap;
        public BitmapCreatorService(string filename, byte[] imageData)
        {
            Filename = filename;

            ImageResult result = ImageResult.FromMemory(imageData);
            ColorComponents components = result.Comp;
            
            switch(components)
            {
                case ColorComponents.RedGreenBlueAlpha:
                    pixelFormat = PixelInternalFormat.Rgba;
                    break;
                case ColorComponents.RedGreenBlue:
                    pixelFormat = PixelInternalFormat.Rgb;
                    break;
                default:
                    throw new ArgumentException("Unsupported pixel format");
            }

            height = result.Height;
            width = result.Width;

            bitmap = result.Data;
        }
        public IEnumerable<byte> GetResource()
        {
            var data = new List<byte>();

            data.Write((int)pixelFormat);
            data.Write(height);
            data.Write(width);

            data.Write(bitmap.Length);
            data.AddRange(bitmap);

            return data;
        }
    }
}
