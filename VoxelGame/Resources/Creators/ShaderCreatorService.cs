
using OpenTK.Graphics.OpenGL4;
using Utils;

namespace Resources.Creators
{
    public struct ShaderCreatorData
    {
        public ShaderType Type { get; init; }
        public string ShaderCode { get; init; }
    }
    public class ShaderCreatorService : ICreatorService
    {
        List<byte> data;
        public string Filename { get; init; }
        public FileType FileType { get { return FileType.SHADER; } }
        public ShaderCreatorService(string filename, ShaderCreatorData[] shaderData) {
            Filename = filename;
            data = new();

            data.Write(filename);
            data.Write((byte)shaderData.Length);
            foreach (ShaderCreatorData shader in shaderData)
            {
                data.Write((int)shader.Type);
                data.Write(shader.ShaderCode);
            }
        }

        public IEnumerable<byte> GetResource() => data.ToArray();
    }
}
