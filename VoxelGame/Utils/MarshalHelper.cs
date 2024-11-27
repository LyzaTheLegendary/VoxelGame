using System.Runtime.InteropServices;

namespace VoxelGame.Utils
{
    public static class MarshalHelper
    {
        public static byte[] ToBytes<T>(T obj) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] byteArray = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, ptr, false);
                Marshal.Copy(ptr, byteArray, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return byteArray;
        }

        public static T ToStruct<T>(byte[] data) where T : struct
        {
            int size = Marshal.SizeOf<T>();

            if (data.Length < size)
                throw new ArgumentException("Byte array is too small for the structure.");

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(data, 0, ptr, size);
                return Marshal.PtrToStructure<T>(ptr)!;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
