using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BonedVertex
    {
        public Vector3 Position { get; init; }
        public Vector2 TexCoord { get; init; }
        public int BoneIndex { get; init; }
        public float Weight { get; init; }
        public BonedVertex() { }
        public BonedVertex(Vector3 position, Vector2 texCoord, int boneIndex, float weight)
        {
            Position = position;
            TexCoord = texCoord;
            BoneIndex = boneIndex;
            Weight = weight;
        }
    }
}
