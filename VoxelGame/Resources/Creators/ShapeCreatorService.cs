using OpenTK.Mathematics;
using Resources.Creators;

using Utils;

namespace Resources.Creators
{
    public class ShapeCreatorService : ICreatorService
    {
        public string Filename { get; init; }
        public FileType FileType { get {  return FileType.SHAPE; } }
        IEnumerable<byte> data;
        public ShapeCreatorService(string filename, string shapeName, uint[] indices, Vector3[] vertices)
        {
            Filename = filename;

            List<byte> data = new List<byte>();
            FileHeader header = new FileHeader()
            {
                Flags = 0,
                Version = 1,
                Type = FileType.SHAPE
            };

            data.Write(header);
            data.Write(shapeName);
            data.Write<int>(indices.Length);

            for(int i = 0; i < indices.Length; i++)
                data.Write<uint>(indices[i]);

            data.Write<int>(vertices.Length);

            for (int i = 0; i < vertices.Length; i++)
                data.Write(vertices[i]);

            this.data = data;
        }
        public IEnumerable<byte> GetResource() => data.ToArray();
    }
}
