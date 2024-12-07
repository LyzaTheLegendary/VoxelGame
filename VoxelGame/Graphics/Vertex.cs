using OpenTK.Mathematics;

namespace Graphics
{
    public struct Vertex
    {
        public Vector3 Position { get; init; }
        public Vector2 TexCoord { get; init; }

        public Vertex() { }
        public Vertex(Vector3 position, Vector2 texCoord)
        {
            Position = position;
            TexCoord = texCoord;
        }
    }
}
