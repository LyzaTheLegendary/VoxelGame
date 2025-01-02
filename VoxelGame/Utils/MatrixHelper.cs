using OpenTK.Mathematics;

namespace Utils
{
    public static class MatrixHelper
    {
        public static Matrix4 Lerp(Matrix4 from, Matrix4 to, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            return new Matrix4(
                Lerp(from.M11, to.M11, t), Lerp(from.M12, to.M12, t), Lerp(from.M13, to.M13, t), Lerp(from.M14, to.M14, t),
                Lerp(from.M21, to.M21, t), Lerp(from.M22, to.M22, t), Lerp(from.M23, to.M23, t), Lerp(from.M24, to.M24, t),
                Lerp(from.M31, to.M31, t), Lerp(from.M32, to.M32, t), Lerp(from.M33, to.M33, t), Lerp(from.M34, to.M34, t),
                Lerp(from.M41, to.M41, t), Lerp(from.M42, to.M42, t), Lerp(from.M43, to.M43, t), Lerp(from.M44, to.M44, t)
            );
        }
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
