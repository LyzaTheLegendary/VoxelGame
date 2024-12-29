using OpenTK.Mathematics;

namespace Utils
{
    public static class MatrixHelper
    {
        public static Matrix3 Lerp(Matrix3 from, Matrix3 to, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            return new Matrix3(
                Lerp(from.M11, to.M11, t), Lerp(from.M12, to.M12, t), Lerp(from.M13, to.M13, t),
                Lerp(from.M21, to.M21, t), Lerp(from.M22, to.M22, t), Lerp(from.M23, to.M23, t),
                Lerp(from.M31, to.M31, t), Lerp(from.M32, to.M32, t), Lerp(from.M33, to.M33, t)
            );
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
