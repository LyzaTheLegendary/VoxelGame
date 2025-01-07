using OpenTK.Mathematics;

namespace VoxelGame.Utils;

public static class QuaternionHelper
{
    public static float Dot(Quaternion a, Quaternion b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }
}