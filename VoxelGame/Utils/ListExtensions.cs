using System.Runtime.InteropServices;
using System.Text;

namespace Utils
{
    public static class ListExtensions
    {
        public static void Write<T>(this List<byte> list, T value) where T : struct {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write(buffer, value);

            list.AddRange(buffer);
        }

        public static void Write(this List<byte> list, string value) {
            byte[] buffer = Encoding.UTF8.GetBytes(value);

            list.Write<int>(buffer.Length);
            list.AddRange(buffer);
        }
    }
}
