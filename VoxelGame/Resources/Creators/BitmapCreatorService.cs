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

        private byte isAtlas;
        private int rows;
        private int columns;
        private int cellWidth;
        private int cellHeight;
        public BitmapCreatorService(string filename, byte[] imageData, bool isAtlas = false, int cellWidth = 0, int cellHeight = 0)
        {
            Filename = filename;
            this.isAtlas = isAtlas ? (byte)1 : (byte)0;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;

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

            if (isAtlas)
            {              
                this.columns = width / cellHeight;
                this.rows = width / cellHeight;
            }

            bitmap = result.Data;
        }
        public IEnumerable<byte> GetResource()
        {
            var data = new List<byte>();

            data.Write((int)pixelFormat);
            data.Write(height);
            data.Write(width);

            data.Write((byte)isAtlas);
            if(isAtlas == 1)
            {
                data.Write(rows);
                data.Write(columns);
                data.Write(cellWidth);
                data.Write(cellHeight);
            }

            data.Write(bitmap.Length);
            data.AddRange(bitmap);

            return data;
        }
    }
}
