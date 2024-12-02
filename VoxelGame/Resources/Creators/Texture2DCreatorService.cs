using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using Utils;

namespace Resources.Creators
{
    public class Texture2DCreatorService : ICreatorService
    {
        public string Filename { get; init; }
        public FileType FileType => FileType.TEXTURE;
        private PixelInternalFormat pixelFormat;
        private byte[] bitmap;
        public Texture2DCreatorService(string filename, byte[] imageData)
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

            bitmap = result.Data;
        }
        public IEnumerable<byte> GetResource()
        {
            var data = new List<byte>();

            data.Write((int)pixelFormat);
            data.AddRange(bitmap);

            return data;
        }
    }
}
