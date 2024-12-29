using OpenTK.Graphics.OpenGL4;
using Utils;
namespace Resources.Components
{
    public class Bitmap : IComponent
    {
        public PixelFormat Format { get; private set; }
        public byte[] Data { get; private set; } = Array.Empty<byte>();
        public int Height { get; private set; }
        public int Width { get; private set; }

        public bool IsAtlas { get; private set; }
        public int Rows { get; private set; } = 0;
        public int Columns { get; private set; } = 0;
        public int CellWidth { get; private set; } = 0;
        public int CellHeight { get; private set; } = 0;
        public int TotalCells { get; private set; } = 0;

        public Bitmap(Stream stream)
        {
            Format = (PixelFormat)stream.Read<int>();
            Height = stream.Read<int>();
            Width = stream.Read<int>();

            if(IsAtlas = stream.ReadByte() == 1)
            {
                Rows = stream.Read<int>();
                Columns = stream.Read<int>();
                CellWidth = stream.Read<int>();
                CellHeight = stream.Read<int>();
                TotalCells = Rows * Columns;
            }

            int length = stream.Read<int>();
            Data = new byte[length];

            stream.ReadExactly(Data, 0, length);
        }
    }
}
