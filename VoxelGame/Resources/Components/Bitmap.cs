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

        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            using (var stream = new MemoryStream(data as byte[] ?? data.ToArray()))
            {
                Format = (PixelFormat)stream.Read<int>();
                Height = stream.Read<int>();
                Width = stream.Read<int>();

                int length = stream.Read<int>();
                Data = new byte[length];

                stream.ReadExactly(Data, 0, length);
            }
        }
    }
}
