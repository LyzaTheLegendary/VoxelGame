using Graphics;
using OpenTK.Mathematics;

using Utils;

namespace Resources.Creators
{
    public class ShapeCreatorService : ICreatorService
    {
        public string Filename { get; init; }
        public FileType FileType { get {  return FileType.SHAPE; } }
        private IEnumerable<byte> data;
        public ShapeCreatorService(string filename, string shapeName, Vertex[] vertices, uint[] indices)
        {
            Filename = filename;

            List<byte> data = new List<byte>();

            data.Write(shapeName);

            data.Write<int>(vertices.Length);
            data.Write<int>(indices.Length);

            for(int i = 0; i < vertices.Length; i++)
                data.Write<Vertex>(vertices[i]);
            
            for(int i = 0; i < indices.Length; i++)
                data.Write<uint>(indices[i]);

            //data.Write<int>(indices.Length);

            //for(int i = 0; i < indices.Length; i++)
            //    data.Write<uint>(indices[i]);

            //data.Write<int>(vertices.Length);

            //for (int i = 0; i < vertices.Length; i++)
            //    data.Write(vertices[i]);

            //data.Write<int>(uniforms.Length);

            //for(int i = 0; i < uniforms.Length; i++)
            //    data.Write(uniforms[i]);

            this.data = data;
        }
        public IEnumerable<byte> GetResource() => data.ToArray();
    }
}
