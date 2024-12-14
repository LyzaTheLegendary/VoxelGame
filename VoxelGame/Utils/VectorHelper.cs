using OpenTK.Mathematics;

namespace Utils
{
    public static class VectorHelper
    {
        public static int GetByIndex(this Vector3i vector, int index)
        {
            return index switch
            {
                0 => vector.X,
                1 => vector.Y,
                2 => vector.Z,
                _ => throw new IndexOutOfRangeException("Index out of range")
            };
        }
        public static void SetByIndex(this Vector3i vector, int index, int value)
        {
            switch (index)
            {
                case 0:
                    vector.X = value;
                    break;
                case 1:
                    vector.Y = value;
                    break;
                case 2:
                    vector.Z = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Index out of range");
            }
        }
    }
}
