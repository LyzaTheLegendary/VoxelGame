using System.Runtime.InteropServices;
using System.Text;

namespace Utils
{
    public static class StreamExtensions
    {
        public static T Read<T>(this Stream stream) where T : struct
        {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            stream.ReadExactly(buffer);
            return MemoryMarshal.Read<T>(buffer);
        }

        public static void Write<T>(this Stream stream, T value) where T : struct
        {
            Span<byte> buffer = stackalloc byte[Marshal.SizeOf<T>()];
            MemoryMarshal.Write<T>(buffer, value);

            stream.Write(buffer);
        }

        public static string ReadString(this Stream stream)
        {
            int strLen = stream.Read<int>();

            Span<byte> bytes = strLen > 1024 ? new byte[strLen] : stackalloc byte[strLen]; 

            stream.ReadExactly(bytes);

            return Encoding.UTF8.GetString(bytes);
        }

        public static void Write(this Stream stream, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            
            stream.Write<int>(bytes.Length);
            stream.Write(bytes);
        }
    }
}
